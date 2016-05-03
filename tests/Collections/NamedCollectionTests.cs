using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Collections;
using Zongsoft.Tests;

using Xunit;

namespace Zongsoft.Collections.Tests
{
	public class NamedCollectionTests
	{
		#region 私有字段
		private List<Person> _list;
		private NamedCollection<Customer> _customers;
		private NamedCollection<Employee> _employees;
		#endregion

		public NamedCollectionTests()
		{
			this.Initialize();
		}

		public void Initialize()
		{
			_list = new List<Person>(new Person[]
			{
				new Person()
				{
					Name = "Unnamed",
				},
				new Customer()
				{
					Name = "Customer#1",
				},
				new Employee()
				{
					Name = "Employee#1",
				},
				new Employee()
				{
					Name = "Employee#2",
				},
				new Customer()
				{
					Name = "Customer#2",
				},
				new Customer()
				{
					Name = "Customer#3",
				},
			});

			_customers = new NamedCollection<Customer>(_list, p => p.Name, item => item != null && item.GetType() == typeof(Customer));
			_employees = new NamedCollection<Employee>(_list, p => p.Name, item => item != null && item.GetType() == typeof(Employee));
		}

		[Fact]
		public void AddTest()
		{
			var count = _list.Count;

			_customers.Add(new Customer()
			{
				Name = "New Customer",
			});

			Assert.Equal(count + 1, _list.Count);

			_employees.Add(new Employee()
			{
				Name = "New Employee",
			});

			Assert.Equal(count + 2, _list.Count);
		}

		[Fact]
		public void ClearTest()
		{
			var totalCount = _list.Count;

			var count = _customers.Count;
			_customers.Clear();

			Assert.Equal(0, _customers.Count);
			Assert.Equal(totalCount -= count, _list.Count);

			count = _employees.Count;
			_employees.Clear();

			Assert.Equal(0, _employees.Count);
			Assert.Equal(totalCount -= count, _list.Count);

			_customers.Add(new Customer()
			{
				Name = "New Customer",
			});

			Assert.Equal(1, _customers.Count);

			_list.Clear();
			Assert.Equal(0, _customers.Count);
			Assert.Equal(0, _list.Count);
		}

		[Fact]
		public void RemoveTest()
		{
			const int ADD_COUNT = 10;
			const string KEY_PREFIX = "New Key #";

			int count = _customers.Count;

			for(int i = 0; i < ADD_COUNT; i++)
			{
				_customers.Add(new Customer()
				{
					Name = KEY_PREFIX + i.ToString(),
				});
			}

			Assert.Equal(count + ADD_COUNT, _customers.Count);

			for(int i = 0; i < ADD_COUNT; i++)
			{
				Assert.True(_customers.Remove(KEY_PREFIX + i.ToString()));
			}

			Assert.Equal(count, _customers.Count);
		}

		[Fact]
		public void ContainsTest()
		{
			Assert.False(_customers.Contains("NotExists"));
			Assert.True(_customers.Contains("Customer#1"));
			Assert.True(_customers.Contains("Customer#2"));
			Assert.True(_employees.Contains("Employee#1"));
		}

		[Fact]
		public void CopyToTest()
		{
			var customers = new Customer[8];

			_customers.CopyTo(customers, 2);

			Assert.Null(customers[0]);
			Assert.Null(customers[1]);

			Assert.NotNull(customers[2]);
			Assert.Equal("Customer#1", customers[2].Name);
			Assert.NotNull(customers[3]);
			Assert.Equal("Customer#2", customers[3].Name);
			Assert.NotNull(customers[4]);
			Assert.Equal("Customer#3", customers[4].Name);

			Assert.Null(customers[5]);
			Assert.Null(customers[6]);
			Assert.Null(customers[7]);
		}

		[Fact]
		public void GetEnumeratorTest()
		{
			Assert.Equal(6, _list.Count);
			Assert.Equal(3, _customers.Count);
			Assert.Equal(2, _employees.Count);

			foreach(var customer in _customers)
			{
				Assert.NotNull(customer);
			}

			foreach(var employee in _employees)
			{
				Assert.NotNull(employee);
			}
		}
	}
}
