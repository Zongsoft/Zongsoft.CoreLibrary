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

namespace Zongsoft.Security.Membership
{
	/// <summary>
	/// 表示用户的实体类。
	/// </summary>
	[Serializable]
	public class User : Common.ModelBase, IUser, IMember, IEquatable<User>
	{
		#region 静态字段
		public static readonly string Administrator = "Administrator";
		public static readonly string Guest = "Guest";
		#endregion

		#region 成员字段
		private uint _userId;
		private string _name;
		private string _fullName;
		private string _namespace;
		private string _email;
		private string _phoneNumber;
		private UserStatus _status;
		private DateTime? _statusTimestamp;
		private string _description;
		private uint? _creatorId;
		private DateTime _createdTime;
		private uint? _modifierId;
		private DateTime? _modifiedTime;
		#endregion

		#region 构造函数
		public User()
		{
			this.CreatedTime = DateTime.UtcNow;
		}

		public User(string name, string @namespace) : this(0, name, @namespace)
		{
		}

		public User(uint userId, string name) : this(userId, name, null)
		{
		}

		public User(uint userId, string name, string @namespace)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			this.UserId = userId;
			this.Name = this.FullName = name.Trim();
			this.Namespace = @namespace;
			this.CreatedTime = DateTime.UtcNow;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置用户编号。
		/// </summary>
		public uint UserId
		{
			get
			{
				return _userId;
			}
			set
			{
				this.SetPropertyValue(nameof(UserId), ref _userId, value);
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
					if(string.IsNullOrWhiteSpace(this.Name))
						return;

					throw new ArgumentNullException();
				}

				value = value.Trim();

				//用户名的长度必须不少于4个字符
				if(value.Length < 4)
					throw new ArgumentOutOfRangeException($"The '{value}' user name length must be greater than 3.");

				//更新属性内容
				this.SetPropertyValue(nameof(Name), ref _name, value);
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
				this.SetPropertyValue(nameof(FullName), ref _fullName, value);
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
							throw new ArgumentException("The user namespace contains illegal character.");
					}
				}

				//更新属性内容
				this.SetPropertyValue(nameof(Namespace), ref _namespace, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
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
				this.SetPropertyValue(nameof(Description), ref _description, value);
			}
		}

		/// <summary>
		/// 获取或设置用户的电子邮箱地址。
		/// </summary>
		public string Email
		{
			get
			{
				return _email;
			}
			set
			{
				this.SetPropertyValue(nameof(Email), ref _email, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
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
				this.SetPropertyValue(nameof(PhoneNumber), ref _phoneNumber, string.IsNullOrWhiteSpace(value) ? null : value.Trim());
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
				this.SetPropertyValue(nameof(Status), ref _status, value);
			}
		}

		/// <summary>
		/// 获取或设置用户状态的更新时间。
		/// </summary>
		public DateTime? StatusTimestamp
		{
			get
			{
				return _statusTimestamp;
			}
			set
			{
				this.SetPropertyValue(nameof(StatusTimestamp), ref _statusTimestamp, value);
			}
		}

		/// <summary>
		/// 获取或设置当前用户的创建人编号。
		/// </summary>
		public uint? CreatorId
		{
			get
			{
				return _creatorId;
			}
			set
			{
				this.SetPropertyValue(nameof(CreatorId), ref _creatorId, value);
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
				this.SetPropertyValue(nameof(CreatedTime), ref _createdTime, value);
			}
		}

		/// <summary>
		/// 获取或设置用户信息的最后修改人编号。
		/// </summary>
		public uint? ModifierId
		{
			get
			{
				return _modifierId;
			}
			set
			{
				this.SetPropertyValue(nameof(ModifierId), ref _modifierId, value);
			}
		}

		/// <summary>
		/// 获取或设置用户信息的最后修改时间。
		/// </summary>
		public DateTime? ModifiedTime
		{
			get
			{
				return _modifiedTime;
			}
			set
			{
				this.SetPropertyValue(nameof(ModifiedTime), ref _modifiedTime, value);
			}
		}
		#endregion

		#region 导航属性
		/// <summary>
		/// 获取或设置当前用户的创建人。
		/// </summary>
		public User Creator
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置当前用户的最后修改人。
		/// </summary>
		public User Modifier
		{
			get;
			set;
		}
		#endregion

		#region 重写方法
		public bool Equals(User other)
		{
			if(other == null)
				return false;

			return this.UserId == other.UserId &&
			       string.Equals(this.Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase) &&
			       string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return this.Equals((User)obj);
		}

		public override int GetHashCode()
		{
			var userId = this.UserId;

			if(userId != 0)
				return (int)userId;
			else
				return (this.Namespace + ":" + this.Name).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(this.Namespace))
				return $"[{this.UserId.ToString()}]{this.Name}";
			else
				return $"[{this.UserId.ToString()}]{this.Namespace}:{this.Name}";
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
