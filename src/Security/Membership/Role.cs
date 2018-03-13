/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class Role : Zongsoft.Common.ModelBase, IMember
	{
		#region 静态字段
		public static readonly string Administrators = "Administrators";
		public static readonly string Securities = "Securities";
		#endregion

		#region 构造函数
		public Role()
		{
			this.CreatedTime = DateTime.Now;
		}

		public Role(string name, string @namespace) : this(0, name, @namespace)
		{
		}

		public Role(uint roleId, string name) : this(roleId, name, null)
		{
		}

		public Role(uint roleId, string name, string @namespace)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			this.RoleId = roleId;
			this.Name = this.FullName = name.Trim();
			this.Namespace = @namespace;
			this.CreatedTime = DateTime.Now;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置成员编号，即角色编号。
		/// </summary>
		uint IMember.MemberId
		{
			get
			{
				return this.RoleId;
			}
			set
			{
				this.RoleId = value;
			}
		}

		/// <summary>
		/// 获取成员类型，始终返回<see cref="MemberType.Role"/>。
		/// </summary>
		MemberType IMember.MemberType
		{
			get
			{
				return MemberType.Role;
			}
		}

		/// <summary>
		/// 获取或设置角色编号。
		/// </summary>
		public uint RoleId
		{
			get
			{
				return this.GetPropertyValue(() => this.RoleId);
			}
			set
			{
				this.SetPropertyValue(() => this.RoleId, value);
			}
		}

		/// <summary>
		/// 获取或设置角色名。
		/// </summary>
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

				//角色名的长度必须不少于4个字符
				if(value.Length < 4)
					throw new ArgumentOutOfRangeException($"The '{value}' role name length must be greater than 3.");

				//更新属性内容
				this.SetPropertyValue(() => this.Name, value);
			}
		}

		/// <summary>
		/// 获取或设置角色的全称。
		/// </summary>
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

		/// <summary>
		/// 获取或设置角色的所属命名空间。
		/// </summary>
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

		/// <summary>
		/// 获取或设置角色的描述信息。
		/// </summary>
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

		/// <summary>
		/// 获取或设置角色的创人编号。
		/// </summary>
		public uint CreatorId
		{
			get
			{
				return this.GetPropertyValue(() => this.CreatorId);
			}
			set
			{
				this.SetPropertyValue(() => this.CreatorId, value);
			}
		}

		/// <summary>
		/// 获取或设置角色的创建时间。
		/// </summary>
		public DateTime CreatedTime
		{
			get
			{
				return this.GetPropertyValue(() => this.CreatedTime);
			}
			set
			{
				this.SetPropertyValue(() => this.CreatedTime, value);
			}
		}

		/// <summary>
		/// 获取或设置角色信息的最后修改人编号。
		/// </summary>
		public uint? ModifierId
		{
			get
			{
				return this.GetPropertyValue(() => this.ModifierId);
			}
			set
			{
				this.SetPropertyValue(() => this.ModifierId, value);
			}
		}

		/// <summary>
		/// 获取或设置角色信息的最后修改时间。
		/// </summary>
		public DateTime? ModifiedTime
		{
			get
			{
				return this.GetPropertyValue(() => this.ModifiedTime);
			}
			set
			{
				this.SetPropertyValue(() => this.ModifiedTime, value);
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (Role)obj;

			return this.RoleId == other.RoleId && string.Equals(this.Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (this.Namespace + ":" + this.RoleId.ToString()).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(this.Namespace))
				return string.Format("[{0}]{1}", this.RoleId.ToString(), this.Name);
			else
				return string.Format("[{0}]{1}@{2}", this.RoleId.ToString(), this.Name, this.Namespace);
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
