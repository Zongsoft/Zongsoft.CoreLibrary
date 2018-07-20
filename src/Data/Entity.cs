/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2016-2018 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.CoreLibrary.
 *
 * Zongsoft.CoreLibrary is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.CoreLibrary; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 提供 <see cref="IEntity"/> 数据实体的动态编译及构建的静态类。
	/// </summary>
	public static class Entity
	{
		#region 常量定义
		private const string ASSEMBLY_NAME = "Zongsoft.Dynamics.Entities";

		private const string MASK_VARIABLE = "$MASK$";
		private const string PROPERTY_NAMES_VARIABLE = "$$PROPERTY_NAMES";
		private const string PROPERTY_TOKENS_VARIABLE = "$$PROPERTY_TOKENS";
		#endregion

		#region 成员字段
		private static Type PROPERTY_TOKEN_TYPE = null;
		private static FieldInfo PROPERTY_TOKEN_GETTER_FIELD;
		private static FieldInfo PROPERTY_TOKEN_SETTER_FIELD;
		private static FieldInfo PROPERTY_TOKEN_ORDINAL_FIELD;
		private static ConstructorBuilder PROPERTY_TOKEN_CONSTRUCTOR;

		private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
		private static readonly Dictionary<Type, Func<object>> _cache = new Dictionary<Type, Func<object>>();

		//private static readonly AssemblyBuilder _assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ASSEMBLY_NAME), AssemblyBuilderAccess.Run);
		//private static readonly ModuleBuilder _module = _assembly.DefineDynamicModule(ASSEMBLY_NAME);

		private static readonly AssemblyBuilder _assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(ASSEMBLY_NAME), AssemblyBuilderAccess.RunAndSave);
		private static readonly ModuleBuilder _module = _assembly.DefineDynamicModule(ASSEMBLY_NAME, ASSEMBLY_NAME + ".dll");
		#endregion

		#region 公共方法
		public static T Build<T>()
		{
			return (T)GetCreator(typeof(T))();
		}

		public static T Build<T>(Action<T> map)
		{
			var entity = (T)GetCreator(typeof(T))();
			map?.Invoke(entity);
			return entity;
		}

		public static IEnumerable<T> Build<T>(int count, Action<T, int> map = null)
		{
			if(count < 1)
				throw new ArgumentOutOfRangeException(nameof(count));

			var creator = GetCreator(typeof(T));

			if(map == null)
			{
				for(int i = 0; i < count; i++)
				{
					yield return (T)creator();
				}
			}
			else
			{
				for(int i = 0; i < count; i++)
				{
					var entity = (T)creator();
					map(entity, i);
					yield return entity;
				}
			}
		}

		public static object Build(Type type)
		{
			return GetCreator(type)();
		}

		public static object Build(Type type, Action<object> map)
		{
			var entity = GetCreator(type)();
			map?.Invoke(entity);
			return entity;
		}

		public static IEnumerable Build(Type type, int count, Action<object, int> map = null)
		{
			if(count < 1)
				throw new ArgumentOutOfRangeException(nameof(count));

			var creator = GetCreator(type);

			if(map == null)
			{
				for(int i = 0; i < count; i++)
				{
					yield return creator();
				}
			}
			else
			{
				for(int i = 0; i < count; i++)
				{
					var entity = creator();
					map(entity, i);
					yield return entity;
				}
			}
		}

		public static Func<object> GetCreator(Type type)
		{
			_locker.EnterReadLock();
			var existed = _cache.TryGetValue(type, out var creator);
			_locker.ExitReadLock();

			if(existed)
				return creator;

			if(!type.IsInterface)
				throw new ArgumentException($"The '{type.FullName}' type must be an interface.");

			if(type.GetEvents().Length > 0)
				throw new ArgumentException($"The '{type.FullName}' interface cannot define any events.");

			if(type.GetMethods().Length > type.GetProperties().Length * 2)
				throw new ArgumentException($"The '{type.FullName}' interface cannot define any methods.");

			try
			{
				_locker.EnterWriteLock();

				if(!_cache.TryGetValue(type, out creator))
					creator = _cache[type] = Compile(type);

				return creator;
			}
			finally
			{
				_locker.ExitWriteLock();
			}
		}

		public static void Save()
		{
			_assembly.Save(ASSEMBLY_NAME + ".dll");
		}
		#endregion

		#region 私有方法
		private static Func<object> Compile(Type type)
		{
			ILGenerator generator;

			//如果是首次编译，则首先生成属性标记类型
			if(PROPERTY_TOKEN_TYPE == null)
				GeneratePropertyTokenClass();

			var builder = _module.DefineType(
				GetClassName(type),
				TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);

			//添加类型的实现接口声明
			builder.AddInterfaceImplementation(type);
			builder.AddInterfaceImplementation(typeof(IEntity));

			//添加所有接口实现声明，并获取各接口的属性集，以及确认生成“INotifyPropertyChanged”接口
			var properties = MakeInterfaces(type, builder, out var propertyChanged);

			//生成所有接口定义的注解(自定义特性)
			GenerateAnnotations(builder, new HashSet<Type>());

			//获取可写属性的数量
			var countWritable = properties.Count(p => p.CanWrite);

			//定义掩码字段
			FieldBuilder mask = null;

			if(countWritable <= 8)
				mask = builder.DefineField(MASK_VARIABLE, typeof(byte), FieldAttributes.Private);
			else if(countWritable <= 16)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt16), FieldAttributes.Private);
			else if(countWritable <= 32)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt32), FieldAttributes.Private);
			else if(countWritable <= 64)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt64), FieldAttributes.Private);
			else
				mask = builder.DefineField(MASK_VARIABLE, typeof(byte[]), FieldAttributes.Private);

			//生成属性定义以及嵌套子类
			GenerateProperties(builder, mask, properties, propertyChanged, out var methods);

			//生成构造函数
			GenerateConstructor(builder, countWritable, mask, properties);

			//生成静态构造函数
			GenerateTypeInitializer(builder, properties, methods, out var names, out var tokens);

			//生成“HasChanges”方法
			GenerateHasChangesMethod(builder, mask, names, tokens);

			//生成“GetChanges”方法
			GenerateGetChangesMethod(builder, mask, names, tokens);

			//生成“TryGetValue”方法
			GenerateTryGetValueMethod(builder, mask, tokens);

			//生成“TrySetValue”方法
			GenerateTrySetValueMethod(builder, tokens);

			//构建类型
			type = builder.CreateType();

			//生成创建实例的动态方法
			var creator = new DynamicMethod("Create", typeof(object), Type.EmptyTypes);

			generator = creator.GetILGenerator();
			generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
			generator.Emit(OpCodes.Ret);

			//返回实例创建方法的委托
			return (Func<object>)creator.CreateDelegate(typeof(Func<object>));
		}

		private static void GenerateProperties(TypeBuilder builder, FieldBuilder mask, IList<PropertyMetadata> properties, FieldBuilder propertyChangedField, out MethodToken[] methods)
		{
			//生成嵌套匿名委托静态类
			var nested = builder.DefineNestedType("!Methods!", TypeAttributes.NestedPrivate | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);

			//创建返回参数（即方法标记）
			methods = new MethodToken[properties.Count];

			//定义只读属性的递增数量
			var timesReadOnly = 0;
			//定义可写属性的总数量
			var countWritable = properties.Count(p => p.CanWrite);

			//生成属性定义
			for(int i = 0; i < properties.Count; i++)
			{
				var metadata = properties[i].Metadata;
				var fieldType = properties[i].PropertyType;
				FieldBuilder field = null;

				//如果指定了实体属性标签，则进行必要的验证
				if(metadata != null)
				{
					switch(metadata.Mode)
					{
						case PropertyImplementationMode.Default:
							if(metadata.Type != null)
							{
								if(!properties[i].PropertyType.IsAssignableFrom(metadata.Type))
									throw new InvalidOperationException($"The '{metadata.Type}' type of the '{properties[i].Name}' PropertyAttribute does not implement '{properties[i].PropertyType}' interface or class.");

								fieldType = metadata.Type;
							}

							break;
						case PropertyImplementationMode.Singleton:
							if(metadata.Type != null && !properties[i].SingletonFactoryEnabled)
							{
								if(metadata.Type.IsValueType)
									throw new InvalidOperationException($"The {metadata.Type} type of singleton cannot be a value type.");

								if(!properties[i].PropertyType.IsAssignableFrom(metadata.Type))
									throw new InvalidOperationException($"The '{metadata.Type}' type of the '{properties[i].Name}' PropertyAttribute does not implement '{properties[i].PropertyType}' interface or class.");

								fieldType = metadata.Type;
							}

							break;
						case PropertyImplementationMode.Extension:
							if(metadata.Type == null)
								throw new InvalidOperationException($"Missing type of the '{properties[i].Name}' property attribute.");

							break;
					}
				}

				if(properties[i].CanWrite || properties[i].HasDefaultValue ||
				  (metadata != null && metadata.Mode == PropertyImplementationMode.Singleton))
					field = properties[i].Field = builder.DefineField(properties[i].GetFieldName(), fieldType, FieldAttributes.Private);

				var property = properties[i].Builder = builder.DefineProperty(
					properties[i].GetName(),
					PropertyAttributes.None,
					properties[i].PropertyType,
					null);

				//获取当前属性的所有自定义标签
				var customAttributes = properties[i].GetCustomAttributesData();

				//设置属性的自定义标签
				if(customAttributes != null && customAttributes.Count > 0)
				{
					foreach(var customAttribute in customAttributes)
					{
						var annotation = GetAnnotation(customAttribute);

						if(annotation != null)
							property.SetCustomAttribute(annotation);
					}
				}

				//定义属性的获取器方法
				var getter = builder.DefineMethod(properties[i].GetGetterName(),
					(properties[i].IsExplicitImplementation ? MethodAttributes.Private : MethodAttributes.Public) | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
					properties[i].PropertyType,
					Type.EmptyTypes);

				//如果当前属性需要显式实现，则定义其方法覆盖元数据
				if(properties[i].IsExplicitImplementation)
					builder.DefineMethodOverride(getter, properties[i].GetMethod);

				property.SetGetMethod(getter);
				var generator = getter.GetILGenerator();

				if(metadata == null || metadata.Mode == PropertyImplementationMode.Default)
				{
					if(field == null)
					{
						//抛出“NotSupportedException”异常
						generator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes));
						generator.Emit(OpCodes.Throw);
					}
					else
					{
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldfld, field);
						generator.Emit(OpCodes.Ret);
					}
				}
				else if(metadata.Mode == PropertyImplementationMode.Extension)
				{
					var method = metadata.Type.GetMethod("Get" + properties[i].Name, BindingFlags.Public | BindingFlags.Static, null, new Type[] { properties[i].DeclaringType, properties[i].PropertyType }, null) ??
								 metadata.Type.GetMethod("Get" + properties[i].Name, BindingFlags.Public | BindingFlags.Static, null, new Type[] { properties[i].DeclaringType }, null);

					if(method == null)
						throw new InvalidOperationException($"Not found the extension method of the {properties[i].Name} property in the {metadata.Type.FullName} extension type.");

					if(method.ReturnType == null || method.ReturnType == typeof(void) || !properties[i].PropertyType.IsAssignableFrom(method.ReturnType))
						throw new InvalidOperationException($"The return type of the '{method}' extension method is missing or invalid.");

					generator.Emit(OpCodes.Ldarg_0);

					if(method.GetParameters().Length == 2)
					{
						if(field == null)
							LoadDefaultValue(generator, properties[i].PropertyType);
						else
						{
							generator.Emit(OpCodes.Ldarg_0);
							generator.Emit(OpCodes.Ldfld, field);
						}
					}

					generator.Emit(OpCodes.Call, method);
					generator.Emit(OpCodes.Ret);
				}
				else if(metadata.Mode == PropertyImplementationMode.Singleton)
				{
					if(properties[i].HasDefaultValue)
					{
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldfld, field);
						generator.Emit(OpCodes.Ret);
					}
					else
					{
						properties[i].Synchrolock = builder.DefineField(properties[i].GetFieldName("@LOCK"), typeof(object), FieldAttributes.Private | FieldAttributes.InitOnly);
						ConstructorInfo ctor = null;

						if(!properties[i].SingletonFactoryEnabled)
						{
							var implementationType = GetCollectionImplementationType(metadata.Type ?? field.FieldType);
							ctor = implementationType.GetConstructor(Type.EmptyTypes);

							if(ctor == null)
								throw new InvalidOperationException($"The '{implementationType}' type of the '{properties[i].Name}' property is missing the default constructor.");
						}

						generator.DeclareLocal(typeof(object)); // for synchrolock variable
						generator.DeclareLocal(typeof(bool));

						var SETTER_EXIT_LABEL = generator.DefineLabel();
						var SETTER_LEAVE_LABEL = generator.DefineLabel();
						var SETTER_FINALLY_LABEL = generator.DefineLabel();

						// if($Field == null)
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldfld, field);
						generator.Emit(OpCodes.Brtrue_S, SETTER_EXIT_LABEL);

						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldfld, properties[i].Synchrolock);
						generator.Emit(OpCodes.Stloc_0);
						generator.Emit(OpCodes.Ldc_I4_0);
						generator.Emit(OpCodes.Stloc_1);

						// try begin
						generator.BeginExceptionBlock();

						// lock(Synchrolock)
						generator.Emit(OpCodes.Ldloc_0);
						generator.Emit(OpCodes.Ldloca_S, 1);
						generator.Emit(OpCodes.Call, typeof(Monitor).GetMethod("Enter", new Type[] { typeof(object), typeof(bool).MakeByRefType() }));

						// if($Field != null)
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldfld, field);
						generator.Emit(OpCodes.Brtrue_S, SETTER_LEAVE_LABEL);

						if(ctor != null)
						{
							// $Field = new XXXX();
							generator.Emit(OpCodes.Ldarg_0);
							generator.Emit(OpCodes.Newobj, ctor);
							generator.Emit(OpCodes.Stfld, field);
						}
						else // $Field = FactoryClass.GetProperty(...);
						{
							var factory = properties[i].GetSingletonFactory();

							generator.Emit(OpCodes.Ldarg_0);
							if(factory.GetParameters().Length > 0)
								generator.Emit(OpCodes.Ldarg_0);
							generator.Emit(OpCodes.Call, factory);
							generator.Emit(OpCodes.Stfld, field);
						}

						// try end
						generator.MarkLabel(SETTER_LEAVE_LABEL);

						// finally begin
						generator.BeginFinallyBlock();

						generator.Emit(OpCodes.Ldloc_1);
						generator.Emit(OpCodes.Brfalse_S, SETTER_FINALLY_LABEL);

						generator.Emit(OpCodes.Ldloc_0);
						generator.Emit(OpCodes.Call, typeof(Monitor).GetMethod("Exit", new Type[] { typeof(object) }));

						// finally end
						generator.MarkLabel(SETTER_FINALLY_LABEL);
						generator.EndExceptionBlock();

						generator.MarkLabel(SETTER_EXIT_LABEL);

						// return this.$Field;
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldfld, field);
						generator.Emit(OpCodes.Ret);
					}
				}

				//生成获取属性字段的方法
				var getMethod = nested.DefineMethod("Get" + properties[i].Name,
					MethodAttributes.Assembly | MethodAttributes.Static, CallingConventions.Standard,
					typeof(object),
					new Type[] { property.DeclaringType });

				getMethod.DefineParameter(1, ParameterAttributes.None, "target");

				generator = getMethod.GetILGenerator();
				generator.Emit(OpCodes.Ldarg_0);
				//generator.Emit(OpCodes.Castclass, field.DeclaringType);
				if(field == null)
					generator.Emit(OpCodes.Callvirt, getter);
				else
					generator.Emit(OpCodes.Ldfld, field);
				if(properties[i].PropertyType.IsValueType)
					generator.Emit(OpCodes.Box, properties[i].PropertyType);
				generator.Emit(OpCodes.Ret);

				MethodBuilder setMethod = null;

				if(properties[i].CanWrite)
				{
					//定义属性的设置器方法
					var setter = builder.DefineMethod(properties[i].GetSetterName(),
						(properties[i].IsExplicitImplementation ? MethodAttributes.Private : MethodAttributes.Public) | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
						null,
						new Type[] { properties[i].PropertyType });

					//如果当前属性需要显式实现，则定义其方法覆盖元数据
					if(properties[i].IsExplicitImplementation)
						builder.DefineMethodOverride(setter, properties[i].SetMethod);

					setter.DefineParameter(1, ParameterAttributes.None, "value");
					property.SetSetMethod(setter);
					generator = setter.GetILGenerator();
					var exit = generator.DefineLabel();

					if(metadata == null || metadata.Mode == PropertyImplementationMode.Default)
					{
						if(propertyChangedField != null)
						{
							//生成属性值是否发生改变的判断检测
							GeneratePropertyValueChangeChecker(generator, property, field, exit);
						}
					}
					else if(metadata.Mode == PropertyImplementationMode.Extension)
					{
						var method = metadata.Type.GetMethod("Set" + properties[i].Name, BindingFlags.Public | BindingFlags.Static, null, new Type[] { properties[i].DeclaringType, properties[i].PropertyType }, null);

						if(method == null)
						{
							if(propertyChangedField != null)
							{
								//生成属性值是否发生改变的判断检测
								GeneratePropertyValueChangeChecker(generator, property, field, exit);
							}
						}
						else
						{
							if(method.ReturnType != typeof(bool))
								throw new InvalidOperationException($"Invalid '{method}' extension method, it's return type must be boolean type.");

							generator.Emit(OpCodes.Ldarg_0);
							generator.Emit(OpCodes.Ldarg_1);
							generator.Emit(OpCodes.Call, method);
							generator.Emit(OpCodes.Brfalse_S, exit);
						}
					}
					else if(metadata.Mode == PropertyImplementationMode.Singleton)
					{
						//抛出“NotSupportedException”异常
						generator.Emit(OpCodes.Newobj, typeof(NotSupportedException).GetConstructor(Type.EmptyTypes));
						generator.Emit(OpCodes.Throw);
					}

					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldarg_1);
					generator.Emit(OpCodes.Stfld, field);

					if(countWritable <= 64)
						generator.Emit(OpCodes.Ldarg_0);

					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldfld, mask);

					if(countWritable <= 8)
					{
						generator.Emit(OpCodes.Ldc_I4, (int)Math.Pow(2, i - timesReadOnly));
						generator.Emit(OpCodes.Or);
						generator.Emit(OpCodes.Conv_U1);
						generator.Emit(OpCodes.Stfld, mask);
					}
					else if(countWritable <= 16)
					{
						generator.Emit(OpCodes.Ldc_I4, (int)Math.Pow(2, i - timesReadOnly));
						generator.Emit(OpCodes.Or);
						generator.Emit(OpCodes.Conv_U2);
						generator.Emit(OpCodes.Stfld, mask);
					}
					else if(countWritable <= 32)
					{
						generator.Emit(OpCodes.Ldc_I4, (uint)Math.Pow(2, i - timesReadOnly));
						generator.Emit(OpCodes.Or);
						generator.Emit(OpCodes.Conv_U4);
						generator.Emit(OpCodes.Stfld, mask);
					}
					else if(countWritable <= 64)
					{
						generator.Emit(OpCodes.Ldc_I8, (long)Math.Pow(2, i - timesReadOnly));
						generator.Emit(OpCodes.Or);
						generator.Emit(OpCodes.Conv_U8);
						generator.Emit(OpCodes.Stfld, mask);
					}
					else
					{
						generator.Emit(OpCodes.Ldc_I4, (i - timesReadOnly) / 8);
						generator.Emit(OpCodes.Ldelema, typeof(byte));
						generator.Emit(OpCodes.Dup);
						generator.Emit(OpCodes.Ldind_U1);

						switch((i - timesReadOnly) % 8)
						{
							case 0:
								generator.Emit(OpCodes.Ldc_I4_1);
								break;
							case 1:
								generator.Emit(OpCodes.Ldc_I4_2);
								break;
							case 2:
								generator.Emit(OpCodes.Ldc_I4_4);
								break;
							case 3:
								generator.Emit(OpCodes.Ldc_I4_S, 8);
								break;
							case 4:
								generator.Emit(OpCodes.Ldc_I4_S, 16);
								break;
							case 5:
								generator.Emit(OpCodes.Ldc_I4_S, 32);
								break;
							case 6:
								generator.Emit(OpCodes.Ldc_I4_S, 64);
								break;
							case 7:
								generator.Emit(OpCodes.Ldc_I4_S, 128);
								break;
						}

						generator.Emit(OpCodes.Conv_U1);
						generator.Emit(OpCodes.Or);
						generator.Emit(OpCodes.Conv_U1);
						generator.Emit(OpCodes.Stind_I1);
					}

					//处理“PropertyChanged”事件
					if(propertyChangedField != null)
					{
						var RAISE_LABEL = generator.DefineLabel();

						// this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("xxx"));
						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldfld, propertyChangedField);
						generator.Emit(OpCodes.Dup);
						generator.Emit(OpCodes.Brtrue_S, RAISE_LABEL);

						generator.Emit(OpCodes.Pop);
						generator.Emit(OpCodes.Ret);

						generator.MarkLabel(RAISE_LABEL);

						generator.Emit(OpCodes.Ldarg_0);
						generator.Emit(OpCodes.Ldstr, properties[i].Name);
						generator.Emit(OpCodes.Newobj, typeof(PropertyChangedEventArgs).GetConstructor(new Type[] { typeof(string) }));
						generator.Emit(OpCodes.Call, propertyChangedField.FieldType.GetMethod("Invoke"));
					}

					generator.MarkLabel(exit);
					generator.Emit(OpCodes.Ret);

					//生成设置属性的方法
					setMethod = nested.DefineMethod("Set" + properties[i].Name,
						MethodAttributes.Assembly | MethodAttributes.Static, CallingConventions.Standard,
						null,
						new Type[] { field.DeclaringType, typeof(object) });

					setMethod.DefineParameter(1, ParameterAttributes.None, "target");
					setMethod.DefineParameter(2, ParameterAttributes.None, "value");

					generator = setMethod.GetILGenerator();
					generator.Emit(OpCodes.Ldarg_0);
					//generator.Emit(OpCodes.Castclass, field.DeclaringType);
					generator.Emit(OpCodes.Ldarg_1);
					if(properties[i].PropertyType.IsValueType)
						generator.Emit(OpCodes.Unbox_Any, properties[i].PropertyType);
					else
						generator.Emit(OpCodes.Castclass, properties[i].PropertyType);
					generator.Emit(OpCodes.Call, setter);
					generator.Emit(OpCodes.Ret);
				}
				else
				{
					timesReadOnly++;
				}

				//将委托方法保存到方法标记数组元素中
				methods[i] = new MethodToken(getMethod, setMethod);
			}

			//构建嵌套匿名静态类
			nested.CreateType();
		}

		private static CustomAttributeBuilder GetAnnotation(CustomAttributeData attribute)
		{
			var arguments = new object[attribute.ConstructorArguments.Count];

			if(arguments.Length > 0)
			{
				for(int i = 0; i < attribute.ConstructorArguments.Count; i++)
				{
					if(attribute.ConstructorArguments[i].Value == null)
						arguments[i] = null;
					else
					{
						if(Zongsoft.Common.TypeExtension.IsEnumerable(attribute.ConstructorArguments[i].Value.GetType()) &&
						   Zongsoft.Common.TypeExtension.GetElementType(attribute.ConstructorArguments[i].Value.GetType()) == typeof(CustomAttributeTypedArgument))
						{
							var args = new List<object>();

							foreach(CustomAttributeTypedArgument arg in (System.Collections.IEnumerable)attribute.ConstructorArguments[i].Value)
							{
								args.Add(arg.Value);
							}

							arguments[i] = args.ToArray();
						}
						else
							arguments[i] = attribute.ConstructorArguments[i].Value;
					}
				}
			}

			if(attribute.NamedArguments.Count == 0)
				return new CustomAttributeBuilder(attribute.Constructor, arguments);

			var properties = attribute.NamedArguments.Where(p => !p.IsField).ToArray();
			var fields = attribute.NamedArguments.Where(p => p.IsField).ToArray();

			return new CustomAttributeBuilder(attribute.Constructor, arguments,
			                                  properties.Select(p => (PropertyInfo)p.MemberInfo).ToArray(),
			                                  properties.Select(p => p.TypedValue.Value).ToArray(),
			                                  fields.Select(p => (FieldInfo)p.MemberInfo).ToArray(),
			                                  fields.Select(p => p.TypedValue.Value).ToArray());
		}

		private static void GenerateAnnotations(TypeBuilder builder, ISet<Type> types)
		{
			foreach(var type in builder.ImplementedInterfaces)
			{
				var attributes = type.GetCustomAttributesData();

				//设置接口的自定义标签
				if(attributes != null && attributes.Count > 0)
				{
					foreach(var attribute in attributes)
					{
						var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(attribute.AttributeType, typeof(AttributeUsageAttribute));

						if(usage != null && !usage.AllowMultiple && types.Contains(attribute.AttributeType))
							continue;

						var annotation = GetAnnotation(attribute);

						if(annotation != null)
						{
							builder.SetCustomAttribute(annotation);
							types.Add(attribute.AttributeType);
						}
					}
				}
			}
		}

		private static void GeneratePropertyValueChangeChecker(ILGenerator generator, PropertyBuilder property, FieldBuilder field, Label exit)
		{
			if(property.PropertyType.IsPrimitive)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, field);
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Beq_S, exit);
			}
			else
			{
				var equality = property.PropertyType.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static) ??
				               typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static);

				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, field);

				if(property.PropertyType.IsValueType && equality.Name == "Equals")
					generator.Emit(OpCodes.Box, property.PropertyType);

				generator.Emit(OpCodes.Ldarg_1);

				if(property.PropertyType.IsValueType && equality.Name == "Equals")
					generator.Emit(OpCodes.Box, property.PropertyType);

				generator.Emit(OpCodes.Call, equality);
				generator.Emit(OpCodes.Brtrue_S, exit);
			}
		}

		private static void GenerateTypeInitializer(TypeBuilder builder, IList<PropertyMetadata> properties, MethodToken[] methods, out FieldBuilder names, out FieldBuilder tokens)
		{
			names = builder.DefineField(PROPERTY_NAMES_VARIABLE, typeof(string[]), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
			tokens = builder.DefineField(PROPERTY_TOKENS_VARIABLE, typeof(Dictionary<,>).MakeGenericType(typeof(string), PROPERTY_TOKEN_TYPE), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
			var entityType = _module.GetType(builder.UnderlyingSystemType.FullName);

			//定义只读属性的递增数量
			var timesReadOnly = 0;
			//定义可写属性的总数量
			var countWritable = properties.Count(p => p.CanWrite);

			var generator = builder.DefineTypeInitializer().GetILGenerator();

			generator.Emit(OpCodes.Ldc_I4, countWritable);
			generator.Emit(OpCodes.Newarr, typeof(string));

			for(int i = 0; i < properties.Count; i++)
			{
				//忽略只读属性
				if(!properties[i].CanWrite)
				{
					timesReadOnly++;
					continue;
				}

				generator.Emit(OpCodes.Dup);
				generator.Emit(OpCodes.Ldc_I4, i - timesReadOnly);
				generator.Emit(OpCodes.Ldstr, properties[i].Name);
				generator.Emit(OpCodes.Stelem_Ref);
			}

			generator.Emit(OpCodes.Stsfld, names);

			//重置只读属性的递增量
			timesReadOnly = 0;

			var DictionaryAddMethod = tokens.FieldType.GetMethod("Add");
			generator.Emit(OpCodes.Newobj, tokens.FieldType.GetConstructor(Type.EmptyTypes));

			for(int i = 0; i < properties.Count; i++)
			{
				generator.Emit(OpCodes.Dup);
				generator.Emit(OpCodes.Ldstr, properties[i].Name);
				generator.Emit(OpCodes.Ldc_I4, methods[i].SetMethod == null ? -1 : i - timesReadOnly);

				generator.Emit(OpCodes.Ldnull);
				if(methods[i].GetMethod != null)
				{
					generator.Emit(OpCodes.Ldftn, methods[i].GetMethod);
					generator.Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(typeof(object), typeof(object)).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
				}

				generator.Emit(OpCodes.Ldnull);
				if(methods[i].SetMethod != null)
				{
					generator.Emit(OpCodes.Ldftn, methods[i].SetMethod);
					generator.Emit(OpCodes.Newobj, typeof(Action<,>).MakeGenericType(typeof(object), typeof(object)).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
				}
				else
				{
					timesReadOnly++;
				}

				generator.Emit(OpCodes.Newobj, PROPERTY_TOKEN_CONSTRUCTOR);
				//generator.Emit(OpCodes.Newobj, tokenType.GetConstructor(new Type[] { typeof(int), typeof(Func<object, object>), typeof(Action<object, object>) }));
				generator.Emit(OpCodes.Call, DictionaryAddMethod);
			}

			generator.Emit(OpCodes.Stsfld, tokens);

			generator.Emit(OpCodes.Ret);
		}

		private static ConstructorBuilder GenerateConstructor(TypeBuilder builder, int count, FieldBuilder mask, IEnumerable<PropertyMetadata> properties)
		{
			var constructor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, null);
			var generator = constructor.GetILGenerator();

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));

			if(count > 64)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldc_I4, (int)Math.Ceiling(count / 8.0));
				generator.Emit(OpCodes.Newarr, typeof(byte));
				generator.Emit(OpCodes.Stfld, mask);
			}

			//初始化必要的属性
			GenerateInitializer(generator, properties);

			generator.Emit(OpCodes.Ret);

			return constructor;
		}

		private static void GenerateInitializer(ILGenerator generator, IEnumerable<PropertyMetadata> properties)
		{
			object value;
			Type valueType;

			foreach(var property in properties)
			{
				if(property.Synchrolock != null)
				{
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Newobj, typeof(object).GetConstructor(Type.EmptyTypes));
					generator.Emit(OpCodes.Stfld, property.Synchrolock);
				}

				if(property.HasDefaultValue)
				{
					valueType = property.DefaultValueAttribute.Value as Type;

					if(property.CanWrite)
					{
						if(valueType != null && property.PropertyType != typeof(Type))
						{
							if(!valueType.IsClass || valueType.IsAbstract)
								throw new InvalidOperationException($"The specified '{valueType.FullName}' type must be a non-abstract class when generate a default value via DefaultValueAttribute of the '{property.Name}' property.");

							if(!property.Field.FieldType.IsAssignableFrom(valueType))
								throw new InvalidOperationException($"The specified '{valueType}' default value type cannot be converted to the '{property.Field.FieldType}' type of property.");

							generator.Emit(OpCodes.Ldarg_0);
							generator.Emit(OpCodes.Newobj, valueType.GetConstructor(Type.EmptyTypes));
							generator.Emit(OpCodes.Call, property.Builder.SetMethod);
						}
						else
						{
							if(!Common.Convert.TryConvertValue(property.DefaultValueAttribute.Value, property.PropertyType, out value))
								throw new InvalidOperationException($"The '{property.DefaultValueAttribute.Value}' default value cannot be converted to the '{property.PropertyType}' type of property.");

							generator.Emit(OpCodes.Ldarg_0);
							LoadDefaultValue(generator, property.PropertyType, value);
							generator.Emit(OpCodes.Call, property.Builder.SetMethod);
						}
					}
					else if(property.Field != null)
					{
						if(valueType != null && property.Field.FieldType != typeof(Type))
						{
							if(property.SingletonFactoryEnabled)
							{
								var factory = property.GetSingletonFactory();

								//$Field=FactoryClass.GetProperty(...);
								generator.Emit(OpCodes.Ldarg_0);
								if(factory.GetParameters().Length > 0)
									generator.Emit(OpCodes.Ldnull);
								generator.Emit(OpCodes.Call, factory);
								generator.Emit(OpCodes.Stfld, property.Field);
							}
							else
							{
								if(!valueType.IsClass || valueType.IsAbstract)
									throw new InvalidOperationException($"The specified '{valueType.FullName}' type must be a non-abstract class when generate a default value via DefaultValueAttribute of the '{property.Name}' property.");

								if(!property.Field.FieldType.IsAssignableFrom(valueType))
									throw new InvalidOperationException($"The specified '{valueType}' default value type cannot be converted to the '{property.Field.FieldType}' type of field.");

								generator.Emit(OpCodes.Ldarg_0);
								generator.Emit(OpCodes.Newobj, valueType.GetConstructor(Type.EmptyTypes));
								generator.Emit(OpCodes.Stfld, property.Field);
							}
						}
						else
						{
							if(!Common.Convert.TryConvertValue(property.DefaultValueAttribute.Value, property.Field.FieldType, out value))
								throw new InvalidOperationException($"The '{property.DefaultValueAttribute.Value}' default value cannot be converted to the '{property.Field.FieldType}' type of field.");

							generator.Emit(OpCodes.Ldarg_0);
							LoadDefaultValue(generator, property.Field.FieldType, value);
							generator.Emit(OpCodes.Stfld, property.Field);
						}
					}
				}
			}
		}

		private static void GenerateHasChangesMethod(TypeBuilder builder, FieldBuilder mask, FieldBuilder names, FieldBuilder tokens)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IEntity).FullName + ".HasChanges",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(bool),
				new Type[] { typeof(string[]) });

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IEntity).GetMethod("HasChanges"));

			//定义方法参数
			method.DefineParameter(1, ParameterAttributes.None, "names");

			//获取代码生成器
			var generator = method.GetILGenerator();

			generator.DeclareLocal(PROPERTY_TOKEN_TYPE);
			generator.DeclareLocal(typeof(int));

			var EXIT_LABEL = generator.DefineLabel();
			var MASKING_LABEL = generator.DefineLabel();
			var LOOP_INITIATE_LABEL = generator.DefineLabel();
			var LOOP_INCREASE_LABEL = generator.DefineLabel();
			var LOOP_BODY_LABEL = generator.DefineLabel();
			var LOOP_TEST_LABEL = generator.DefineLabel();

			// if(names==null || ...)
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Brfalse_S, MASKING_LABEL);

			// if(... || names.Length== 0)
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ceq);
			generator.Emit(OpCodes.Brfalse_S, LOOP_INITIATE_LABEL);

			generator.MarkLabel(MASKING_LABEL);

			if(mask.FieldType.IsArray)
			{
				var INNER_LOOP_INCREASE_LABEL = generator.DefineLabel();
				var INNER_LOOP_BODY_LABEL = generator.DefineLabel();
				var INNER_LOOP_TEST_LABEL = generator.DefineLabel();

				// for(int i=0; ...; ...)
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Stloc_1, 0);
				generator.Emit(OpCodes.Br_S, INNER_LOOP_TEST_LABEL);

				generator.MarkLabel(INNER_LOOP_BODY_LABEL);

				// if(this.$MASK$[i] != 0)
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldloc_1);
				generator.Emit(OpCodes.Ldelem_U1);
				generator.Emit(OpCodes.Brfalse_S, INNER_LOOP_INCREASE_LABEL);

				// return true;
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.Ret);

				generator.MarkLabel(INNER_LOOP_INCREASE_LABEL);

				// for(...; ...; i++)
				generator.Emit(OpCodes.Ldloc_1);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.Add);
				generator.Emit(OpCodes.Stloc_1);

				generator.MarkLabel(INNER_LOOP_TEST_LABEL);

				// for(...; i<this.$MASK$.Length; ...)
				generator.Emit(OpCodes.Ldloc_1);
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldlen);
				generator.Emit(OpCodes.Conv_I4);
				generator.Emit(OpCodes.Blt_S, INNER_LOOP_BODY_LABEL);

				// return false;
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Ret);
			}
			else
			{
				// return $MASK != 0;
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldc_I4_0);
				if(mask.FieldType == typeof(ulong))
					generator.Emit(OpCodes.Conv_I8);
				generator.Emit(OpCodes.Cgt_Un);
				generator.Emit(OpCodes.Ret);
			}

			generator.MarkLabel(LOOP_INITIATE_LABEL);

			// for(int i=0; ...; ...)
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Stloc_1, 0);
			generator.Emit(OpCodes.Br_S, LOOP_TEST_LABEL);

			generator.MarkLabel(LOOP_BODY_LABEL);

			// if($$PROPERTIES$$.TryGetValue(names[i], out property) && ...)
			generator.Emit(OpCodes.Ldsfld, tokens);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldelem_Ref);
			generator.Emit(OpCodes.Ldloca_S, 0);
			generator.Emit(OpCodes.Call, tokens.FieldType.GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance));
			generator.Emit(OpCodes.Brfalse_S, LOOP_INCREASE_LABEL);

			// if(... && property.Setter != null && ...)
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_SETTER_FIELD);
			generator.Emit(OpCodes.Brfalse_S, LOOP_INCREASE_LABEL);

			if(mask.FieldType.IsArray)
			{
				// if(... && (this.$MASK$[property.Ordinal / 8] >> (property.Ordinal % 8) & 1) == 1)
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_ORDINAL_FIELD);
				generator.Emit(OpCodes.Ldc_I4_8);
				generator.Emit(OpCodes.Div);
				generator.Emit(OpCodes.Ldelem_U1);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_ORDINAL_FIELD);
				generator.Emit(OpCodes.Ldc_I4_8);
				generator.Emit(OpCodes.Rem);
				generator.Emit(OpCodes.Shr);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.And);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.Bne_Un_S, LOOP_INCREASE_LABEL);
			}
			else
			{
				// if(... && (this.$MASK$ >> property.Ordinal & 1) == 1)
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_ORDINAL_FIELD);
				generator.Emit(OpCodes.Shr_Un);
				generator.Emit(OpCodes.Ldc_I4_1);
				if(mask.FieldType == typeof(ulong))
					generator.Emit(OpCodes.Conv_I8);
				generator.Emit(OpCodes.And);
				generator.Emit(OpCodes.Ldc_I4_1);
				if(mask.FieldType == typeof(ulong))
					generator.Emit(OpCodes.Conv_I8);
				generator.Emit(OpCodes.Ceq);
				generator.Emit(OpCodes.Brfalse_S, LOOP_INCREASE_LABEL);
			}

			// return true;
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Ret);

			generator.MarkLabel(LOOP_INCREASE_LABEL);

			// for(...; ...; i++)
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Add);
			generator.Emit(OpCodes.Stloc_1);

			generator.MarkLabel(LOOP_TEST_LABEL);

			// for(...; i<names.Length; ...)
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Conv_I4);
			generator.Emit(OpCodes.Blt_S, LOOP_BODY_LABEL);

			generator.MarkLabel(EXIT_LABEL);

			// return false;
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ret);
		}

		private static void GenerateGetChangesMethod(TypeBuilder builder, FieldBuilder mask, FieldBuilder names, FieldBuilder tokens)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IEntity).FullName + ".GetChanges",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(IDictionary<string, object>),
				Type.EmptyTypes);

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IEntity).GetMethod("GetChanges"));

			//获取代码生成器
			var generator = method.GetILGenerator();

			generator.DeclareLocal(typeof(Dictionary<string, object>));
			generator.DeclareLocal(typeof(int));

			var EXIT_LABEL = generator.DefineLabel();
			var BEGIN_LABEL = generator.DefineLabel();
			var LOOP_INITIATE_LABEL = generator.DefineLabel();
			var LOOP_INCREASE_LABEL = generator.DefineLabel();
			var LOOP_BODY_LABEL = generator.DefineLabel();
			var LOOP_TEST_LABEL = generator.DefineLabel();

			if(!mask.FieldType.IsArray)
			{
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Brtrue_S, BEGIN_LABEL);

				generator.Emit(OpCodes.Ldnull);
				generator.Emit(OpCodes.Ret);
			}

			generator.MarkLabel(BEGIN_LABEL);

			// var dictioanry = new Dictionary<string, object>($$NAMES$$.Length);
			generator.Emit(OpCodes.Ldsfld, names);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Conv_I4);
			generator.Emit(OpCodes.Newobj, typeof(Dictionary<string, object>).GetConstructor(new Type[] { typeof(int) }));
			generator.Emit(OpCodes.Stloc_0);

			generator.MarkLabel(LOOP_INITIATE_LABEL);

			// for(int i=0; ...; ...)
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Stloc_1);
			generator.Emit(OpCodes.Br_S, LOOP_TEST_LABEL);

			generator.MarkLabel(LOOP_BODY_LABEL);

			if(mask.FieldType.IsArray)
			{
				// if((this.$MASK$[i / 8] >> (i % 8) & 1) == 1)
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldloc_1);
				generator.Emit(OpCodes.Ldc_I4_8);
				generator.Emit(OpCodes.Div);
				generator.Emit(OpCodes.Ldelem_U1);
				generator.Emit(OpCodes.Ldloc_1);
				generator.Emit(OpCodes.Ldc_I4_8);
				generator.Emit(OpCodes.Rem);
				generator.Emit(OpCodes.Shr);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.And);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.Bne_Un_S, LOOP_INCREASE_LABEL);
			}
			else
			{
				// if((this.$MASK$ >> i) & i == 1)
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldloc_1);
				generator.Emit(OpCodes.Shr_Un);
				generator.Emit(OpCodes.Ldc_I4_1);
				if(mask.FieldType == typeof(ulong))
					generator.Emit(OpCodes.Conv_I8);
				generator.Emit(OpCodes.And);
				generator.Emit(OpCodes.Ldc_I4_1);
				if(mask.FieldType == typeof(ulong))
					generator.Emit(OpCodes.Conv_I8);
				generator.Emit(OpCodes.Bne_Un_S, LOOP_INCREASE_LABEL);
			}

			// dictioanry[$$NAMES[i]]
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldsfld, names);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldelem_Ref);

			// $$PROPERTIES$$[$$NAMES$$[i]]
			generator.Emit(OpCodes.Ldsfld, tokens);
			generator.Emit(OpCodes.Ldsfld, names);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldelem_Ref);
			generator.Emit(OpCodes.Call, tokens.FieldType.GetProperty("Item", new Type[] { typeof(string) }).GetMethod);

			generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_GETTER_FIELD);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Callvirt, PROPERTY_TOKEN_GETTER_FIELD.FieldType.GetMethod("Invoke"));
			generator.Emit(OpCodes.Callvirt, typeof(Dictionary<string, object>).GetProperty("Item", new Type[] { typeof(string) }).SetMethod);

			generator.MarkLabel(LOOP_INCREASE_LABEL);

			// for(...; ...; i++)
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Add);
			generator.Emit(OpCodes.Stloc_1);

			generator.MarkLabel(LOOP_TEST_LABEL);

			// for(...; i<$NAMES$.Length; ...)
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldsfld, names);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Conv_I4);
			generator.Emit(OpCodes.Blt_S, LOOP_BODY_LABEL);

			generator.MarkLabel(EXIT_LABEL);

			if(mask.FieldType.IsArray)
			{
				var RETURN_NULL_LABEL = generator.DefineLabel();

				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Callvirt, typeof(Dictionary<string, object>).GetProperty("Count").GetMethod);
				generator.Emit(OpCodes.Brfalse_S, RETURN_NULL_LABEL);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ret);

				generator.MarkLabel(RETURN_NULL_LABEL);

				generator.Emit(OpCodes.Ldnull);
				generator.Emit(OpCodes.Ret);
			}
			else
			{
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ret);
			}
		}

		private static void GenerateTryGetValueMethod(TypeBuilder builder, FieldBuilder mask, FieldBuilder tokens)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IEntity).FullName + ".TryGetValue",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(bool),
				new Type[] { typeof(string), typeof(object).MakeByRefType() });

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IEntity).GetMethod("TryGetValue"));

			//定义方法参数
			method.DefineParameter(1, ParameterAttributes.None, "name");
			method.DefineParameter(2, ParameterAttributes.Out, "value");

			//获取代码生成器
			var generator = method.GetILGenerator();

			//声明本地变量
			generator.DeclareLocal(PROPERTY_TOKEN_TYPE);

			//定义代码标签
			var EXIT_LABEL = generator.DefineLabel();
			var GETBODY_LABEL = generator.DefineLabel();

			// value=null;
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Ldnull);
			generator.Emit(OpCodes.Stind_Ref);

			// if($$PROPERTIES.TryGet(name, out var property) && ...)
			generator.Emit(OpCodes.Ldsfld, tokens);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloca_S, 0);
			generator.Emit(OpCodes.Call, tokens.FieldType.GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance));
			generator.Emit(OpCodes.Brfalse_S, EXIT_LABEL);

			// if(... && (property.Ordinal<0 || ...))
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_ORDINAL_FIELD);
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Blt_S, GETBODY_LABEL);

			if(mask.FieldType.IsArray)
			{
				// if(... && (... || (this.$MASK$[property.Ordinal / 8] >> (property.Ordinal % 8) & 1) == 1))
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_ORDINAL_FIELD);
				generator.Emit(OpCodes.Ldc_I4_8);
				generator.Emit(OpCodes.Div);
				generator.Emit(OpCodes.Ldelem_U1);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_ORDINAL_FIELD);
				generator.Emit(OpCodes.Ldc_I4_8);
				generator.Emit(OpCodes.Rem);
				generator.Emit(OpCodes.Shr);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.And);
				generator.Emit(OpCodes.Ldc_I4_1);
				generator.Emit(OpCodes.Bne_Un_S, EXIT_LABEL);
			}
			else
			{
				// if(... && (... || ((this.$MASK$ >> property.Ordinal) & 1) == 1))
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);
				generator.Emit(OpCodes.Ldloc_0);
				generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_ORDINAL_FIELD);
				generator.Emit(OpCodes.Shr_Un);
				generator.Emit(OpCodes.Ldc_I4_1);
				if(mask.FieldType == typeof(ulong))
					generator.Emit(OpCodes.Conv_I8);
				generator.Emit(OpCodes.And);
				generator.Emit(OpCodes.Ldc_I4_1);
				if(mask.FieldType == typeof(ulong))
					generator.Emit(OpCodes.Conv_I8);
				generator.Emit(OpCodes.Bne_Un_S, EXIT_LABEL);
			}

			generator.MarkLabel(GETBODY_LABEL);

			// value = property.Getter(this);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_GETTER_FIELD);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Call, PROPERTY_TOKEN_GETTER_FIELD.FieldType.GetMethod("Invoke"));
			generator.Emit(OpCodes.Stind_Ref);

			// return true;
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Ret);

			generator.MarkLabel(EXIT_LABEL);

			//return false;
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ret);
		}

		private static void GenerateTrySetValueMethod(TypeBuilder builder, FieldBuilder tokens)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IEntity).FullName + ".TrySetValue",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(bool),
				new Type[] { typeof(string), typeof(object) });

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IEntity).GetMethod("TrySetValue"));

			//定义方法参数
			method.DefineParameter(1, ParameterAttributes.None, "name");
			method.DefineParameter(2, ParameterAttributes.None, "value");

			//获取代码生成器
			var generator = method.GetILGenerator();

			//声明本地变量
			generator.DeclareLocal(PROPERTY_TOKEN_TYPE);

			//定义代码标签
			var EXIT_LABEL = generator.DefineLabel();

			// if($$PROPERTIES$$.TryGetValue(name, out var property))
			generator.Emit(OpCodes.Ldsfld, tokens);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloca_S, 0);
			generator.Emit(OpCodes.Call, tokens.FieldType.GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance));
			generator.Emit(OpCodes.Brfalse_S, EXIT_LABEL);

			// if(... && property.Setter != null)
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_SETTER_FIELD);
			generator.Emit(OpCodes.Brfalse_S, EXIT_LABEL);

			// property.Setter(this, value);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldfld, PROPERTY_TOKEN_SETTER_FIELD);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Call, PROPERTY_TOKEN_SETTER_FIELD.FieldType.GetMethod("Invoke"));

			// return true;
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Ret);

			generator.MarkLabel(EXIT_LABEL);

			//return false;
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ret);
		}

		private static void GeneratePropertyTokenClass()
		{
			var builder = _module.DefineType("<PropertyToken>", TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout, typeof(ValueType));

			PROPERTY_TOKEN_ORDINAL_FIELD = builder.DefineField("Ordinal", typeof(int), FieldAttributes.Public | FieldAttributes.InitOnly);
			//PROPERTY_TOKEN_GETTER_FIELD = builder.DefineField("Getter", typeof(Func<,>).MakeGenericType(type, typeof(object)), FieldAttributes.Public | FieldAttributes.InitOnly);
			//PROPERTY_TOKEN_SETTER_FIELD = builder.DefineField("Setter", typeof(Action<,>).MakeGenericType(type, typeof(object)), FieldAttributes.Public | FieldAttributes.InitOnly);
			PROPERTY_TOKEN_GETTER_FIELD = builder.DefineField("Getter", typeof(Func<object,object>), FieldAttributes.Public | FieldAttributes.InitOnly);
			PROPERTY_TOKEN_SETTER_FIELD = builder.DefineField("Setter", typeof(Action<object,object>), FieldAttributes.Public | FieldAttributes.InitOnly);

			PROPERTY_TOKEN_CONSTRUCTOR = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(int), PROPERTY_TOKEN_GETTER_FIELD.FieldType, PROPERTY_TOKEN_SETTER_FIELD.FieldType });
			PROPERTY_TOKEN_CONSTRUCTOR.DefineParameter(1, ParameterAttributes.None, "ordinal");
			PROPERTY_TOKEN_CONSTRUCTOR.DefineParameter(2, ParameterAttributes.None, "getter");
			PROPERTY_TOKEN_CONSTRUCTOR.DefineParameter(3, ParameterAttributes.None, "setter");

			var generator = PROPERTY_TOKEN_CONSTRUCTOR.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, PROPERTY_TOKEN_ORDINAL_FIELD);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Stfld, PROPERTY_TOKEN_GETTER_FIELD);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_3);
			generator.Emit(OpCodes.Stfld, PROPERTY_TOKEN_SETTER_FIELD);
			generator.Emit(OpCodes.Ret);

			PROPERTY_TOKEN_TYPE = builder.CreateType();
		}

		private static FieldBuilder GeneratePropertyChangedEvent(TypeBuilder builder)
		{
			var exchangeMethod = typeof(Interlocked)
				.GetMethods(BindingFlags.Public | BindingFlags.Static)
				.First(p => p.Name == "CompareExchange" && p.IsGenericMethod)
				.MakeGenericMethod(typeof(PropertyChangedEventHandler));

			//添加类型的实现接口声明
			if(!builder.ImplementedInterfaces.Contains(typeof(INotifyPropertyChanged)))
				builder.AddInterfaceImplementation(typeof(INotifyPropertyChanged));

			//定义“PropertyChanged”事件的委托链字段
			var field = builder.DefineField("PropertyChanged", typeof(PropertyChangedEventHandler), FieldAttributes.Private);

			//定义“PropertyChanged”事件
			var e = builder.DefineEvent("PropertyChanged", EventAttributes.None, typeof(PropertyChangedEventHandler));

			//定义事件的Add方法
			var add = builder.DefineMethod("add_PropertyChanged", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot, null, new Type[] { typeof(PropertyChangedEventHandler) });
			//定义事件方法的参数
			add.DefineParameter(1, ParameterAttributes.None, "value");

			var generator = add.GetILGenerator();
			generator.DeclareLocal(typeof(PropertyChangedEventHandler)); //original
			generator.DeclareLocal(typeof(PropertyChangedEventHandler)); //current
			generator.DeclareLocal(typeof(PropertyChangedEventHandler)); //latest

			var ADD_LOOP_LABEL = generator.DefineLabel();

			// var original = this.PropertyChanged;
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, field);
			generator.Emit(OpCodes.Stloc_0);

			// do{}
			generator.MarkLabel(ADD_LOOP_LABEL);

			// current=original
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Stloc_1);

			// var latest=Delegate.Combine(current, value);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Call, typeof(Delegate).GetMethod("Combine", new Type[] { typeof(Delegate), typeof(Delegate) }));
			generator.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
			generator.Emit(OpCodes.Stloc_2);

			// original = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, latest, current);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldflda, field);
			generator.Emit(OpCodes.Ldloc_2);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Call, exchangeMethod);
			generator.Emit(OpCodes.Stloc_0);

			// while(original != current);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Bne_Un_S, ADD_LOOP_LABEL);

			generator.Emit(OpCodes.Ret);

			//设置事件的Add方法
			e.SetAddOnMethod(add);

			//定义事件的Remove方法
			var remove = builder.DefineMethod("remove_PropertyChanged", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot, null, new Type[] { typeof(PropertyChangedEventHandler) });
			//定义事件方法的参数
			remove.DefineParameter(1, ParameterAttributes.None, "value");

			generator = remove.GetILGenerator();
			generator.DeclareLocal(typeof(PropertyChangedEventHandler)); //original
			generator.DeclareLocal(typeof(PropertyChangedEventHandler)); //current
			generator.DeclareLocal(typeof(PropertyChangedEventHandler)); //latest

			var REMOVE_LOOP_LABEL = generator.DefineLabel();

			// var original = this.PropertyChanged;
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, field);
			generator.Emit(OpCodes.Stloc_0);

			// do{}
			generator.MarkLabel(REMOVE_LOOP_LABEL);

			// current=original
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Stloc_1);

			// var latest=Delegate.Remove(current, value);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Call, typeof(Delegate).GetMethod("Remove", new Type[] { typeof(Delegate), typeof(Delegate) }));
			generator.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
			generator.Emit(OpCodes.Stloc_2);

			// original = Interlocked.CompareExchange<PropertyChangedEventHandler>(ref this.PropertyChanged, latest, current);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldflda, field);
			generator.Emit(OpCodes.Ldloc_2);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Call, exchangeMethod);
			generator.Emit(OpCodes.Stloc_0);

			// while(original != current);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Bne_Un_S, REMOVE_LOOP_LABEL);

			generator.Emit(OpCodes.Ret);

			//设置事件的Remove方法
			e.SetRemoveOnMethod(remove);

			return field;
		}

		private static IList<PropertyMetadata> MakeInterfaces(Type type, TypeBuilder builder, out FieldBuilder propertyChanged)
		{
			propertyChanged = null;
			var queue = new Queue<Type>(type.GetInterfaces());
			var result = new List<PropertyMetadata>();

			//首先将当前类型的属性信息加入到结果集中
			result.AddRange(type.GetProperties().Select(p => new PropertyMetadata(p)));

			while(queue.Count > 0)
			{
				var interfaceType = queue.Dequeue();

				//如果该接口已经被声明，则跳过它
				if(builder.ImplementedInterfaces.Contains(interfaceType))
					continue;

				//将指定类型继承的接口加入到实现接口声明中
				builder.AddInterfaceImplementation(interfaceType);

				if(interfaceType == typeof(INotifyPropertyChanged))
				{
					if(propertyChanged == null)
						propertyChanged = GeneratePropertyChangedEvent(builder); //生成“INotifyPropertyChanged”接口实现
				}
				else
				{
					result.AddRange(interfaceType.GetProperties().Select(p => new PropertyMetadata(p)));

					//获取当前接口的父接口
					var baseInterfaces = interfaceType.GetInterfaces();

					if(baseInterfaces != null && baseInterfaces.Length > 0)
					{
						foreach(var baseInterface in baseInterfaces)
						{
							queue.Enqueue(baseInterface);
						}
					}
				}
			}

			return result;
		}

		private static void LoadDefaultValue(ILGenerator generator, Type type, object value = null)
		{
			if(type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if(value == null)
					generator.Emit(OpCodes.Ldnull);
				else
					LoadDefaultValue(generator, Nullable.GetUnderlyingType(type), value);

				return;
			}

			if(type.IsEnum)
			{
				LoadDefaultValue(generator, Enum.GetUnderlyingType(type), Convert.ChangeType(value, Enum.GetUnderlyingType(type)));
				return;
			}

			switch(Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					if(value != null && (bool)Convert.ChangeType(value, typeof(bool)))
						generator.Emit(OpCodes.Ldc_I4_1);
					else
						generator.Emit(OpCodes.Ldc_I4_0);
					return;
				case TypeCode.Byte:
					if(value == null || (byte)Convert.ChangeType(value, TypeCode.Byte) == 0)
						generator.Emit(OpCodes.Ldc_I4_0);
					else
						generator.Emit(OpCodes.Ldc_I4, (byte)Convert.ChangeType(value, TypeCode.Byte));

					generator.Emit(OpCodes.Conv_U1);
					return;
				case TypeCode.SByte:
					if(value == null || (sbyte)Convert.ChangeType(value, TypeCode.SByte) == 0)
						generator.Emit(OpCodes.Ldc_I4_0);
					else
						generator.Emit(OpCodes.Ldc_I4, (sbyte)Convert.ChangeType(value, TypeCode.SByte));

					generator.Emit(OpCodes.Conv_I1);
					return;
				case TypeCode.Single:
					generator.Emit(OpCodes.Ldc_R4, value == null ? 0 : (float)Convert.ChangeType(value, TypeCode.Single));
					return;
				case TypeCode.Double:
					generator.Emit(OpCodes.Ldc_R8, value == null ? 0 : (double)Convert.ChangeType(value, TypeCode.Double));
					return;
				case TypeCode.Int16:
					if(value == null || (short)Convert.ChangeType(value, TypeCode.Int16) == 0)
						generator.Emit(OpCodes.Ldc_I4_0);
					else
						generator.Emit(OpCodes.Ldc_I4, (short)Convert.ChangeType(value, TypeCode.Int16));

					generator.Emit(OpCodes.Conv_I2);
					return;
				case TypeCode.Int32:
					if(value == null || (int)Convert.ChangeType(value, TypeCode.Int32) == 0)
						generator.Emit(OpCodes.Ldc_I4_0);
					else
						generator.Emit(OpCodes.Ldc_I4, (int)Convert.ChangeType(value, TypeCode.Int32));
					return;
				case TypeCode.Int64:
					if(value == null || (long)Convert.ChangeType(value, TypeCode.Int64) == 0)
					{
						generator.Emit(OpCodes.Ldc_I4_0);
						generator.Emit(OpCodes.Conv_I8);
					}
					else
						generator.Emit(OpCodes.Ldc_I8, (long)Convert.ChangeType(value, TypeCode.Int64));

					return;
				case TypeCode.UInt16:
					if(value == null || (ushort)Convert.ChangeType(value, TypeCode.UInt16) == 0)
						generator.Emit(OpCodes.Ldc_I4_0);
					else
						generator.Emit(OpCodes.Ldc_I4, (ushort)Convert.ChangeType(value, TypeCode.UInt16));

					generator.Emit(OpCodes.Conv_U2);
					return;
				case TypeCode.UInt32:
					if(value == null || (uint)Convert.ChangeType(value, TypeCode.UInt32) == 0)
						generator.Emit(OpCodes.Ldc_I4_0);
					else
						generator.Emit(OpCodes.Ldc_I4, (uint)Convert.ChangeType(value, TypeCode.UInt32));

					generator.Emit(OpCodes.Conv_U4);
					return;
				case TypeCode.UInt64:
					if(value == null || (ulong)Convert.ChangeType(value, TypeCode.UInt64) == 0)
						generator.Emit(OpCodes.Ldc_I4_0);
					else
						generator.Emit(OpCodes.Ldc_I8, (ulong)Convert.ChangeType(value, TypeCode.UInt64));

					generator.Emit(OpCodes.Conv_U8);
					return;
				case TypeCode.String:
					if(value == null)
						generator.Emit(OpCodes.Ldnull);
					else
						generator.Emit(OpCodes.Ldstr, value.ToString());

					return;
				case TypeCode.Char:
					if(value == null || (char)Convert.ChangeType(value, TypeCode.Char) == '\0')
						generator.Emit(OpCodes.Ldsfld, typeof(Char).GetField("MinValue", BindingFlags.Public | BindingFlags.Static));
					else
						generator.Emit(OpCodes.Ldc_I4_S, (char)Convert.ChangeType(value, TypeCode.Char));
					return;
				case TypeCode.DateTime:
					if(value == null || (DateTime)Convert.ChangeType(value, TypeCode.DateTime) == DateTime.MinValue)
						generator.Emit(OpCodes.Ldsfld, typeof(DateTime).GetField("MinValue", BindingFlags.Public | BindingFlags.Static));
					else
					{
						generator.Emit(OpCodes.Ldc_I8, ((DateTime)Convert.ChangeType(value, TypeCode.DateTime)).Ticks);
						generator.Emit(OpCodes.Newobj, typeof(DateTime).GetConstructor(new Type[] { typeof(long) }));
					}

					return;
				case TypeCode.Decimal:
					if(value == null)
						generator.Emit(OpCodes.Ldsfld, typeof(Decimal).GetField("Zero", BindingFlags.Public | BindingFlags.Static));
					else
					{
						switch(Type.GetTypeCode(value.GetType()))
						{
							case TypeCode.Single:
								generator.Emit(OpCodes.Ldc_R4, (float)Convert.ChangeType(value, typeof(float)));
								generator.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(float) }));
								break;
							case TypeCode.Double:
								generator.Emit(OpCodes.Ldc_R8, (double)Convert.ChangeType(value, typeof(double)));
								generator.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(double) }));
								break;
							case TypeCode.Decimal:
								var bits = Decimal.GetBits((decimal)Convert.ChangeType(value, TypeCode.Decimal));
								generator.Emit(OpCodes.Ldc_I4_S, bits.Length);
								generator.Emit(OpCodes.Newarr, typeof(int));

								for(int i = 0; i < bits.Length; i++)
								{
									generator.Emit(OpCodes.Dup);
									generator.Emit(OpCodes.Ldc_I4_S, i);
									generator.Emit(OpCodes.Ldc_I4, bits[i]);
									generator.Emit(OpCodes.Stelem_I4);
								}

								generator.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(int[]) }));

								break;
							case TypeCode.SByte:
							case TypeCode.Int16:
							case TypeCode.Int32:
								generator.Emit(OpCodes.Ldc_I4, (int)Convert.ChangeType(value, typeof(int)));
								generator.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(int) }));
								break;
							case TypeCode.Int64:
								generator.Emit(OpCodes.Ldc_I8, (long)Convert.ChangeType(value, typeof(long)));
								generator.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(long) }));
								break;
							case TypeCode.Byte:
							case TypeCode.Char:
							case TypeCode.UInt16:
							case TypeCode.UInt32:
								generator.Emit(OpCodes.Ldc_I4, (int)Convert.ChangeType(value, typeof(int)));
								generator.Emit(OpCodes.Conv_U4);
								generator.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(uint) }));
								break;
							case TypeCode.UInt64:
								generator.Emit(OpCodes.Ldc_I8, (long)Convert.ChangeType(value, typeof(long)));
								generator.Emit(OpCodes.Conv_U8);
								generator.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[] { typeof(ulong) }));
								break;
							default:
								throw new InvalidOperationException($"Unable to convert '{value.GetType()}' type to decimal type.");
						}
					}

					return;
				case TypeCode.DBNull:
					generator.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value", BindingFlags.Public | BindingFlags.Static));
					return;
			}

			if(type.IsValueType)
				generator.Emit(OpCodes.Initobj, type);
			else
				generator.Emit(OpCodes.Ldnull);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static bool IsCollectionType(Type type)
		{
			if(type.IsPrimitive || type.IsValueType || type == typeof(string))
				return false;

			return Zongsoft.Common.TypeExtension.IsEnumerable(type);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static Type GetCollectionImplementationType(Type type)
		{
			if(type.IsClass && type.IsAbstract)
				throw new NotSupportedException($"The '{type}' type cannot be an abstract class and must be a deterministic class or interface.");

			if(type.IsValueType || type.IsClass)
				return type;

			if(type.IsGenericType)
			{
				var prototype = type.GetGenericTypeDefinition();

				if(prototype == typeof(ISet<>))
					return typeof(HashSet<>).MakeGenericType(type.GetGenericArguments());
				if(prototype == typeof(System.Collections.Specialized.INotifyCollectionChanged))
					return typeof(System.Collections.ObjectModel.ObservableCollection<>).MakeGenericType(type.GetGenericArguments());
				else if(prototype == typeof(IDictionary<,>))
					return typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
				else if(prototype == typeof(IReadOnlyDictionary<,>))
					return typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
				else if(prototype == typeof(IList<>) || prototype == typeof(ICollection<>))
					return typeof(List<>).MakeGenericType(type.GetGenericArguments());
				else if(prototype == typeof(IReadOnlyList<>) || prototype == typeof(IReadOnlyCollection<>))
					return typeof(List<>).MakeGenericType(type.GetGenericArguments());
				else if(prototype == typeof(IEnumerable<>))
					return typeof(List<>).MakeGenericType(type.GetGenericArguments());
			}
			else
			{
				if(type == typeof(IDictionary))
					return typeof(Dictionary<object, object>);
				else if(type == typeof(IList) || type == typeof(ICollection))
					return typeof(List<object>);
				else if(type == typeof(IEnumerable))
					return typeof(List<object>);
			}

			throw new InvalidOperationException($"The '{type}' type is not a specific or supported collection type.");
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static string GetClassName(Type type)
		{
			return (string.IsNullOrEmpty(type.Namespace) ? string.Empty : type.Namespace + ".") +
			       (type.Name.Length > 1 && type.Name[0] == 'I' && char.IsUpper(type.Name[1]) ? type.Name.Substring(1) : type.Name);
		}
		#endregion

		#region 嵌套子类
		/// <summary>
		/// 表示实体属性实现代码的生成方式。
		/// </summary>
		public enum PropertyImplementationMode
		{
			/// <summary>默认实现方式。</summary>
			Default,

			/// <summary>以类似扩展方法的方式实现属性。</summary>
			Extension,

			/// <summary>以单例模式的方式实现属性。</summary>
			Singleton,
		}

		/// <summary>
		/// 提供实体属性动态编译的自定义特性。
		/// </summary>
		[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Method)]
		public class PropertyAttribute : Attribute
		{
			#region 构造函数
			public PropertyAttribute()
			{
				this.Mode = PropertyImplementationMode.Default;
			}

			public PropertyAttribute(PropertyImplementationMode mode, Type type = null)
			{
				this.Mode = mode;
				this.Type = type;
			}
			#endregion

			#region 公共属性
			/// <summary>
			/// 获取扩展方法的静态类的类型或属性的具体类型，具体含义由<see cref="Mode"/>属性值确定。
			/// </summary>
			public Type Type
			{
				get;
			}

			/// <summary>
			/// 获取或设置实体属性代码的实现方式。
			/// </summary>
			public PropertyImplementationMode Mode
			{
				get;
				set;
			}

			/// <summary>
			/// 获取或设置属性是否以显式实现方式生成。
			/// </summary>
			public bool IsExplicitImplementation
			{
				get;
				set;
			}
			#endregion
		}

		private class PropertyMetadata
		{
			#region 私有变量
			private readonly PropertyInfo _property;
			private readonly PropertyAttribute _metadata;
			private readonly DefaultValueAttribute _defaultAttribute;
			#endregion

			#region 公共字段
			public PropertyBuilder Builder;
			public FieldBuilder Field;
			public FieldBuilder Synchrolock;
			public bool IsExplicitImplementation;
			#endregion

			#region 构造函数
			public PropertyMetadata(PropertyInfo property)
			{
				_property = property ?? throw new ArgumentNullException(nameof(property));
				_metadata = property.GetCustomAttribute<PropertyAttribute>(true);
				_defaultAttribute = _property.GetCustomAttribute<DefaultValueAttribute>(true);

				if(_metadata != null)
					this.IsExplicitImplementation = _metadata.IsExplicitImplementation;
			}
			#endregion

			#region 公共属性
			public bool IsReadOnly
			{
				get
				{
					return _property.CanRead && !_property.CanWrite;
				}
			}

			public bool CanRead
			{
				get
				{
					return _property.CanRead;
				}
			}

			public bool CanWrite
			{
				get
				{
					return _property.CanWrite;
				}
			}

			public string Name
			{
				get
				{
					return _property.Name;
				}
			}

			public Type DeclaringType
			{
				get
				{
					return _property.DeclaringType;
				}
			}

			public Type PropertyType
			{
				get
				{
					return _property.PropertyType;
				}
			}

			public MethodInfo GetMethod
			{
				get
				{
					return _property.GetMethod;
				}
			}

			public MethodInfo SetMethod
			{
				get
				{
					return _property.SetMethod;
				}
			}

			public PropertyAttribute Metadata
			{
				get
				{
					return _metadata;
				}
			}

			public bool SingletonFactoryEnabled
			{
				get
				{
					//当条件满足以下条件之一则返回真，否则返回假：
					//1. 属性有 DefaultValueAttribute 自定义标签，且它的 Value 是 Type，且指定的 Type 是一个静态类；
					//2. 属性有 PropertyAttribute 元数据标签，且实现方式是单例模式，且它的 Type 指定的是一个静态类。
					return (_defaultAttribute != null &&
					        _defaultAttribute.Value is Type type &&
					        type.IsAbstract && type.IsSealed) ||
					       (_metadata != null && _metadata.Mode == PropertyImplementationMode.Singleton &&
					        _metadata.Type != null && _metadata.Type.IsAbstract && _metadata.Type.IsSealed);
				}
			}

			public bool HasDefaultValue
			{
				get
				{
					return _defaultAttribute != null;
				}
			}

			public DefaultValueAttribute DefaultValueAttribute
			{
				get
				{
					return _defaultAttribute;
				}
			}
			#endregion

			#region 公共方法
			public string GetName()
			{
				return this.IsExplicitImplementation ?
				       _property.DeclaringType.FullName + "." + _property.Name :
				       _property.Name;
			}

			public string GetFieldName(string suffix = null)
			{
				return "$" + (this.IsExplicitImplementation ?
					   _property.DeclaringType.FullName + "." + _property.Name :
					   _property.Name) + suffix;
			}

			public string GetGetterName()
			{
				return this.IsExplicitImplementation ?
				       _property.DeclaringType.FullName + "." + _property.GetMethod.Name :
				       _property.GetMethod.Name;
			}

			public string GetSetterName()
			{
				return this.IsExplicitImplementation ?
				       _property.DeclaringType.FullName + "." + _property.SetMethod.Name :
				       _property.SetMethod.Name;
			}

			public MethodInfo GetSingletonFactory()
			{
				Type factoryType = null;

				if(_defaultAttribute != null && _defaultAttribute.Value is Type type && type.IsAbstract && type.IsSealed)
					factoryType = type;
				else if(_metadata != null && _metadata.Mode == PropertyImplementationMode.Singleton && _metadata.Type != null && _metadata.Type.IsAbstract && _metadata.Type.IsSealed)
					factoryType = _metadata.Type;

				if(factoryType == null)
					return null;

				var method = factoryType.GetMethod("Get" + _property.Name, BindingFlags.Public | BindingFlags.Static, null, new Type[] { _property.DeclaringType }, null) ??
				             factoryType.GetMethod("Get" + _property.Name, BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null);

				if(method == null)
					throw new InvalidOperationException($"Not found the 'Get{_property.Name}(...)' factory method in the '{factoryType}' extension class.");

				if(method.ReturnType == null || method.ReturnType == typeof(void) || !_property.PropertyType.IsAssignableFrom(method.ReturnType))
					throw new InvalidOperationException($"The return type of the 'Get{_property.Name}(...)' factory method in the '{factoryType}' extension class is missing or invliad.");

				return method;
			}

			public IList<CustomAttributeData> GetCustomAttributesData()
			{
				return _property.GetCustomAttributesData();
			}
			#endregion
		}

		private struct MethodToken
		{
			public MethodToken(MethodBuilder getMethod, MethodBuilder setMethod)
			{
				this.GetMethod = getMethod;
				this.SetMethod = setMethod;
			}

			public readonly MethodBuilder GetMethod;
			public readonly MethodBuilder SetMethod;
		}
		#endregion
	}
}
