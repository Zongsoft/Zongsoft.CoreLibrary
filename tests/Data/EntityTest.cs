using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data
{
	public class EntityTest
	{
		#region 常量定义
		private static readonly string NOTEXISTS = "NotExists!";
		private static readonly DateTime BIRTHDATE = DateTime.Now;
		#endregion

		#region 公共方法
		[Fact]
		public void Test()
		{
			TestPerson();
			TestEmployee();
			TestCustomer();
			TestSpecialEmployee();
		}

		[Fact]
		public void TestPerson()
		{
			// Dynamically build an instance of the IPerson interface.
			var person = Entity.Build<Zongsoft.Tests.IPerson>();
			Assert.NotNull(person);
			this.TestPerson(person);
		}

		[Fact]
		public void TestEmployee()
		{
			// Dynamically build an instance of the IEmployee interface.
			var employee = Entity.Build<Zongsoft.Tests.IEmployee>();
			Assert.NotNull(employee);
			this.TestEmployee(employee);
		}

		[Fact]
		public void TestCustomer()
		{
			// Dynamically build an instance of the ICustomer interface.
			var customer = Entity.Build<Zongsoft.Tests.ICustomer>();
			Assert.NotNull(customer);
			this.TestCustomer(customer);
		}

		[Fact]
		public void TestSpecialEmployee()
		{
			// Dynamically build an instance of the ISpecialEmployee interface.
			var special = Entity.Build<Zongsoft.Tests.ISpecialEmployee>();
			Assert.NotNull(special);
			this.TestSpecialEmployee(special);
		}
		#endregion

		#region 私有方法
		private void TestPerson(Tests.IPerson person)
		{
			if(person == null)
				throw new ArgumentNullException(nameof(person));

			object value;
			IDictionary<string, object> changes;

			Assert.NotNull(person);
			Assert.Null(person.GetChanges());
			Assert.False(person.HasChanges());

			// Test the TryGetValue(...) method for properties of the IPerson interface on uninitialized(unchanged).
			Assert.False(person.TryGetValue(nameof(person.Name), out value));
			Assert.False(person.TryGetValue(nameof(person.Gender), out value));
			Assert.False(person.TryGetValue(nameof(person.Birthdate), out value));
			Assert.False(person.TryGetValue(nameof(person.BloodType), out value));
			Assert.False(person.TryGetValue(nameof(person.HomeAddress), out value));

			Assert.False(person.TryGetValue(NOTEXISTS, out value));
			Assert.False(person.TrySetValue(NOTEXISTS, null));

			// Change the 'Name' property of the IPerson interface.
			person.Name = "Popeye";
			Assert.True(person.HasChanges());
			Assert.True(person.HasChanges(nameof(person.Name)));
			Assert.True(person.HasChanges(nameof(person.Name), nameof(person.Gender), nameof(person.Birthdate), nameof(person.BloodType), nameof(person.HomeAddress)));
			Assert.False(person.HasChanges(nameof(person.Gender)));
			Assert.False(person.HasChanges(nameof(person.Birthdate)));
			Assert.False(person.HasChanges(nameof(person.BloodType)));
			Assert.False(person.HasChanges(nameof(person.HomeAddress)));

			changes = person.GetChanges();
			Assert.Equal(1, changes.Count);
			Assert.True(changes.TryGetValue(nameof(person.Name), out value));
			Assert.Equal("Popeye", value);

			// Change the 'Gender' property of the IPerson interface.
			person.Gender = Tests.Gender.Male;
			Assert.True(person.HasChanges());
			Assert.True(person.HasChanges(nameof(person.Gender)));
			Assert.True(person.HasChanges(nameof(person.Name), nameof(person.Gender), nameof(person.Birthdate), nameof(person.BloodType), nameof(person.HomeAddress)));
			Assert.True(person.HasChanges(nameof(person.Gender)));
			Assert.False(person.HasChanges(nameof(person.Birthdate)));
			Assert.False(person.HasChanges(nameof(person.BloodType)));
			Assert.False(person.HasChanges(nameof(person.HomeAddress)));

			changes = person.GetChanges();
			Assert.Equal(2, changes.Count);
			Assert.True(changes.TryGetValue(nameof(person.Name), out value));
			Assert.Equal("Popeye", value);
			Assert.True(changes.TryGetValue(nameof(person.Gender), out value));
			Assert.Equal(Tests.Gender.Male, value);

			// Change the 'Birthdate' property of the IPerson interface.
			person.Birthdate = new DateTime(BIRTHDATE.Ticks);
			Assert.True(person.HasChanges());
			Assert.True(person.HasChanges(nameof(person.Birthdate)));
			Assert.True(person.HasChanges(nameof(person.Name), nameof(person.Gender), nameof(person.Birthdate), nameof(person.BloodType), nameof(person.HomeAddress)));
			Assert.True(person.HasChanges(nameof(person.Gender)));
			Assert.True(person.HasChanges(nameof(person.Birthdate)));
			Assert.False(person.HasChanges(nameof(person.BloodType)));
			Assert.False(person.HasChanges(nameof(person.HomeAddress)));

			changes = person.GetChanges();
			Assert.Equal(3, changes.Count);
			Assert.True(changes.TryGetValue(nameof(person.Name), out value));
			Assert.Equal("Popeye", value);
			Assert.True(changes.TryGetValue(nameof(person.Gender), out value));
			Assert.Equal(Tests.Gender.Male, value);
			Assert.True(changes.TryGetValue(nameof(person.Birthdate), out value));
			Assert.Equal(BIRTHDATE, value);

			// Change the 'BloodType' property of the IPerson interface via TrySetValue(...) method.
			Assert.True(person.TrySetValue(nameof(person.BloodType), "X"));
			Assert.True(person.HasChanges());
			Assert.True(person.HasChanges(nameof(person.BloodType)));
			Assert.True(person.HasChanges(nameof(person.Name), nameof(person.Gender), nameof(person.Birthdate), nameof(person.BloodType), nameof(person.HomeAddress)));
			Assert.True(person.HasChanges(nameof(person.Gender)));
			Assert.True(person.HasChanges(nameof(person.Birthdate)));
			Assert.True(person.HasChanges(nameof(person.BloodType)));
			Assert.False(person.HasChanges(nameof(person.HomeAddress)));

			changes = person.GetChanges();
			Assert.Equal(4, changes.Count);
			Assert.True(changes.TryGetValue(nameof(person.Name), out value));
			Assert.Equal("Popeye", value);
			Assert.True(changes.TryGetValue(nameof(person.Gender), out value));
			Assert.Equal(Tests.Gender.Male, value);
			Assert.True(changes.TryGetValue(nameof(person.Birthdate), out value));
			Assert.Equal(BIRTHDATE, value);
			Assert.True(changes.TryGetValue(nameof(person.BloodType), out value));
			Assert.Equal("X", value);

			// Change the 'HomeAddress' property of the IPerson interface via TrySetValue(...) method.
			Assert.True(person.TrySetValue(nameof(person.HomeAddress), new Tests.Address()
			{
				CountryId = 86,
				PostalCode = "430223",
				City = "Wuhan",
				Detail = "No.1 The university park road",
			}));
			Assert.True(person.HasChanges());
			Assert.True(person.HasChanges(nameof(person.HomeAddress)));
			Assert.True(person.HasChanges(nameof(person.Name), nameof(person.Gender), nameof(person.Birthdate), nameof(person.BloodType), nameof(person.HomeAddress)));
			Assert.True(person.HasChanges(nameof(person.Gender)));
			Assert.True(person.HasChanges(nameof(person.Birthdate)));
			Assert.True(person.HasChanges(nameof(person.BloodType)));
			Assert.True(person.HasChanges(nameof(person.HomeAddress)));

			changes = person.GetChanges();
			Assert.Equal(5, changes.Count);
			Assert.True(changes.TryGetValue(nameof(person.Name), out value));
			Assert.Equal("Popeye", value);
			Assert.True(changes.TryGetValue(nameof(person.Gender), out value));
			Assert.Equal(Tests.Gender.Male, value);
			Assert.True(changes.TryGetValue(nameof(person.Birthdate), out value));
			Assert.Equal(BIRTHDATE, value);
			Assert.True(changes.TryGetValue(nameof(person.BloodType), out value));
			Assert.Equal("X", value);
			Assert.True(changes.TryGetValue(nameof(person.HomeAddress), out value));
			Assert.Equal(86, ((Tests.Address)value).CountryId);
			Assert.Equal("430223", ((Tests.Address)value).PostalCode);
			Assert.Equal("Wuhan", ((Tests.Address)value).City);
			Assert.Contains("No.1", ((Tests.Address)value).Detail);

			// Retest the TryGetValue(...) method for properties of the IPerson interface on initialized(changed).
			Assert.True(person.TryGetValue(nameof(person.Name), out value));
			Assert.Equal("Popeye", value);
			Assert.True(person.TryGetValue(nameof(person.Gender), out value));
			Assert.Equal(Tests.Gender.Male, value);
			Assert.True(person.TryGetValue(nameof(person.Birthdate), out value));
			Assert.Equal(BIRTHDATE, value);
			Assert.True(person.TryGetValue(nameof(person.BloodType), out value));
			Assert.Equal("X", value);
			Assert.True(person.TryGetValue(nameof(person.HomeAddress), out value));
			Assert.Equal(86, ((Tests.Address)value).CountryId);
			Assert.Equal("430223", ((Tests.Address)value).PostalCode);
			Assert.Equal("Wuhan", ((Tests.Address)value).City);
			Assert.Contains("No.1", ((Tests.Address)value).Detail);

			Assert.False(person.TryGetValue(NOTEXISTS, out value));
			Assert.False(person.TrySetValue(NOTEXISTS, null));
		}

		private void TestEmployee(Tests.IEmployee employee)
		{
			object value;
			IDictionary<string, object> changes;

			// Test the IPerson interface.
			this.TestPerson(employee);

			// Test the TryGetValue(...) method for properties of the IEmployee interface on uninitialized(unchanged).
			Assert.False(employee.TryGetValue(nameof(employee.EmployeeId), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Department), out value));
			Assert.False(employee.TryGetValue(nameof(employee.OfficeAddress), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Salary), out value));

			Assert.False(employee.TryGetValue(NOTEXISTS, out value));
			Assert.False(employee.TrySetValue(NOTEXISTS, null));

			// Change the 'EmployeeId' property of the IEmployee interface.
			employee.EmployeeId = 100;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId)));
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId), nameof(employee.Department), nameof(employee.OfficeAddress), nameof(employee.Salary)));
			Assert.False(employee.HasChanges(nameof(employee.Department)));
			Assert.False(employee.HasChanges(nameof(employee.OfficeAddress)));
			Assert.False(employee.HasChanges(nameof(employee.Salary)));

			changes = employee.GetChanges();
			Assert.Equal(5 + 1, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.EmployeeId), out value));
			Assert.Equal(100, value);

			// Change the 'Department' property of the IEmployee interface.
			employee.Department = new Tests.Department("Development");
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId)));
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId), nameof(employee.Department), nameof(employee.OfficeAddress), nameof(employee.Salary)));
			Assert.True(employee.HasChanges(nameof(employee.Department)));
			Assert.False(employee.HasChanges(nameof(employee.OfficeAddress)));
			Assert.False(employee.HasChanges(nameof(employee.Salary)));

			changes = employee.GetChanges();
			Assert.Equal(5 + 2, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.EmployeeId), out value));
			Assert.Equal(100, value);
			Assert.True(changes.TryGetValue(nameof(employee.Department), out value));
			Assert.Equal("Development", ((Tests.Department)value).Name);

			// Change the 'OfficeAddress' property of the IEmployee interface.
			Assert.True(employee.TrySetValue(nameof(employee.OfficeAddress), new Tests.Address()
			{
				CountryId = 86,
				PostalCode = "518049",
				City = "Shenzhen",
				Detail = "No.19 Meilin road, Futian district",
			}));
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId)));
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId), nameof(employee.Department), nameof(employee.OfficeAddress), nameof(employee.Salary)));
			Assert.True(employee.HasChanges(nameof(employee.Department)));
			Assert.True(employee.HasChanges(nameof(employee.OfficeAddress)));
			Assert.False(employee.HasChanges(nameof(employee.Salary)));

			changes = employee.GetChanges();
			Assert.Equal(5 + 3, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.EmployeeId), out value));
			Assert.Equal(100, value);
			Assert.True(changes.TryGetValue(nameof(employee.Department), out value));
			Assert.Equal("Development", ((Tests.Department)value).Name);
			Assert.True(changes.TryGetValue(nameof(employee.OfficeAddress), out value));
			Assert.Equal(86, ((Tests.Address)value).CountryId);
			Assert.Equal("518049", ((Tests.Address)value).PostalCode);
			Assert.Equal("Shenzhen", ((Tests.Address)value).City);
			Assert.Contains("No.19", ((Tests.Address)value).Detail);

			// Change the 'Salary' property of the IEmployee interface.
			Assert.True(employee.TrySetValue(nameof(employee.Salary), 35000m));
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId)));
			Assert.True(employee.HasChanges(nameof(employee.EmployeeId), nameof(employee.Department), nameof(employee.OfficeAddress), nameof(employee.Salary)));
			Assert.True(employee.HasChanges(nameof(employee.Department)));
			Assert.True(employee.HasChanges(nameof(employee.OfficeAddress)));
			Assert.True(employee.HasChanges(nameof(employee.Salary)));

			changes = employee.GetChanges();
			Assert.Equal(5 + 4, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.EmployeeId), out value));
			Assert.Equal(100, value);
			Assert.True(changes.TryGetValue(nameof(employee.Department), out value));
			Assert.Equal("Development", ((Tests.Department)value).Name);
			Assert.True(changes.TryGetValue(nameof(employee.OfficeAddress), out value));
			Assert.Equal(86, ((Tests.Address)value).CountryId);
			Assert.Equal("518049", ((Tests.Address)value).PostalCode);
			Assert.Equal("Shenzhen", ((Tests.Address)value).City);
			Assert.Contains("No.19", ((Tests.Address)value).Detail);
			Assert.True(changes.TryGetValue(nameof(employee.Salary), out value));
			Assert.Equal(35000m, value);

			// Retest the TryGetValue(...) method for properties of the IEmployee interface on initialized(changed).
			Assert.True(employee.TryGetValue(nameof(employee.EmployeeId), out value));
			Assert.Equal(100, value);
			Assert.True(employee.TryGetValue(nameof(employee.Salary), out value));
			Assert.Equal(35000m, value);
			Assert.True(employee.TryGetValue(nameof(employee.Department), out value));
			Assert.Equal("Development", ((Tests.Department)value).Name);
			Assert.True(employee.TryGetValue(nameof(employee.OfficeAddress), out value));
			Assert.Equal(86, ((Tests.Address)value).CountryId);
			Assert.Equal("518049", ((Tests.Address)value).PostalCode);
			Assert.Equal("Shenzhen", ((Tests.Address)value).City);
			Assert.Contains("No.19", ((Tests.Address)value).Detail);

			Assert.False(employee.TryGetValue(NOTEXISTS, out value));
			Assert.False(employee.TrySetValue(NOTEXISTS, null));
		}

		private void TestCustomer(Tests.ICustomer customer)
		{
			object value;
			IDictionary<string, object> changes;

			// Test the IPerson interface.
			this.TestPerson(customer);

			// Test the TryGetValue(...) method for properties of the ICustomer interface on uninitialized(unchanged).
			Assert.False(customer.TryGetValue(nameof(customer.Level), out value));

			Assert.False(customer.TryGetValue(NOTEXISTS, out value));
			Assert.False(customer.TrySetValue(NOTEXISTS, null));

			// Change the 'Level' property of the ICustomer interface.
			customer.Level = 10;
			Assert.True(customer.HasChanges());
			Assert.True(customer.HasChanges(nameof(customer.Level)));

			changes = customer.GetChanges();
			Assert.Equal(5 + 1, changes.Count);
			Assert.True(changes.TryGetValue(nameof(customer.Level), out value));
			Assert.Equal((byte)10, value);

			// Retest the TryGetValue(...) method for properties of the ICustomer interface on initialized(changed).
			Assert.True(customer.TryGetValue(nameof(customer.Level), out value));
			Assert.Equal((byte)10, value);

			Assert.False(customer.TryGetValue(NOTEXISTS, out value));
			Assert.False(customer.TrySetValue(NOTEXISTS, null));
		}

		private void TestSpecialEmployee(Tests.ISpecialEmployee employee)
		{
			object value;
			IDictionary<string, object> changes;

			// Test the IEmployee interface.
			this.TestEmployee(employee);

			// Test the TryGetValue(...) method for properties of the ISpecialEmployee interface on uninitialized(unchanged).
			Assert.False(employee.TryGetValue(nameof(employee.Property01), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property02), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property03), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property04), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property05), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property06), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property07), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property11), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property12), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property13), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property14), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property15), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property16), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property17), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property18), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property19), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property20), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(employee.TryGetValue(nameof(employee.Property56), out value));

			Assert.False(employee.TryGetValue(NOTEXISTS, out value));
			Assert.False(employee.TrySetValue(NOTEXISTS, null));

			// Change the 'Property01' property of the ISpecialEmployee interface.
			employee.Property01 = 1;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.False(employee.HasChanges(nameof(employee.Property02)));
			Assert.False(employee.HasChanges(nameof(employee.Property03)));
			Assert.False(employee.HasChanges(nameof(employee.Property04)));
			Assert.False(employee.HasChanges(nameof(employee.Property05)));
			Assert.False(employee.HasChanges(nameof(employee.Property06)));
			Assert.False(employee.HasChanges(nameof(employee.Property07)));
			Assert.False(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 1, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property02' property of the ISpecialEmployee interface.
			employee.Property02 = 2;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.False(employee.HasChanges(nameof(employee.Property03)));
			Assert.False(employee.HasChanges(nameof(employee.Property04)));
			Assert.False(employee.HasChanges(nameof(employee.Property05)));
			Assert.False(employee.HasChanges(nameof(employee.Property06)));
			Assert.False(employee.HasChanges(nameof(employee.Property07)));
			Assert.False(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 2, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property03' property of the ISpecialEmployee interface.
			employee.Property03 = 3;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.False(employee.HasChanges(nameof(employee.Property04)));
			Assert.False(employee.HasChanges(nameof(employee.Property05)));
			Assert.False(employee.HasChanges(nameof(employee.Property06)));
			Assert.False(employee.HasChanges(nameof(employee.Property07)));
			Assert.False(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 3, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property04' property of the ISpecialEmployee interface.
			employee.Property04 = 4;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.False(employee.HasChanges(nameof(employee.Property05)));
			Assert.False(employee.HasChanges(nameof(employee.Property06)));
			Assert.False(employee.HasChanges(nameof(employee.Property07)));
			Assert.False(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 4, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property05' property of the ISpecialEmployee interface.
			employee.Property05 = 5;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.False(employee.HasChanges(nameof(employee.Property06)));
			Assert.False(employee.HasChanges(nameof(employee.Property07)));
			Assert.False(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 5, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property06' property of the ISpecialEmployee interface.
			employee.Property06 = 6;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.True(employee.HasChanges(nameof(employee.Property06)));
			Assert.False(employee.HasChanges(nameof(employee.Property07)));
			Assert.False(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 6, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.Equal(6, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property07' property of the ISpecialEmployee interface.
			employee.Property07 = 7;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.True(employee.HasChanges(nameof(employee.Property06)));
			Assert.True(employee.HasChanges(nameof(employee.Property07)));
			Assert.False(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 7, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.Equal(6, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.Equal(7, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property08' property of the ISpecialEmployee interface.
			employee.Property08 = 8;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.True(employee.HasChanges(nameof(employee.Property06)));
			Assert.True(employee.HasChanges(nameof(employee.Property07)));
			Assert.True(employee.HasChanges(nameof(employee.Property08)));
			Assert.False(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 8, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.Equal(6, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.Equal(7, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.Equal(8, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property09' property of the ISpecialEmployee interface.
			employee.Property09 = 9;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.True(employee.HasChanges(nameof(employee.Property06)));
			Assert.True(employee.HasChanges(nameof(employee.Property07)));
			Assert.True(employee.HasChanges(nameof(employee.Property08)));
			Assert.True(employee.HasChanges(nameof(employee.Property09)));
			Assert.False(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 9, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.Equal(6, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.Equal(7, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.Equal(8, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.Equal(9, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property10' property of the ISpecialEmployee interface.
			employee.Property10 = 10;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.True(employee.HasChanges(nameof(employee.Property06)));
			Assert.True(employee.HasChanges(nameof(employee.Property07)));
			Assert.True(employee.HasChanges(nameof(employee.Property08)));
			Assert.True(employee.HasChanges(nameof(employee.Property09)));
			Assert.True(employee.HasChanges(nameof(employee.Property10)));
			Assert.False(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 10, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.Equal(6, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.Equal(7, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.Equal(8, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.Equal(9, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.Equal(10, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property55' property of the ISpecialEmployee interface.
			employee.Property55 = 55;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.True(employee.HasChanges(nameof(employee.Property06)));
			Assert.True(employee.HasChanges(nameof(employee.Property07)));
			Assert.True(employee.HasChanges(nameof(employee.Property08)));
			Assert.True(employee.HasChanges(nameof(employee.Property09)));
			Assert.True(employee.HasChanges(nameof(employee.Property10)));
			Assert.True(employee.HasChanges(nameof(employee.Property55)));
			Assert.False(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 11, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.Equal(6, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.Equal(7, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.Equal(8, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.Equal(9, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.Equal(10, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.Equal(55, value);
			Assert.False(changes.TryGetValue(nameof(employee.Property56), out value));

			// Change the 'Property56' property of the ISpecialEmployee interface.
			employee.Property56 = 56;
			Assert.True(employee.HasChanges());
			Assert.True(employee.HasChanges(nameof(employee.Property01)));
			Assert.True(employee.HasChanges(nameof(employee.Property01), nameof(employee.Property02), nameof(employee.Property03), nameof(employee.Property04), nameof(employee.Property05), nameof(employee.Property06), nameof(employee.Property07), nameof(employee.Property08), nameof(employee.Property09), nameof(employee.Property10), nameof(employee.Property55), nameof(employee.Property56)));
			Assert.True(employee.HasChanges(nameof(employee.Property02)));
			Assert.True(employee.HasChanges(nameof(employee.Property03)));
			Assert.True(employee.HasChanges(nameof(employee.Property04)));
			Assert.True(employee.HasChanges(nameof(employee.Property05)));
			Assert.True(employee.HasChanges(nameof(employee.Property06)));
			Assert.True(employee.HasChanges(nameof(employee.Property07)));
			Assert.True(employee.HasChanges(nameof(employee.Property08)));
			Assert.True(employee.HasChanges(nameof(employee.Property09)));
			Assert.True(employee.HasChanges(nameof(employee.Property10)));
			Assert.True(employee.HasChanges(nameof(employee.Property55)));
			Assert.True(employee.HasChanges(nameof(employee.Property56)));

			changes = employee.GetChanges();
			Assert.Equal(9 + 12, changes.Count);
			Assert.True(changes.TryGetValue(nameof(employee.Property01), out value));
			Assert.Equal(1, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property02), out value));
			Assert.Equal(2, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property03), out value));
			Assert.Equal(3, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property04), out value));
			Assert.Equal(4, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property05), out value));
			Assert.Equal(5, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property06), out value));
			Assert.Equal(6, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property07), out value));
			Assert.Equal(7, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property08), out value));
			Assert.Equal(8, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property09), out value));
			Assert.Equal(9, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property10), out value));
			Assert.Equal(10, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property55), out value));
			Assert.Equal(55, value);
			Assert.True(changes.TryGetValue(nameof(employee.Property56), out value));
			Assert.Equal(56, value);

			Assert.False(employee.TryGetValue(NOTEXISTS, out value));
			Assert.False(employee.TrySetValue(NOTEXISTS, null));
		}
		#endregion
	}
}
