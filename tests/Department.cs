using System;
using System.Collections.Generic;
using System.Linq;

namespace Zongsoft.Tests
{
	public class Department
	{
		#region 成员字段
		private string _name;
		private EmployeeCollection _employees;
		#endregion

		#region 构造函数
		public Department()
		{
			_employees = new EmployeeCollection(this);
		}

		public Department(string name, IEnumerable<Employee> employees = null)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name;
			_employees = new EmployeeCollection(this);

			if(employees != null)
				_employees.AddRange(employees);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置部门的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				_name = value;
			}
		}

		/// <summary>
		/// 获取当前部门中指定名称的员工。
		/// </summary>
		/// <param name="name">指定要获取的本部门的员工的姓名。</param>
		/// <returns>返回的员工对象。</returns>
		public Employee this[string name]
		{
			get
			{
				return _employees[name];
			}
		}

		/// <summary>
		/// 获取当前部门中指定序号的员工。
		/// </summary>
		/// <param name="index">指定的要获取的员工所在本部门的序号。</param>
		/// <returns>返回的员工对象。</returns>
		public Employee this[int index]
		{
			get
			{
				return _employees[index];
			}
		}

		/// <summary>
		/// 获取当前部门的员工集合。
		/// </summary>
		public EmployeeCollection Employees
		{
			get
			{
				return _employees;
			}
		}
		#endregion
	}
}
