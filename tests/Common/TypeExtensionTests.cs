using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Zongsoft.Tests;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class TypeExtensionTests
	{
		[Fact]
		public void IsAssignableFromTest()
		{
			var baseType = typeof(ICollection<Person>);
			var instanceType = typeof(Collection<Person>);

			Assert.True(Zongsoft.Common.TypeExtension.IsAssignableFrom(baseType, instanceType));
			Assert.True(baseType.IsAssignableFrom(instanceType));

			baseType = typeof(ICollection<>);

			Assert.True(Zongsoft.Common.TypeExtension.IsAssignableFrom(baseType, instanceType));
			Assert.False(baseType.IsAssignableFrom(instanceType));

			Assert.True(TypeExtension.IsAssignableFrom(typeof(PersonServiceBase<>), typeof(EmployeeService)));
		}

		[Fact]
		public void GetTypeTest()
		{
			Assert.Same(typeof(void), Zongsoft.Common.TypeExtension.GetType("void"));
			Assert.Same(typeof(object), Zongsoft.Common.TypeExtension.GetType("object"));
			Assert.Same(typeof(object), Zongsoft.Common.TypeExtension.GetType("System.object"));

			Assert.Same(typeof(string), Zongsoft.Common.TypeExtension.GetType("string"));
			Assert.Same(typeof(string), Zongsoft.Common.TypeExtension.GetType("System.string"));

			Assert.Same(typeof(int), Zongsoft.Common.TypeExtension.GetType("int"));
			Assert.Same(typeof(int), Zongsoft.Common.TypeExtension.GetType("int32"));
			Assert.Same(typeof(int), Zongsoft.Common.TypeExtension.GetType("System.Int32"));
			Assert.Same(typeof(int?), Zongsoft.Common.TypeExtension.GetType("int?"));
			Assert.Same(typeof(int[]), Zongsoft.Common.TypeExtension.GetType("int[]"));

			Assert.Same(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("date"));
			Assert.Same(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("time"));
			Assert.Same(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("datetime"));
			Assert.Same(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("date?"));
			Assert.Same(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("time?"));
			Assert.Same(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("datetime?"));
			Assert.Same(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("date[]"));
			Assert.Same(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("time[]"));
			Assert.Same(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("datetime[]"));

			Assert.Same(typeof(Guid), Zongsoft.Common.TypeExtension.GetType("GUID"));
			Assert.Same(typeof(Guid), Zongsoft.Common.TypeExtension.GetType("system.guid"));
			Assert.Same(typeof(Guid?), Zongsoft.Common.TypeExtension.GetType("guid?"));
			Assert.Same(typeof(Guid[]), Zongsoft.Common.TypeExtension.GetType("guid[]"));

			Assert.Same(typeof(Person), Zongsoft.Common.TypeExtension.GetType("Zongsoft.Tests.Person, Zongsoft.CoreLibrary.Tests"));
		}

		[Fact]
		public void GetDefaultValueTest()
		{
			Assert.Equal(0, TypeExtension.GetDefaultValue(typeof(int)));
			Assert.Equal(0d, TypeExtension.GetDefaultValue(typeof(double)));
			Assert.Equal(DBNull.Value, TypeExtension.GetDefaultValue(typeof(DBNull)));
			Assert.Equal(Gender.Female, TypeExtension.GetDefaultValue(typeof(Gender)));
			Assert.Null(TypeExtension.GetDefaultValue(typeof(int?)));
			Assert.Null(TypeExtension.GetDefaultValue(typeof(string)));
		}

		#region 嵌套子类
		public class PersonServiceBase<T> where T : Person
		{
			public virtual T GetPerson(int id)
			{
				throw new NotImplementedException();
			}
		}

		public class EmployeeService : PersonServiceBase<Employee>
		{
			public override Employee GetPerson(int id)
			{
				throw new NotImplementedException();
			}
		}
		#endregion
	}
}
