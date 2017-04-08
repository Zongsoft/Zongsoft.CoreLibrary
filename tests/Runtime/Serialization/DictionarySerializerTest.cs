using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using Zongsoft.Tests;

namespace Zongsoft.Runtime.Serialization.Tests
{
	public class DictionarySerializerTest
	{
		#region 私有变量
		private Department _department;
		#endregion

		#region 构造函数
		public DictionarySerializerTest()
		{
			_department = new Department
			{
				CorporationId = 1,
				DepartmentId = 101,
				Name = "开发部",
			};

			var employee = new Employee()
			{
				EmployeeId = 100,
				Name = "钟少",
				Gender = Gender.Male,
				Salary = 30000.5m,
				HomeAddress = new Address()
				{
					CountryId = 86,
					City = "武汉市",
					Detail = "光谷软件园",
					PostalCode = "123456",
				},
				OfficeAddress = new Address
				{
					City = "Shenzhen",
					Detail = "Futian",
				},
				Department = new Department
				{
					DepartmentId = 101,
					Name = "开发部",
					CorporationId = 1,
				},
			};

			_department.Employees.Add(employee);
		}
		#endregion

		[Fact]
		public void SerializeTest()
		{
			var dictionary = DictionarySerializer.Default.Serialize(_department);
			Assert.NotNull(dictionary);
		}
	}
}
