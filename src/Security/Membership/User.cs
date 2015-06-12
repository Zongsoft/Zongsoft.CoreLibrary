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
using System.Data;
using System.Collections.Generic;

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 表示用户的实体类。
	/// </summary>
	[Serializable]
	public class User : Zongsoft.ComponentModel.NotifyObject
	{
		#region 静态字段
		public static readonly string Administrator = "Administrator";
		public static readonly string Guest = "Guest";
		#endregion

		#region 成员字段
		private int _userId;
		private string _name;
		private string _fullName;
		private string _description;
		private string _namespace;
		private string _principal;
		private string _email;
		private string _phoneNumber;
		private bool _approved;
		private bool _suspended;
		private bool _changePasswordOnFirstTime;
		private byte _maxInvalidPasswordAttempts;
		private byte _minRequiredPasswordLength;
		private int _passwordAttemptWindow;
		private DateTime _passwordExpires;
		private DateTime _createdTime;
		private DateTime? _approvedTime;
		private DateTime? _suspendedTime;
		#endregion

		#region 构造函数
		public User(int userId, string name) : this(userId, name, null)
		{
		}

		public User(int userId, string name, string @namespace)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_userId = userId;
			_name = name.Trim();
			_namespace = @namespace;
			_createdTime = DateTime.Now;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置用户编号。
		/// </summary>
		public int UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				this.SetValue(ref _userId, value, () => this.UserId);
			}
		}

		/// <summary>
		/// 获取或设置用户名。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				this.SetValue(ref _name, value.Trim(), () => this.Name);
			}
		}

		/// <summary>
		/// 获取或设置用户全称。
		/// </summary>
		public string FullName
		{
			get
			{
				return _fullName;
			}
			set
			{
				this.SetValue(ref _fullName, value, () => this.FullName);
			}
		}

		/// <summary>
		/// 获取或设置当前用户所属的命名空间。
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				this.SetValue(ref _namespace, value, () => this.Namespace);
			}
		}

		/// <summary>
		/// 获取或设置对用户的描述文本。
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				this.SetValue(ref _description, value, () => this.Description);
			}
		}

		/// <summary>
		/// 获取或设置用户对应的主体标识。
		/// </summary>
		public string Principal
		{
			get
			{
				return _principal;
			}
			set
			{
				this.SetValue(ref _principal, value, () => this.Principal);
			}
		}

		/// <summary>
		/// 获取或设置用户的电子邮箱地址，邮箱地址不允许重复。
		/// </summary>
		public string Email
		{
			get
			{
				return _email;
			}
			set
			{
				this.SetValue(ref _email, value, () => this.Email);
			}
		}

		/// <summary>
		/// 获取或设置用户的手机号码。
		/// </summary>
		public string PhoneNumber
		{
			get
			{
				return _phoneNumber;
			}
			set
			{
				this.SetValue(ref _phoneNumber, value, () => this.PhoneNumber);
			}
		}

		/// <summary>
		/// 获取或设置用户是否被禁用标志。
		/// </summary>
		public bool Suspended
		{
			get
			{
				return _suspended;
			}
			set
			{
				this.SetValue(ref _suspended, value, () => this.Suspended);
			}
		}

		/// <summary>
		/// 获取或设置用户是否已被审核通过。
		/// </summary>
		public bool Approved
		{
			get
			{
				return _approved;
			}
			set
			{
				this.SetValue(ref _approved, value, () => this.Approved);
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示当该用户被首次使用时是否必须修改密码。
		/// </summary>
		public bool ChangePasswordOnFirstTime
		{
			get
			{
				return _changePasswordOnFirstTime;
			}
			set
			{
				this.SetValue(ref _changePasswordOnFirstTime, value, () => this.ChangePasswordOnFirstTime);
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示该用户密码验证失败允许尝试的最大次数。
		/// </summary>
		public byte MaxInvalidPasswordAttempts
		{
			get
			{
				return _maxInvalidPasswordAttempts;
			}
			set
			{
				this.SetValue(ref _maxInvalidPasswordAttempts, value, () => this.MaxInvalidPasswordAttempts);
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示该用户设置密码的最小长度。
		/// </summary>
		public byte MinRequiredPasswordLength
		{
			get
			{
				return _minRequiredPasswordLength;
			}
			set
			{
				this.SetValue(ref _minRequiredPasswordLength, value, () => this.MinRequiredPasswordLength);
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示当该用户密码验证失败次数达到<see cref="MaxInvalidPasswordAttempts"/>属性指定的数值后，再次进行密码验证的间隔时长(单位：秒)。
		/// </summary>
		public int PasswordAttemptWindow
		{
			get
			{
				return _passwordAttemptWindow;
			}
			set
			{
				this.SetValue(ref _passwordAttemptWindow, value, () => this.PasswordAttemptWindow);
			}
		}

		/// <summary>
		/// 获取或设置当前用户密码的过期时间。
		/// </summary>
		public DateTime PasswordExpires
		{
			get
			{
				return _passwordExpires;
			}
			set
			{
				this.SetValue(ref _passwordExpires, value, () => this.PasswordExpires);
			}
		}

		/// <summary>
		/// 获取或设置当前用户的创建时间。
		/// </summary>
		public DateTime CreatedTime
		{
			get
			{
				return _createdTime;
			}
			set
			{
				this.SetValue(ref _createdTime, value, () => this.CreatedTime);
			}
		}

		/// <summary>
		/// 获取或设置当前用户被审核通过的时间。
		/// </summary>
		public DateTime? ApprovedTime
		{
			get
			{
				return _approvedTime;
			}
			set
			{
				this.SetValue(ref _approvedTime, value, () => this.ApprovedTime);
			}
		}

		/// <summary>
		/// 获取或设置当前用户被禁用的时间。
		/// </summary>
		public DateTime? SuspendedTime
		{
			get
			{
				return _suspendedTime;
			}
			set
			{
				this.SetValue(ref _suspendedTime, value, () => this.SuspendedTime);
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (User)obj;

			return _userId == other._userId && string.Equals(_namespace, other._namespace, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (_namespace + ":" + _userId).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_namespace))
				return string.Format("[{0}]{1}", _userId, _name);
			else
				return string.Format("[{0}]{1}@{2}", _userId, _name, _namespace);
		}
		#endregion

		#region 静态方法
		public static bool IsFixedUser(User user)
		{
			if(user == null)
				return false;

			return IsFixedUser(user.Name);
		}

		public static bool IsFixedUser(string userName)
		{
			return string.Equals(userName, User.Administrator, StringComparison.OrdinalIgnoreCase) ||
			       string.Equals(userName, User.Guest, StringComparison.OrdinalIgnoreCase);
		}
		#endregion
	}
}
