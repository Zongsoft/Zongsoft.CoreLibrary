using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zongsoft.Tests
{
	public class Person
	{
		public string Name
		{
			get;
			set;
		}

		public Gender? Gender
		{
			get;
			set;
		}

		public Address HomeAddress
		{
			get;
			set;
		}

		public Address OfficeAddress
		{
			get;
			set;
		}
	}

	public class Employee : Person
	{
		/// <summary>
		/// 获取或设置员工编号。
		/// </summary>
		public int EmployeeId
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置员工所属的部门。
		/// </summary>
		public Department Department
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置员工的月薪。
		/// </summary>
		public decimal Salary
		{
			get;
			set;
		}
	}

	public class Customer : Person
	{
		/// <summary>
		/// 获取或设置客户的评级。
		/// </summary>
		public int Level
		{
			get;
			set;
		}
	}

	public class EmployeeCollection : Zongsoft.Collections.NamedCollectionBase<Employee>
	{
		private Department _department;

		public EmployeeCollection(Department department)
		{
			_department = department;
		}

		protected override string GetKeyForItem(Employee item)
		{
			return item.Name;
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if(args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach(Employee item in args.NewItems)
					item.Department = _department;
			}

			base.OnCollectionChanged(args);
		}
	}
}
