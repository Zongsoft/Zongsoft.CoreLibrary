using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	public class UserEntity : IUserEntity
	{
		#region 静态字段
		private static readonly string[] __NAMES__ = new string[] { "UserId", "Namespace", "Name", "FullName", "Email", "PhoneNumber", "Avatar", "Status", "StatusTimestamp", "PrincipalId", "CreatedTime", "Description" };
		private static readonly Dictionary<string, DataEntity.PropertyToken<UserEntity>> __PROPERTIES__ = new Dictionary<string, DataEntity.PropertyToken<UserEntity>>()
		{
			{ "UserId", new DataEntity.PropertyToken<UserEntity>(0, target => target._userId, (target, value) => target.UserId = (uint)value) },
			{ "Namespace", new DataEntity.PropertyToken<UserEntity>(1, target => target._namespace, (target, value) => target.Namespace = (string)value) },
			{ "Name", new DataEntity.PropertyToken<UserEntity>(2, target => target._name, (target, value) => target.Name = (string)value) },
			{ "FullName", new DataEntity.PropertyToken<UserEntity>(3, target => target._fullName, (target, value) => target.FullName = (string)value) },
			{ "Email", new DataEntity.PropertyToken<UserEntity>(4, target => target._email, (target, value) => target.Email = (string)value) },
			{ "PhoneNumber", new DataEntity.PropertyToken<UserEntity>(5, target => target._phoneNumber, (target, value) => target.PhoneNumber = (string)value) },
			{ "Avatar", new DataEntity.PropertyToken<UserEntity>(6, target => target._avatar, (target, value) => target.Avatar = (string)value) },
			{ "Status", new DataEntity.PropertyToken<UserEntity>(7, target => target._status, (target, value) => target.Status = (byte)value) },
			{ "StatusTimestamp", new DataEntity.PropertyToken<UserEntity>(8, target => target._statusTimestamp, (target, value) => target.StatusTimestamp = (DateTime?)value) },
			{ "PrincipalId", new DataEntity.PropertyToken<UserEntity>(9, target => target._principalId, (target, value) => target.PrincipalId = (string)value) },
			{ "CreatedTime", new DataEntity.PropertyToken<UserEntity>(10, target => target._createdTime, (target, value) => target.CreatedTime = (DateTime)value) },
			{ "Description", new DataEntity.PropertyToken<UserEntity>(11, target => target._description, (target, value) => target.Description = (string)value) },
		};
		#endregion

		#region 标记变量
		private ushort _MASK_;
		//private readonly byte[] _flags_;
		#endregion

		#region 构造函数
		public UserEntity()
		{
			//_flags_ = new byte[10];
		}
		#endregion

		#region 成员字段
		private uint _userId;
		private string _namespace;
		private string _name;
		private string _fullName;
		private string _email;
		private string _phoneNumber;
		private string _avatar;
		private byte _status;
		private DateTime? _statusTimestamp;
		private string _principalId;
		private DateTime _createdTime;
		private string _description;
		#endregion

		#region 公共属性
		public uint UserId
		{
			get => _userId;
			set
			{
				_userId = value;
				_MASK_ |= 1;
			}
		}

		public string Namespace
		{
			get => _namespace;
			set
			{
				_namespace = value;
				_MASK_ |= 2;
			}
		}

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				_MASK_ |= 4;
			}
		}

		public string FullName
		{
			get => _fullName;
			set
			{
				_fullName = value;
				_MASK_ |= 8;
			}
		}

		public string Email
		{
			get => _email;
			set
			{
				_email = value;
				_MASK_ |= 16;
			}
		}

		public string PhoneNumber
		{
			get => _phoneNumber;
			set
			{
				_phoneNumber = value;
				_MASK_ |= 32;
			}
		}

		public string Avatar
		{
			get => _avatar;
			set
			{
				_avatar = value;
				_MASK_ |= 64;
			}
		}

		public byte Status
		{
			get => _status;
			set
			{
				_status = value;
				_MASK_ |= 128;
			}
		}

		public DateTime? StatusTimestamp
		{
			get => _statusTimestamp;
			set
			{
				_statusTimestamp = value;
				_MASK_ |= 256;
			}
		}

		public string PrincipalId
		{
			get => _principalId;
			set
			{
				_principalId = value;
				_MASK_ |= 512;
			}
		}

		public DateTime CreatedTime
		{
			get => _createdTime;
			set
			{
				_createdTime = value;
				_MASK_ |= 1024;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
				_MASK_ |= 2048;
				//return;

				//var flag = _flags_[12 / 8];
				//flag |= (byte)Math.Pow(2, 12 % 8);

				//_flags_[12 / 8] |= (byte)Math.Pow(2, 12 % 8);
			}
		}
		#endregion

		#region 接口方法
		bool Zongsoft.Data.IDataEntity.HasChanges(params string[] names)
		{
			if(names == null || names.Length == 0)
				return _MASK_ != 0;

			for(var i = 0; i < names.Length; i++)
			{
				if(__PROPERTIES__.TryGetValue(names[i], out var property) && (_MASK_ >> property.Ordinal & 1) == 1)
					return true;
			}

			return false;
		}

		IDictionary<string, object> Zongsoft.Data.IDataEntity.GetChanges()
		{
			if(_MASK_ == 0)
				return null;

			var dictionary = new Dictionary<string, object>(__NAMES__.Length);

			for(int i = 0; i < __NAMES__.Length; i++)
			{
				if((_MASK_ >> i & 1) == 1)
				{
					dictionary[__NAMES__[i]] = __PROPERTIES__[__NAMES__[i]].Getter(this);
				}
			}

			return dictionary;
		}

		bool Zongsoft.Data.IDataEntity.TryGet(string name, out object value)
		{
			value = null;

			if(__PROPERTIES__.TryGetValue(name, out var property) && (_MASK_ >> property.Ordinal & 1) == 1)
			{
				value = property.Getter(this);
				return true;
			}

			return false;
		}

		bool Zongsoft.Data.IDataEntity.TrySet(string name, object value)
		{
			if(__PROPERTIES__.TryGetValue(name, out var property))
			{
				property.Setter(this, value);
				return true;
			}

			return false;
		}

		private bool TrySet(string name, object value)
		{
			switch(name)
			{
				case "UserId":
					_userId = (uint)value;
					return true;
				case "Namespace":
					_namespace = (string)value;
					return true;
				case "Name":
					_name = (string)value;
					return true;
				case "FullName":
					_fullName = (string)value;
					return true;
				case "Email":
					_email = (string)value;
					return true;
				case "PhoneNumber":
					_phoneNumber = (string)value;
					return true;
				case "Avatar":
					_avatar = (string)value;
					return true;
				case "Status":
					_status = (byte)value;
					return true;
				case "StatusTimestamp":
					_statusTimestamp = (DateTime?)value;
					return true;
				case "PrincipalId":
					_principalId = (string)value;
					return true;
				case "CreatedTime":
					_createdTime = (DateTime)value;
					return true;
				case "Description":
					_description = (string)value;
					return true;
				default:
					return false;
			}
		}
		#endregion

		private static object Create()
		{
			return new UserEntity();
		}
	}
}
