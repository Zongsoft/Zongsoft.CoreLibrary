using System;
using System.Collections.Generic;
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
}
