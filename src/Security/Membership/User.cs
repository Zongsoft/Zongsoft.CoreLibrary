/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;

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
		private string _avatar;
		private object _principal;
		private string _principalId;
		private string _email;
		private string _phoneNumber;
		private UserStatus _status;
		private DateTime? _statusTime;
		private DateTime _createdTime;
		#endregion

		#region 构造函数
		public User()
		{
			_status = UserStatus.Unapproved;
			_createdTime = DateTime.Now;
		}

		public User(string name, string @namespace) : this(0, name, @namespace)
		{
		}

		public User(int userId, string name) : this(userId, name, null)
		{
		}

		public User(int userId, string name, string @namespace)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_userId = userId;
			this.Name = _fullName = name.Trim();
			this.Namespace = @namespace;
			_status = UserStatus.Unapproved;
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
				this.SetPropertyValue(() => this.UserId, ref _userId, value);
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
				//首先处理设置值为空或空字符串的情况
				if(string.IsNullOrWhiteSpace(value))
				{
					//如果当前用户名为空，则说明是通过默认构造函数创建，因此现在不用做处理；否则抛出参数空异常
					if(string.IsNullOrWhiteSpace(_name))
						return;

					throw new ArgumentNullException();
				}

				value = value.Trim();

				//用户名的长度必须不少于2个字符
				if(value.Length < 2)
					throw new ArgumentOutOfRangeException();

				//用户名的首字符必须是字母、下划线、美元符
				if(!(Char.IsLetter(value[0]) || value[0] == '_' || value[0] == '$'))
					throw new ArgumentException("Invalid user name.");

				//检查用户名的其余字符的合法性
				for(int i = 1; i < value.Length; i++)
				{
					//用户名的中间字符必须是字母、数字或下划线
					if(!Char.IsLetterOrDigit(value[i]) && value[i] != '_')
						throw new ArgumentException("The user name contains invalid character.");
				}

				//更新属性内容
				this.SetPropertyValue(() => this.Name, ref _name, value);
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
				this.SetPropertyValue(() => this.FullName, ref _fullName, value);
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
				if(!string.IsNullOrWhiteSpace(value))
				{
					value = value.Trim();

					foreach(var chr in value)
					{
						//命名空间的字符必须是字母、数字、下划线或点号组成
						if(!Char.IsLetterOrDigit(chr) && chr != '_' && chr != '.')
							throw new ArgumentException("The user namespace contains invalid character.");
					}
				}

				//更新属性内容
				this.SetPropertyValue(() => this.Namespace, ref _namespace, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
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
				this.SetPropertyValue(() => this.Description, ref _description, value);
			}
		}

		/// <summary>
		/// 获取或设置用户的头像标识(头像代码或头像图片的URL)。
		/// </summary>
		public string Avatar
		{
			get
			{
				return _avatar;
			}
			set
			{
				this.SetPropertyValue(() => this.Avatar, ref _avatar, value);
			}
		}

		/// <summary>
		/// 获取或设置用户对应的主体对象。
		/// </summary>
		public object Principal
		{
			get
			{
				return _principal;
			}
			set
			{
				this.SetPropertyValue(() => this.Principal, ref _principal, value);
			}
		}

		/// <summary>
		/// 获取或设置用户对应的主体标识。
		/// </summary>
		public string PrincipalId
		{
			get
			{
				return _principalId;
			}
			set
			{
				this.SetPropertyValue(() => this.PrincipalId, ref _principalId, value);
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
				if(!string.IsNullOrWhiteSpace(value))
				{
					if(!Text.TextRegular.Web.Email.IsMatch(value))
						throw new ArgumentException("Invalid email format.");
				}

				this.SetPropertyValue(() => this.Email, ref _email, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
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
				this.SetPropertyValue(() => this.PhoneNumber, ref _phoneNumber, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
			}
		}

		/// <summary>
		/// 获取或设置用户的状态。
		/// </summary>
		public UserStatus Status
		{
			get
			{
				return _status;
			}
			set
			{
				if(value == _status)
					return;

				this.SetPropertyValue(() => this.Status, ref _status, value);

				//同步设置状态更新时间戳
				_statusTime = DateTime.Now;
			}
		}

		/// <summary>
		/// 获取或设置用户状态的更新时间。
		/// </summary>
		public DateTime? StatusTime
		{
			get
			{
				return _statusTime;
			}
			set
			{
				this.SetPropertyValue(() => this.StatusTime, ref _statusTime, value);
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
				this.SetPropertyValue(() => this.CreatedTime, ref _createdTime, value);
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
		public static bool IsBuiltin(User user)
		{
			if(user == null)
				return false;

			return IsBuiltin(user.Name);
		}

		public static bool IsBuiltin(string userName)
		{
			return string.Equals(userName, User.Administrator, StringComparison.OrdinalIgnoreCase) ||
			       string.Equals(userName, User.Guest, StringComparison.OrdinalIgnoreCase);
		}
		#endregion
	}
}
