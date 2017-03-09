/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2014 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.CoreLibrary.
 *
 * Zongsoft.CoreLibrary is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.CoreLibrary; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.Collections.Generic;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 表示角色的实体类。
	/// </summary>
	[Serializable]
	public class Role : Zongsoft.ComponentModel.NotifyObject
	{
		#region 静态字段
		public static readonly string Administrators = "Administrators";
		public static readonly string Securities = "Securities";
		#endregion

		#region 成员字段
		private int _roleId;
		private DateTime _createdTime;
		#endregion

		#region 构造函数
		public Role()
		{
			_createdTime = DateTime.Now;
		}

		public Role(string name, string @namespace) : this(0, name, @namespace)
		{
		}

		public Role(int roleId, string name) : this(roleId, name, null)
		{
		}

		public Role(int roleId, string name, string @namespace)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_roleId = roleId;
			this.Name = this.FullName = name.Trim();
			this.Namespace = @namespace;
			_createdTime = DateTime.Now;
		}
		#endregion

		#region 公共属性
		public int RoleId
		{
			get
			{
				return _roleId;
			}
			set
			{
				this.SetPropertyValue(() => this.RoleId, ref _roleId, value);
			}
		}

		public string Name
		{
			get
			{
				return this.GetPropertyValue(() => this.Name);
			}
			set
			{
				//首先处理设置值为空或空字符串的情况
				if(string.IsNullOrWhiteSpace(value))
				{
					//如果当前角色名为空，则说明是通过默认构造函数创建，因此现在不用做处理；否则抛出参数空异常
					if(string.IsNullOrWhiteSpace(this.Name))
						return;

					throw new ArgumentNullException();
				}

				value = value.Trim();

				//角色名的长度必须不少于2个字符
				if(value.Length < 2)
					throw new ArgumentOutOfRangeException();

				//角色名的首字符必须是字母、下划线、美元符
				if(!(Char.IsLetter(value[0]) || value[0] == '_' || value[0] == '$'))
					throw new ArgumentException("Invalid role name.");

				//检查角色名的其余字符的合法性
				for(int i = 1; i < value.Length; i++)
				{
					//角色名的中间字符必须是字母、数字或下划线
					if(!Char.IsLetterOrDigit(value[i]) && value[i] != '_')
						throw new ArgumentException("The role name contains invalid character.");
				}

				//更新属性内容
				this.SetPropertyValue(() => this.Name, value);
			}
		}

		public string FullName
		{
			get
			{
				return this.GetPropertyValue(() => this.FullName);
			}
			set
			{
				this.SetPropertyValue(() => this.FullName, value);
			}
		}

		public string Namespace
		{
			get
			{
				return this.GetPropertyValue(() => this.Namespace);
			}
			set
			{
				if(!string.IsNullOrWhiteSpace(value))
				{
					value = value.Trim();

					foreach(var chr in value)
					{
						//命名空间的字符必须是字母、数字、下划线或点号组成
						if(!Char.IsLetterOrDigit(chr) && chr != '_' && chr != '.')
							throw new ArgumentException("The role namespace contains invalid character.");
					}
				}

				//更新属性内容
				this.SetPropertyValue(() => this.Namespace, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
			}
		}

		public string Description
		{
			get
			{
				return this.GetPropertyValue(() => this.Description);
			}
			set
			{
				this.SetPropertyValue(() => this.Description, value);
			}
		}

		public DateTime CreatedTime
		{
			get
			{
				return _createdTime;
			}
			set
			{
				this.SetPropertyValue(() => this.CreatedTime, ref _createdTime, value);
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (Role)obj;

			return _roleId == other._roleId && string.Equals(this.Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (this.Namespace + ":" + _roleId.ToString()).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(this.Namespace))
				return string.Format("[{0}]{1}", _roleId.ToString(), this.Name);
			else
				return string.Format("[{0}]{1}@{2}", _roleId.ToString(), this.Name, this.Namespace);
		}
		#endregion

		#region 静态方法
		public static bool IsBuiltin(Role role)
		{
			if(role == null)
				return false;

			return IsBuiltin(role.Name);
		}

		public static bool IsBuiltin(string roleName)
		{
			return string.Equals(roleName, Role.Administrators, StringComparison.OrdinalIgnoreCase) ||
			       string.Equals(roleName, Role.Securities, StringComparison.OrdinalIgnoreCase);
		}
		#endregion
	}
}
