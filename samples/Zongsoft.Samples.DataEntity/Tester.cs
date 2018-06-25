using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity
{
	public class Tester
	{
		public static void Build()
		{
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Zongsoft.Samples.Dynamics"), AssemblyBuilderAccess.RunAndSave);
			var module = assembly.DefineDynamicModule("DefaultModule", "test.dll");
			var builder = module.DefineType("MyFoo", TypeAttributes.Class | TypeAttributes.Public);

			var method = builder.DefineMethod("Say", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), new Type[] { typeof(string) });
			Expression<Func<string, bool>> lambda = p => true;
			lambda.CompileToMethod(method);

			builder.CreateType();
			assembly.Save("test.dll");
		}
	}

	public interface IFoo
	{
		bool Say(string message);
	}
}
