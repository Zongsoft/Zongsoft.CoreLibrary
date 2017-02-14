using System;

using Xunit;

using Zongsoft.Tests;

namespace Zongsoft.Reflection.Tests
{
	public class MemberAccessTest
	{
		#region 全局变量
		private Department _department;
		#endregion

		#region 构造函数
		public MemberAccessTest()
		{
			_department = new Department("Develop");

			_department.Employees.AddRange(
				new Employee()
				{
					Name = "Popeye Zhong",
					Gender = Gender.Male,
					Salary = 10000.01m,
					HomeAddress = new Address
					{
						CountryId = 123,
						City = "Wuhan",
						Detail = "光谷",
					},
				},
				new Employee()
				{
					Name = "Jason Yang",
					Gender = Gender.Female,
					Salary = 100,
					HomeAddress = new Address
					{
						CountryId = 456,
						City = "Shenzhen",
						Detail = "大白石洲",
					}
				});
		}
		#endregion

		[Fact]
		public void ResolvePathTest()
		{
			Assert.Null(MemberAccess.Resolve(""));
			Assert.Null(MemberAccess.Resolve("   "));

			MemberToken[] members;

			members = MemberAccess.Resolve("[key].workbench.views[0, 'column'].Value");
			Assert.NotNull(members);
			Assert.Equal(5, members.Length);
			Assert.Equal(1, members[0].Parameters.Length);
			Assert.Equal("key", members[0].Parameters[0]);
			Assert.Equal("workbench", members[1].Name);
			Assert.Equal("views", members[2].Name);
			Assert.Equal(2, members[3].Parameters.Length);
			Assert.Equal(0, members[3].Parameters[0]);
			Assert.Equal("column", members[3].Parameters[1]);
			Assert.Equal("Value", members[4].Name);

			members = MemberAccess.Resolve("  [ key ]. workbench .views [ 0 , 'column' ] . Value ");
			Assert.NotNull(members);
			Assert.Equal(5, members.Length);
			Assert.Equal(1, members[0].Parameters.Length);
			Assert.Equal("key", members[0].Parameters[0]);
			Assert.Equal("workbench", members[1].Name);
			Assert.Equal("views", members[2].Name);
			Assert.Equal(2, members[3].Parameters.Length);
			Assert.Equal(0, members[3].Parameters[0]);
			Assert.Equal("column", members[3].Parameters[1]);
			Assert.Equal("Value", members[4].Name);

		}

		[Fact]
		public void GetMemberTypeTest()
		{
			Assert.Same(typeof(Employee), MemberAccess.GetMemberType(_department, "[0]"));
			Assert.Same(typeof(Employee), MemberAccess.GetMemberType(_department.GetType(), "['Popeye Zhong']"));
			Assert.Same(typeof(Employee), MemberAccess.GetMemberType(_department.GetType(), "Employees[0]"));
			Assert.Same(typeof(Employee), MemberAccess.GetMemberType(_department, "Employees['Popeye Zhong']"));

			Assert.Same(typeof(string), MemberAccess.GetMemberType(_department, "[0].Name"));
			Assert.Same(typeof(string), MemberAccess.GetMemberType(_department, "['Popeye Zhong'].Name"));
			Assert.Same(typeof(string), MemberAccess.GetMemberType(_department, "Employees[0].Name"));
			Assert.Same(typeof(string), MemberAccess.GetMemberType(_department, "Employees['Popeye Zhong'].Name"));
		}

		[Fact]
		public void AccessMemberValueTest()
		{
			var emp1 = _department[0];

			Assert.Equal("Popeye Zhong", MemberAccess.GetMemberValue<string>(emp1, "Name"));
			Assert.Equal("Wuhan", MemberAccess.GetMemberValue<string>(emp1, "HomeAddress.City"));

			MemberAccess.SetMemberValue(emp1, "Name", "Popeye");
			Assert.Equal("Popeye", MemberAccess.GetMemberValue<string>(emp1, "Name"));

			MemberAccess.SetMemberValue(emp1, "HomeAddress.City", "Hunan Shaoyang");
			Assert.Equal("Hunan Shaoyang", MemberAccess.GetMemberValue<string>(emp1, "HomeAddress.City"));

			Assert.NotNull(MemberAccess.GetMemberValue<object>(_department, "[0]"));
			Assert.NotNull(MemberAccess.GetMemberValue<object>(_department, "['Popeye Zhong']"));
			Assert.Equal("Jason Yang", MemberAccess.GetMemberValue<string>(_department, "[1].Name"));
			Assert.Equal("Jason Yang", MemberAccess.GetMemberValue<string>(_department, "['Jason Yang'].Name"));

			Assert.NotNull(MemberAccess.GetMemberValue<object>(_department, "Employees[0]"));
			Assert.NotNull(MemberAccess.GetMemberValue<object>(_department, "Employees['Popeye Zhong']"));
			Assert.Equal("Jason Yang", MemberAccess.GetMemberValue<string>(_department, "Employees[1].Name"));
			Assert.Equal("Jason Yang", MemberAccess.GetMemberValue<string>(_department, "Employees['Jason Yang'].Name"));
		}
	}
}
