using System;
using System.Collections.Generic;
using System.Linq;

namespace Zongsoft.Tests
{
	public class Department
	{
		#region 成员字段
		private IList<Employee> _employees;
		#endregion

		#region 构造函数
		public Department()
		{
			_employees = new List<Employee>();
		}

		public Department(string name, IEnumerable<Employee> employees = null)
		{
			this.Name = name;

			if(employees == null)
				_employees = new List<Employee>();
			else
				_employees = new List<Employee>(employees);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前部门中指定名称的员工。
		/// </summary>
		/// <param name="name">指定要获取的本部门的员工的姓名。</param>
		/// <returns>返回的员工对象。</returns>
		public Employee this[string name]
		{
			get
			{
				return _employees.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
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
				if(_employees == null || _employees.Count < 1)
					return null;

				return _employees[index];
			}
		}

		/// <summary>
		/// 获取或设置部门的名称。
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		#endregion

		#region 公共方法
		public void AddEmployee(Employee employee)
		{
			if(employee == null)
				throw new ArgumentNullException("employee");

			_employees.Add(employee);
		}

		public void RemoveEmployee(Employee employee)
		{
			if(employee == null)
				return;

			_employees.Remove(employee);
		}
		#endregion
	}
}
