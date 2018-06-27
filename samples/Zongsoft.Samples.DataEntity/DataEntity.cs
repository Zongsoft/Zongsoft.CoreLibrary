using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity
{
	public static class DataEntity
	{
		#region 常量定义
		private const string ASSEMBLY_NAME = "Zongsoft.Dynamics.Entities";

		private const string PROPERTY_NAMES_VARIABLE = "$NAMES$";
		private const string PROPERTY_TOKENS_VARIABLE = "$PROPERTIES$";
		private const string MASK_VARIABLE = "$MASK$";
		#endregion

		#region 成员字段
		private static readonly MethodInfo POW_METHOD = typeof(Math).GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
		private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
		private static readonly Dictionary<Type, Func<object>> _cache = new Dictionary<Type, Func<object>>();

		private static readonly AssemblyBuilder _assembly = AppDomain.CurrentDomain
			.DefineDynamicAssembly(new AssemblyName(ASSEMBLY_NAME), AssemblyBuilderAccess.RunAndSave);
		private static readonly ModuleBuilder _module = _assembly
			.DefineDynamicModule(ASSEMBLY_NAME, ASSEMBLY_NAME + ".dll");
		#endregion

		#region 公共方法
		public static T Build<T>() where T : Zongsoft.Data.IDataEntity
		{
			return (T)GetCreator<T>()();
		}

		public static IEnumerable<T> Build<T>(int count, Action<T, int> map = null) where T : Zongsoft.Data.IDataEntity
		{
			if(count < 1)
				throw new ArgumentOutOfRangeException(nameof(count));

			var creator = GetCreator<T>();

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
		#endregion

		#region 私有方法
		public static Func<object> GetCreator<T>()
		{
			var type = typeof(T);

			if(!type.IsInterface)
				throw new ArgumentException("The generic parameter type must be a interface.");

			_locker.EnterReadLock();
			var existed = _cache.TryGetValue(type, out var creator);
			_locker.ExitReadLock();

			if(existed)
				return creator;

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

		private static Func<object> Compile(Type type)
		{
			ILGenerator generator;

			//if(_module.GetType("PropertyToken") == null)
			//	GeneratePropertyToken();

			var builder = _module.DefineType(
				GetClassName(type) + "Ex",
				TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);

			//添加类型的实现接口声明
			builder.AddInterfaceImplementation(type);
			builder.AddInterfaceImplementation(typeof(Zongsoft.Data.IDataEntity));

			//获取指定接口的所有属性集
			var properties = type.GetProperties();

			//生成构造函数
			GenerateConstructor(builder, properties.Length, out var mask);

			//生成属性定义以及嵌套子类
			GenerateProperties(builder, mask, properties, out var methods);

			var constructor = GeneratePropertyToken(builder);

			//生成静态构造函数
			GenerateTypeInitializer(builder, properties, constructor, methods, out var names, out var tokens);

			//生成“HasChanges”方法
			GenerateHasChangesMethod(builder, mask, names, tokens);

			//生成“GetChanges”方法
			GenerateGetChangesMethod(builder);

			//生成“TryGet”方法
			GenerateTryGetMethod(builder);

			//生成“TrySet”方法
			GenerateTrySetMethod(builder);

			//构建类型
			type = builder.CreateType();

			_assembly.Save(ASSEMBLY_NAME + ".dll");

			//生成创建实例的动态方法
			var creator = new DynamicMethod("Create", typeof(object), Type.EmptyTypes);

			generator = creator.GetILGenerator();
			generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
			generator.Emit(OpCodes.Ret);

			//返回实例创建方法的委托
			return (Func<object>)creator.CreateDelegate(typeof(Func<object>));
		}

		private static void GenerateProperties(TypeBuilder builder, FieldBuilder mask, PropertyInfo[] properties, out MethodToken[] methods)
		{
			//生成嵌套匿名委托静态类
			var nested = builder.DefineNestedType("!Methods!", TypeAttributes.NestedPrivate | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);

			//创建返回参数（即方法标记）
			methods = new MethodToken[properties.Length];

			//生成属性定义
			for(int i = 0; i < properties.Length; i++)
			{
				var field = builder.DefineField("$" + properties[i].Name, properties[i].PropertyType, FieldAttributes.Private);
				var property = builder.DefineProperty(properties[i].Name, PropertyAttributes.None, properties[i].PropertyType, null);

				var getter = builder.DefineMethod("get_" + properties[i].Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot, properties[i].PropertyType, Type.EmptyTypes);
				var generator = getter.GetILGenerator();
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, field);
				generator.Emit(OpCodes.Ret);
				property.SetGetMethod(getter);

				var setter = builder.DefineMethod("set_" + properties[i].Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot, null, new Type[] { properties[i].PropertyType });
				setter.DefineParameter(1, ParameterAttributes.None, "value");
				generator = setter.GetILGenerator();
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldarg_1);
				generator.Emit(OpCodes.Stfld, field);
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, mask);

				if(properties.Length <= 8)
				{
					generator.Emit(OpCodes.Ldc_I4, (int)Math.Pow(2, i));
					generator.Emit(OpCodes.Or);
					generator.Emit(OpCodes.Conv_U1);
					generator.Emit(OpCodes.Stfld, mask);
				}
				else if(properties.Length <= 16)
				{
					generator.Emit(OpCodes.Ldc_I4, (int)Math.Pow(2, i));
					generator.Emit(OpCodes.Or);
					generator.Emit(OpCodes.Conv_U2);
					generator.Emit(OpCodes.Stfld, mask);
				}
				else if(properties.Length <= 32)
				{
					generator.Emit(OpCodes.Ldc_I4, (uint)Math.Pow(2, i));
					generator.Emit(OpCodes.Or);
					generator.Emit(OpCodes.Conv_U4);
					generator.Emit(OpCodes.Stfld, mask);
				}
				else if(properties.Length <= 64)
				{
					generator.Emit(OpCodes.Ldc_I8, (long)Math.Pow(2, i));
					generator.Emit(OpCodes.Or);
					generator.Emit(OpCodes.Conv_U8);
					generator.Emit(OpCodes.Stfld, mask);
				}
				else
				{
					generator.Emit(OpCodes.Ldc_I4, i / 8);
					generator.Emit(OpCodes.Ldelema, typeof(byte));
					generator.Emit(OpCodes.Dup);
					generator.Emit(OpCodes.Ldind_U1);
					generator.Emit(OpCodes.Ldc_R8, 2);
					generator.Emit(OpCodes.Ldc_R8, i % 8);
					generator.Emit(OpCodes.Call, POW_METHOD);
					generator.Emit(OpCodes.Conv_U1);
					generator.Emit(OpCodes.Or);
					generator.Emit(OpCodes.Conv_U1);
					generator.Emit(OpCodes.Stind_I1);
				}

				generator.Emit(OpCodes.Ret);
				property.SetSetMethod(setter);

				//生成获取属性字段的方法
				var getMethod = nested.DefineMethod("Get" + properties[i].Name, MethodAttributes.Assembly | MethodAttributes.Static, CallingConventions.Standard, typeof(object), new Type[] { field.DeclaringType });
				getMethod.DefineParameter(1, ParameterAttributes.None, "target");

				generator = getMethod.GetILGenerator();
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, field);
				if(properties[i].PropertyType.IsValueType)
					generator.Emit(OpCodes.Box, properties[i].PropertyType);
				generator.Emit(OpCodes.Ret);

				//生成设置属性的方法
				var setMethod = nested.DefineMethod("Set" + properties[i].Name, MethodAttributes.Assembly | MethodAttributes.Static, CallingConventions.Standard, null, new Type[] { field.DeclaringType, typeof(object) });
				setMethod.DefineParameter(1, ParameterAttributes.None, "target");
				setMethod.DefineParameter(2, ParameterAttributes.None, "value");

				generator = setMethod.GetILGenerator();
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldarg_1);
				if(properties[i].PropertyType.IsValueType)
					generator.Emit(OpCodes.Unbox_Any, properties[i].PropertyType);
				else
					generator.Emit(OpCodes.Castclass, properties[i].PropertyType);
				generator.Emit(OpCodes.Call, setter);
				generator.Emit(OpCodes.Ret);

				//将委托方法保存到方法标记数组元素中
				methods[i] = new MethodToken(getMethod, setMethod);
			}

			//构建嵌套匿名静态类
			nested.CreateType();
		}

		private static void GenerateTypeInitializer(TypeBuilder builder, PropertyInfo[] properties, ConstructorBuilder constructor, MethodToken[] methods, out FieldBuilder names, out FieldBuilder tokens)
		{
			//var tokenType = typeof(PropertyToken<>).MakeGenericType(typeof(Zongsoft.Data.IDataEntity));
			//var tokenType = _module.GetType(builder.UnderlyingSystemType.FullName + "!PropertyToken");
			var tokenType = _module.GetType(constructor.DeclaringType.FullName);
			names = builder.DefineField(PROPERTY_NAMES_VARIABLE, typeof(string[]), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
			tokens = builder.DefineField(PROPERTY_TOKENS_VARIABLE, typeof(Dictionary<,>).MakeGenericType(typeof(string), tokenType), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
			var entityType = _module.GetType(builder.UnderlyingSystemType.FullName);

			var generator = builder.DefineTypeInitializer().GetILGenerator();

			generator.Emit(OpCodes.Ldc_I4, properties.Length);
			generator.Emit(OpCodes.Newarr, typeof(string));

			for(int i = 0; i < properties.Length; i++)
			{
				generator.Emit(OpCodes.Dup);
				generator.Emit(OpCodes.Ldc_I4, i);
				generator.Emit(OpCodes.Ldstr, properties[i].Name);
				generator.Emit(OpCodes.Stelem_Ref);
			}

			generator.Emit(OpCodes.Stsfld, names);

			generator.Emit(OpCodes.Newobj, tokens.FieldType.GetConstructor(Type.EmptyTypes));

			for(int i = 0; i < properties.Length; i++)
			{
				generator.Emit(OpCodes.Dup);
				generator.Emit(OpCodes.Ldstr, properties[i].Name);
				generator.Emit(OpCodes.Ldc_I4, i);

				generator.Emit(OpCodes.Ldnull);
				generator.Emit(OpCodes.Ldftn, methods[i].GetMethod);
				generator.Emit(OpCodes.Newobj, typeof(Func<,>).MakeGenericType(typeof(object), typeof(object)).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));

				generator.Emit(OpCodes.Ldnull);
				generator.Emit(OpCodes.Ldftn, methods[i].SetMethod);
				generator.Emit(OpCodes.Newobj, typeof(Action<,>).MakeGenericType(typeof(object), typeof(object)).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));

				generator.Emit(OpCodes.Newobj, constructor);
				//generator.Emit(OpCodes.Newobj, tokenType.GetConstructor(new Type[] { typeof(int), typeof(Func<object, object>), typeof(Action<object, object>) }));
				generator.Emit(OpCodes.Call, tokens.FieldType.GetMethod("Add"));
			}

			generator.Emit(OpCodes.Stsfld, tokens);

			generator.Emit(OpCodes.Ret);
		}

		private static ConstructorBuilder GenerateConstructor(TypeBuilder builder, int count, out FieldBuilder mask)
		{
			mask = null;

			if(count <= 8)
				mask = builder.DefineField(MASK_VARIABLE, typeof(byte), FieldAttributes.Private);
			else if(count <= 16)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt16), FieldAttributes.Private);
			else if(count <= 32)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt32), FieldAttributes.Private);
			else if(count <= 64)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt64), FieldAttributes.Private);
			else
				mask = builder.DefineField(MASK_VARIABLE, typeof(byte[]), FieldAttributes.Private);

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

			generator.Emit(OpCodes.Ret);

			return constructor;
		}

		private static void GenerateHasChangesMethod(TypeBuilder builder, FieldBuilder mask, FieldBuilder names, FieldBuilder tokens)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IDataEntity).FullName + ".HasChanges",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(bool),
				new Type[] { typeof(string[]) });

			var tokenType = _module.GetType(builder.UnderlyingSystemType.FullName + "!PropertyToken");

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IDataEntity).GetMethod("HasChanges"));

			//定义方法参数
			method.DefineParameter(1, ParameterAttributes.None, "names");

			//获取代码生成器
			var generator = method.GetILGenerator();

			generator.DeclareLocal(tokenType);
			generator.DeclareLocal(typeof(int));

			var loop = generator.DefineLabel();
			var exit = generator.DefineLabel();

			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Brfalse_S, exit);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldlen);
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ceq);
			generator.Emit(OpCodes.Brfalse_S, exit);

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, mask);
			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Cgt_Un);
			generator.Emit(OpCodes.Brtrue, exit);

			generator.Emit(OpCodes.Ldsfld, tokens);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloc_1);
			generator.Emit(OpCodes.Ldelem_Ref);
			generator.Emit(OpCodes.Ldloca_S, 0);
			generator.Emit(OpCodes.Call, tokens.FieldType.GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance));
			generator.Emit(OpCodes.Brfalse_S, loop);

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, mask);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ldfld, tokenType.GetField("Ordinal"));
			generator.Emit(OpCodes.Shr);
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.And);
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Ceq);
			generator.Emit(OpCodes.Brfalse_S, loop);

			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Ret);

			generator.MarkLabel(loop);

			generator.MarkLabel(exit);

			generator.Emit(OpCodes.Ldc_I4_0);
			generator.Emit(OpCodes.Ret);
		}

		private static void GenerateGetChangesMethod(TypeBuilder builder)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IDataEntity).FullName + ".GetChanges",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(IDictionary<string, object>),
				Type.EmptyTypes);

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IDataEntity).GetMethod("GetChanges"));

			//获取代码生成器
			var generator = method.GetILGenerator();

			generator.Emit(OpCodes.Ldnull);
			generator.Emit(OpCodes.Ret);
		}

		private static void GenerateTryGetMethod(TypeBuilder builder)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IDataEntity).FullName + ".TryGet",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(bool),
				new Type[] { typeof(string), typeof(object).MakeByRefType() });

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IDataEntity).GetMethod("TryGet"));

			//定义方法参数
			method.DefineParameter(1, ParameterAttributes.None, "name");
			method.DefineParameter(2, ParameterAttributes.Out, "value");

			//获取代码生成器
			var generator = method.GetILGenerator();

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Ret);
		}

		private static void GenerateTrySetMethod(TypeBuilder builder)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IDataEntity).FullName + ".TrySet",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(bool),
				new Type[] { typeof(string), typeof(object) });

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IDataEntity).GetMethod("TrySet"));

			//定义方法参数
			method.DefineParameter(1, ParameterAttributes.None, "name");
			method.DefineParameter(2, ParameterAttributes.None, "value");

			//获取代码生成器
			var generator = method.GetILGenerator();

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldc_I4_1);
			generator.Emit(OpCodes.Ret);
		}

		private static ConstructorBuilder GeneratePropertyToken(TypeBuilder type)
		{
			var builder = _module.DefineType(type.FullName + "!PropertyToken", TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout, typeof(ValueType));
			//var builder = type.DefineNestedType("PropertyToken", TypeAttributes.NestedAssembly | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout, typeof(ValueType));

			var ordinal = builder.DefineField("Ordinal", typeof(int), FieldAttributes.Public | FieldAttributes.InitOnly);
			var getter = builder.DefineField("Getter", typeof(Func<,>).MakeGenericType(type, typeof(object)), FieldAttributes.Public | FieldAttributes.InitOnly);
			var setter = builder.DefineField("Setter", typeof(Action<,>).MakeGenericType(type, typeof(object)), FieldAttributes.Public | FieldAttributes.InitOnly);

			var constructor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(int), getter.FieldType, setter.FieldType });
			constructor.DefineParameter(1, ParameterAttributes.None, "ordinal");
			constructor.DefineParameter(2, ParameterAttributes.None, "getter");
			constructor.DefineParameter(3, ParameterAttributes.None, "setter");

			var generator = constructor.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, ordinal);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Stfld, getter);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_3);
			generator.Emit(OpCodes.Stfld, setter);
			generator.Emit(OpCodes.Ret);

			builder.CreateType();

			return constructor;
		}

		private static string GetClassName(Type type)
		{
			return (string.IsNullOrEmpty(type.Namespace) ? string.Empty : type.Namespace + ".") +
			       (type.Name.Length > 1 && type.Name[0] == 'I' && char.IsUpper(type.Name[1]) ? type.Name.Substring(1) : type.Name);
		}
		#endregion

		#region 嵌套子类
		internal struct PropertyToken<T>
		{
			public PropertyToken(int ordinal, Func<T, object> getter, Action<T, object> setter)
			{
				this.Ordinal = ordinal;
				this.Getter = getter;
				this.Setter = setter;
			}

			public readonly int Ordinal;
			public readonly Func<T, object> Getter;
			public readonly Action<T, object> Setter;
		}

		internal struct PropertyTokenEx
		{
			public PropertyTokenEx(int ordinal, Func<object, object> getter, Action<object, object> setter)
			{
				this.Ordinal = ordinal;
				this.Getter = getter;
				this.Setter = setter;
			}

			public int Ordinal;
			public Func<object, object> Getter;
			public Action<object, object> Setter;
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
