using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Collections;
using Zongsoft.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Collections.Tests
{
	[TestClass]
	public class NamedCollectionTests
	{
		#region 私有字段
		private List<Person> _list;
		private NamedCollection<Customer> _customers;
		private NamedCollection<Employee> _employees;
		#endregion

		[TestInitialize]
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

		[TestMethod()]
		public void AddTest()
		{
			var count = _list.Count;

			_customers.Add(new Customer()
			{
				Name = "New Customer",
			});

			Assert.AreEqual(count + 1, _list.Count);

			_employees.Add(new Employee()
			{
				Name = "New Employee",
			});

			Assert.AreEqual(count + 2, _list.Count);
		}

		[TestMethod()]
		public void ClearTest()
		{
			var totalCount = _list.Count;

			var count = _customers.Count;
			_customers.Clear();

			Assert.AreEqual(0, _customers.Count);
			Assert.AreEqual(totalCount -= count, _list.Count);

			count = _employees.Count;
			_employees.Clear();

			Assert.AreEqual(0, _employees.Count);
			Assert.AreEqual(totalCount -= count, _list.Count);

			_customers.Add(new Customer()
			{
				Name = "New Customer",
			});

			Assert.AreEqual(1, _customers.Count);

			_list.Clear();
			Assert.AreEqual(0, _customers.Count);
			Assert.AreEqual(0, _list.Count);
		}

		[TestMethod()]
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

			Assert.AreEqual(count + ADD_COUNT, _customers.Count);

			for(int i = 0; i < ADD_COUNT; i++)
			{
				Assert.IsTrue(_customers.Remove(KEY_PREFIX + i.ToString()));
			}

			Assert.AreEqual(count, _customers.Count);
		}

		[TestMethod()]
		public void ContainsTest()
		{
			Assert.IsFalse(_customers.Contains("NotExists"));
			Assert.IsTrue(_customers.Contains("Customer#1"));
			Assert.IsTrue(_customers.Contains("Customer#2"));
			Assert.IsTrue(_employees.Contains("Employee#1"));
		}

		[TestMethod()]
		public void CopyToTest()
		{
			var customers = new Customer[8];

			_customers.CopyTo(customers, 2);

			Assert.IsNull(customers[0]);
			Assert.IsNull(customers[1]);

			Assert.IsNotNull(customers[2]);
			Assert.AreEqual("Customer#1", customers[2].Name);
			Assert.IsNotNull(customers[3]);
			Assert.AreEqual("Customer#2", customers[3].Name);
			Assert.IsNotNull(customers[4]);
			Assert.AreEqual("Customer#3", customers[4].Name);

			Assert.IsNull(customers[5]);
			Assert.IsNull(customers[6]);
			Assert.IsNull(customers[7]);
		}

		[TestMethod()]
		public void GetEnumeratorTest()
		{
			Assert.AreEqual(6, _list.Count);
			Assert.AreEqual(3, _customers.Count);
			Assert.AreEqual(2, _employees.Count);

			foreach(var customer in _customers)
			{
				Assert.IsNotNull(customer);
			}

			foreach(var employee in _employees)
			{
				Assert.IsNotNull(employee);
			}
		}
	}
}
