using System;
using System.Linq;
using System.Linq.Expressions;
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
		private const string PROPERTY_ACCESSORS_VARIABLE = "$PROPERTIES$";
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
			var type = typeof(T);

			if(!type.IsInterface)
				throw new ArgumentException("The generic parameter type must be a interface.");

			_locker.EnterReadLock();
			var existed = _cache.TryGetValue(type, out var creator);
			_locker.ExitReadLock();

			if(existed)
				return (T)creator();

			_locker.EnterWriteLock();

			try
			{
				if(!_cache.TryGetValue(type, out creator))
					creator = _cache[type] = Compile(type);
			}
			finally
			{
				_locker.ExitWriteLock();
			}

			var result =creator();

			return (T)result;
		}
		#endregion

		#region 私有方法
		private static Func<object> Compile(Type type)
		{
			var builder = _module.DefineType(
				GetClassName(type),
				TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed,
				null,
				new Type[] { typeof(Zongsoft.Data.IDataEntity) });

			//添加类型的实现接口声明
			builder.AddInterfaceImplementation(typeof(Zongsoft.Data.IDataEntity));
			builder.AddInterfaceImplementation(type);

			//获取指定接口的所有属性集
			var properties = type.GetProperties();

			//生成静态构造函数
			GenerateTypeInitializer(builder, properties, out var names, out var accessors);

			FieldBuilder mask = null;
			ILGenerator generator;

			if(properties.Length <= 8)
				mask = builder.DefineField(MASK_VARIABLE, typeof(byte), FieldAttributes.Private);
			else if(properties.Length <= 16)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt16), FieldAttributes.Private);
			else if(properties.Length <= 32)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt32), FieldAttributes.Private);
			else if(properties.Length <= 64)
				mask = builder.DefineField(MASK_VARIABLE, typeof(UInt64), FieldAttributes.Private);
			else
				mask = builder.DefineField(MASK_VARIABLE, typeof(byte[]), FieldAttributes.Private);

			//生成构造函数
			if(properties.Length > 64)
			{
				var constructor = builder
					.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, null)
					.GetILGenerator();

				constructor.Emit(OpCodes.Ldarg_0);
				constructor.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
				constructor.Emit(OpCodes.Ldarg_0);
				constructor.Emit(OpCodes.Ldc_I4, (int)Math.Ceiling(properties.Length / 8.0));
				constructor.Emit(OpCodes.Newarr, typeof(byte));
				constructor.Emit(OpCodes.Stfld, mask);
				constructor.Emit(OpCodes.Ret);
			}

			//生成属性定义
			for(int i = 0; i < properties.Length; i++)
			{
				var field = builder.DefineField("$" + properties[i].Name, properties[i].PropertyType, FieldAttributes.Private);
				var property = builder.DefineProperty(properties[i].Name, PropertyAttributes.None, properties[i].PropertyType, null);

				var getter = builder.DefineMethod("get_" + properties[i].Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot, properties[i].PropertyType, Type.EmptyTypes);
				generator = getter.GetILGenerator();
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
			}

			//生成“HasChanges”方法
			GenerateHasChangesMethod(builder);

			//生成“GetChanges”方法
			GenerateGetChangesMethod(builder);

			//生成“TryGet”方法
			GenerateTryGetMethod(builder);

			//生成“TrySet”方法
			GenerateTrySetMethod(builder);

			//创建类型代码
			type = builder.CreateType();

			_assembly.Save(ASSEMBLY_NAME + ".dll");

			//生成一个创建实例的动态方法
			var creator = new DynamicMethod("Create", typeof(object), Type.EmptyTypes);

			generator = creator.GetILGenerator();
			generator.DeclareLocal(typeof(object));
			generator.Emit(OpCodes.Newobj, builder.UnderlyingSystemType.GetConstructor(Type.EmptyTypes));
			generator.Emit(OpCodes.Stloc_0);
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Ret);

			//返回实例创建方法的委托
			return (Func<object>)creator.CreateDelegate(typeof(Func<object>));
		}

		private static void GenerateTypeInitializer(TypeBuilder builder, PropertyInfo[] properties, out FieldBuilder names, out FieldBuilder accessors)
		{
			names = builder.DefineField(PROPERTY_NAMES_VARIABLE, typeof(string[]), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);
			accessors = builder.DefineField(PROPERTY_ACCESSORS_VARIABLE, typeof(Dictionary<string, DataEntity.PropertyToken<Zongsoft.Data.IDataEntity>>), FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly);

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

			generator.Emit(OpCodes.Ret);
		}

		private static void GenerateHasChangesMethod(TypeBuilder builder)
		{
			var method = builder.DefineMethod(typeof(Zongsoft.Data.IDataEntity).FullName + ".HasChanges",
				MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot,
				typeof(bool),
				new Type[] { typeof(string[]) });

			//添加方法的实现标记
			builder.DefineMethodOverride(method, typeof(Zongsoft.Data.IDataEntity).GetMethod("HasChanges"));

			//定义方法参数
			method.DefineParameter(1, ParameterAttributes.None, "names");

			//获取代码生成器
			var generator = method.GetILGenerator();

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldc_I4_1);
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

		private static string GetClassName(Type type)
		{
			return type.Name[0] == 'I' && char.IsUpper(type.Name[1]) ? type.Name.Substring(1) : type.Name;
		}
		#endregion

		#region 嵌套子类
		internal struct PropertyToken<T> where T : Zongsoft.Data.IDataEntity
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
		#endregion
	}
}
