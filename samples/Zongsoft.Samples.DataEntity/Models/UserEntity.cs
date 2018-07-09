using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	public class UserEntity : IUserEntity, System.ComponentModel.INotifyPropertyChanged
	{
		#region 静态字段
		private static readonly string[] __NAMES__ = new string[] { "UserId", "Namespace", "Name", "FullName", "Email", "PhoneNumber", "Avatar", "Status", "StatusTimestamp", "PrincipalId", "CreatedTime", "Description" };
		private static readonly Dictionary<string, PropertyToken<UserEntity>> __PROPERTIES__ = new Dictionary<string, PropertyToken<UserEntity>>()
		{
			{ "UserId", new PropertyToken<UserEntity>(0, target => target._userId, (target, value) => target.UserId = (uint) value) },
			{ "Namespace", new PropertyToken<UserEntity>(1, target => target._namespace, (target, value) => target.Namespace = (string) value) },
			{ "Name", new PropertyToken<UserEntity>(2, target => target._name, (target, value) => target.Name = (string) value) },
			{ "FullName", new PropertyToken<UserEntity>(3, target => target._fullName, (target, value) => target.FullName = (string) value) },
			{ "Email", new PropertyToken<UserEntity>(4, target => target._email, (target, value) => target.Email = (string) value) },
			{ "PhoneNumber", new PropertyToken<UserEntity>(5, target => target._phoneNumber, (target, value) => target.PhoneNumber = (string) value) },
			{ "Avatar", new PropertyToken<UserEntity>(6, target => target._avatar, (target, value) => target.Avatar = (string) value) },
			{ "Status", new PropertyToken<UserEntity>(7, target => target._status, (target, value) => target.Status = (byte) value) },
			{ "StatusTimestamp", new PropertyToken<UserEntity>(8, target => target._statusTimestamp, (target, value) => target.StatusTimestamp = (DateTime?) value) },
			{ "PrincipalId", new PropertyToken<UserEntity>(9, target => target._principalId, (target, value) => target.PrincipalId = (string) value) },
			{ "CreatedTime", new PropertyToken<UserEntity>(10, target => target._createdTime, (target, value) => target.CreatedTime = (DateTime) value) },
			{ "Description", new PropertyToken<UserEntity>(11, target => target._description, (target, value) => target.Description = (string) value) },
		};
		#endregion

		#region 事件定义
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region 标记变量
		private ulong _MASK_;
		private readonly byte[] _flags_;
		#endregion

		#region 构造函数
		public UserEntity()
		{
			_flags_ = new byte[10];
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
		private ICollection<string> _assets;
		#endregion

		#region 公共属性
		public uint UserId
		{
			get => _userId;
			set
			{
				if(_userId == value)
					return;

				_userId = value;
				_MASK_ |= 1;

				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserId)));
			}
		}

		public string Namespace
		{
			get => _namespace;
			set
			{
				if(_namespace == value)
					return;

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

		public string AvatarUrl
		{
			get => UserExtension.GetAvatarUrl(this);
		}

		public ICollection<string> Assets
		{
			get
			{
				if(_assets == null)
				{
					lock(this)
					{
						if(_assets == null)
							_assets = new List<string>();
					}
				}

				return _assets;
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
				if(object.Equals(_statusTimestamp, value))
				//if(_statusTimestamp == value)
					return;

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
				if(_createdTime == value)
					return;

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
				//_MASK_ |= 2048;
				//return;

				//var flag = _flags_[11 / 8];
				//flag |= (byte)Math.Pow(2, 11 % 8);

				_flags_[11 / 8] |= (byte)Math.Pow(2, 11 % 8);
			}
		}
		#endregion

		#region 接口方法
		private bool HasChanges(params string[] names)
		{
			PropertyToken<UserEntity> property;

			if(names == null || names.Length == 0)
			{
				for(int i = 0; i < _flags_.Length; i++)
				{
					if(_flags_[i] != 0)
						return true;
				}

				return false;
			}

			for(var i = 0; i < names.Length; i++)
			{
				if(__PROPERTIES__.TryGetValue(names[i], out property) && property.Setter != null && (_flags_[property.Ordinal / 8] >> (property.Ordinal % 8) & 1) == 1)
					return true;
			}

			return false;
		}

		bool Zongsoft.Data.IEntity.HasChanges(params string[] names)
		{
			PropertyToken<UserEntity> property;

			if(names == null || names.Length == 0)
				return _MASK_ != 0;

			for(var i = 0; i < names.Length; i++)
			{
				if(__PROPERTIES__.TryGetValue(names[i], out property) && property.Setter != null && (_MASK_ >> property.Ordinal & 1) == 1)
					return true;
			}

			return false;
		}

		private IDictionary<string, object> GetChanges()
		{
			var dictionary = new Dictionary<string, object>(__NAMES__.Length);

			for(int i = 0; i < __NAMES__.Length; i++)
			{
				if((_flags_[i / 8] >> (i % 8) & 1) == 1)
				{
					dictionary[__NAMES__[i]] = __PROPERTIES__[__NAMES__[i]].Getter(this);
				}
			}

			return dictionary;
		}

		IDictionary<string, object> Zongsoft.Data.IEntity.GetChanges()
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

		private bool TryGet(string name, out object value)
		{
			value = null;

			if(__PROPERTIES__.TryGetValue(name, out var property) && (property.Ordinal < 0 || (_flags_[property.Ordinal / 8] >> (property.Ordinal % 8) & 1) == 1))
			{
				value = property.Getter(this);
				return true;
			}

			return false;
		}

		bool Zongsoft.Data.IEntity.TryGetValue(string name, out object value)
		{
			value = null;

			if(__PROPERTIES__.TryGetValue(name, out var property) && (property.Ordinal < 0 || (_MASK_ >> property.Ordinal & 1) == 1))
			{
				value = property.Getter(this);
				return true;
			}

			return false;
		}

		bool Zongsoft.Data.IEntity.TrySetValue(string name, object value)
		{
			if(__PROPERTIES__.TryGetValue(name, out var property))
			{
				property.Setter(this, value);
				return true;
			}

			return false;
		}

		private bool TrySetValue(string name, object value)
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

		private static class Anonymous
		{
			private static uint GetUserId(UserEntity target)
			{
				return target.UserId;
			}

			private static void SetUserId(UserEntity target, uint value)
			{
				target.UserId = value;
			}

			private static byte GetStatus(object target)
			{
				return ((UserEntity)target)._status;
			}

			private static void SetStatus(object target, object value)
			{
				((UserEntity)target).Status = (byte)value;
			}
		}
	}
}
