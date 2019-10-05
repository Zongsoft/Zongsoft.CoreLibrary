using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.Samples.Models
{
	public abstract class UserEntityBase : IUserEntity
	{
		public abstract uint UserId
		{
			get;
			set;
		}
		public abstract string Namespace
		{
			get;
			set;
		}
		public abstract string Email
		{
			get;
			set;
		}
		public abstract string PhoneNumber
		{
			get;
			set;
		}
		public abstract string Avatar
		{
			get;
			set;
		}
		public abstract string AvatarUrl
		{
			get;
		}
		public abstract ICollection<object> Assets
		{
			get;
		}
		public abstract byte Status
		{
			get;
			set;
		}
		public abstract DateTime? StatusTimestamp
		{
			get;
			set;
		}
		public abstract string PrincipalId
		{
			get;
			set;
		}
		public abstract DateTime CreatedTime
		{
			get;
			set;
		}
		public abstract string Description
		{
			get;
			set;
		}
		public abstract string Name
		{
			get;
			set;
		}
		public abstract string FullName
		{
			get;
			set;
		}

		public abstract int Count();
		public abstract IDictionary<string, object> GetChanges();
		public abstract bool HasChanges(params string[] names);
		public abstract bool Reset(string name, out object value);
		public abstract void Reset(params string[] names);
		public abstract bool TryGetValue(string name, out object value);
		public abstract bool TrySetValue(string name, object value);
	}

	public class UserEntity : IUserEntity, INotifyPropertyChanged
	{
		#region 静态字段
		private static readonly string[] __NAMES__ = new string[] { "UserId", "Namespace", "Name", "FullName", "Email", "PhoneNumber", "Avatar", "Status", "StatusTimestamp", "PrincipalId", "CreatedTime", "Description" };
		private static readonly Dictionary<string, PropertyToken<UserEntity>> __TOKENS__ = new Dictionary<string, PropertyToken<UserEntity>>()
		{
			{ "UserId", new PropertyToken<UserEntity>(0, target => target._userId, (target, value) => target.UserId = (uint)Convert.ChangeType(value, typeof(uint))) },
			{ "Namespace", new PropertyToken<UserEntity>(1, target => target._namespace, (target, value) => target.Namespace = (string)value) },
			{ "Name", new PropertyToken<UserEntity>(2, target => target._name, (target, value) => target.Name = (string)value) },
			{ "FullName", new PropertyToken<UserEntity>(3, target => target._fullName, (target, value) => target.FullName = (string)value) },
			{ "Email", new PropertyToken<UserEntity>(4, target => target._email, (target, value) => target.Email = (string)value) },
			{ "PhoneNumber", new PropertyToken<UserEntity>(5, target => target._phoneNumber, (target, value) => target.PhoneNumber = (string)value) },
			{ "Avatar", new PropertyToken<UserEntity>(6, target => target._avatar, (target, value) => target.Avatar = (string)value) },
			{ "Status", new PropertyToken<UserEntity>(7, target => target._status, (target, value) => target.Status = (byte)value) },
			{ "StatusTimestamp", new PropertyToken<UserEntity>(8, target => target._statusTimestamp, (target, value) => target.StatusTimestamp = (DateTime?)value) },
			{ "PrincipalId", new PropertyToken<UserEntity>(9, target => target._principalId, (target, value) => target.PrincipalId = (string)value) },
			{ "CreatedTime", new PropertyToken<UserEntity>(10, target => target._createdTime, (target, value) => target.CreatedTime = (DateTime)value) },
			{ "Description", new PropertyToken<UserEntity>(11, target => target._description, (target, value) => target.Description = (string)value) },
		};
		#endregion

		#region 事件定义
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region 标记变量
		private ushort _MASK_;
		private readonly byte[] _flags_;
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
		private ICollection<object> _assets;
		#endregion

		#region 公共属性
		public uint UserId
		{
			get => _userId;
			set
			{
				//if(_userId == value)
				//	return;

				_userId = value;
				_MASK_ |= 1;

				//this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserId)));
			}
		}

		public string Namespace
		{
			get => _namespace;
			set
			{
				//if(_namespace == value)
				//	return;

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

		[Zongsoft.Data.Model.Property(Data.Model.PropertyImplementationMode.Extension, typeof(UserExtension))]
		public string Avatar
		{
			get => _avatar;
			set
			{
				//if(UserExtension.SetAvatar(this, _avatar, value, out var result))
				{
					_avatar = value;
					_MASK_ |= 64;
				}
			}
		}

		public string AvatarUrl
		{
			get => UserExtension.GetAvatarUrl(this);
		}

		ICollection<object> IUserEntity.Assets
		{
			get
			{
				if(_assets == null)
				{
					lock(this)
					{
						if(_assets == null)
							_assets = new List<object>();
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
				//if(object.Equals(_statusTimestamp, value))
				//if(_statusTimestamp == value)
				//	return;

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
				//if(_createdTime == value)
				//	return;

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

				//_flags_[11 / 8] |= (byte)Math.Pow(2, 11 % 8);
			}
		}
		#endregion

		#region 接口方法
		public int Count()
		{
			int count = 0;
			var mask = _MASK_;

			if(_MASK_ == 0)
				return 0;

			//for(int i = 0; i < 64; i++)
			//{
			//	if((_MASK_ >> i & 1) == 1)
			//		count++;
			//}

			if((mask & 1UL) == 1UL) //bit 1
				count++;
			if((mask & 2UL) == 2UL) //bit 2
				count++;
			if((mask & 4UL) == 4UL) //bit 3
				count++;
			if((mask & 8UL) == 8UL) //bit 4
				count++;
			if((mask & 0x10) == 0x10) //bit 5
				count++;
			if((mask & 0x20) == 0x20) //bit 6
				count++;
			if((mask & 0x40) == 0x40) //bit 7
				count++;
			if((mask & 0x80) == 0x80) //bit 8
				count++;
			if((mask & 0x100) == 0x100) //bit 9
				count++;
			if((mask & 0x200) == 0x200) //bit 10
				count++;
			if((mask & 0x400) == 0x400) //bit 11
				count++;
			if((mask & 0x800) == 0x800) //bit 12
				count++;
			if((mask & 0x1000) == 0x1000) //bit 13
				count++;
			if((mask & 0x2000) == 0x2000) //bit 14
				count++;
			if((mask & 0x4000) == 0x4000) //bit 15
				count++;
			if((mask & 0x8000) == 0x8000) //bit 16
				count++;
			//if((mask & 0x10000) == 0x10000) //bit 17
			//	count++;
			//if((mask & 0x20000) == 0x20000) //bit 18
			//	count++;
			//if((mask & 0x40000) == 0x40000) //bit 19
			//	count++;
			//if((mask & 0x80000) == 0x80000) //bit 20
			//	count++;
			//if((mask & 0x100000) == 0x100000) //bit 21
			//	count++;
			//if((mask & 0x200000) == 0x200000) //bit 22
			//	count++;
			//if((mask & 0x400000) == 0x400000) //bit 23
			//	count++;
			//if((mask & 0x800000) == 0x800000) //bit 24
			//	count++;
			//if((mask & 0x1000000) == 0x1000000) //bit 25
			//	count++;
			//if((mask & 0x2000000) == 0x2000000) //bit 26
			//	count++;
			//if((mask & 0x4000000) == 0x4000000) //bit 27
			//	count++;
			//if((mask & 0x8000000) == 0x8000000) //bit 28
			//	count++;
			//if((mask & 0x10000000) == 0x10000000) //bit 29
			//	count++;
			//if((mask & 0x20000000) == 0x20000000) //bit 30
			//	count++;
			//if((mask & 0x40000000) == 0x40000000) //bit 31
			//	count++;
			//if((mask & 0x80000000) == 0x80000000) //bit 32
			//	count++;
			//if((mask & 0x100000000) == 0x100000000) //bit 33
			//	count++;
			//if((mask & 0x200000000) == 0x200000000) //bit 34
			//	count++;
			//if((mask & 0x400000000) == 0x400000000) //bit 35
			//	count++;
			//if((mask & 0x800000000UL) == 0x800000000UL) //bit 36
			//	count++;

			return count;
		}

		public int CountArray()
		{
			int count = 0;
			byte flag = 0;

			for(int i = 0; i < _flags_.Length; i++)
			{
				flag = _flags_[i];

				if(flag == 0)
					continue;

				if((flag & 1) == 1)
					count++;
				if((flag & 2) == 2)
					count++;
				if((flag & 4) == 4)
					count++;
				if((flag & 8) == 8)
					count++;
				if((flag & 16) == 16)
					count++;
				if((flag & 32) == 32)
					count++;
				if((flag & 64) == 64)
					count++;
				if((flag & 128) == 128)
					count++;
			}

			return count;
		}

		public bool Reset(string name, out object value)
		{
			value = null;

			if(name == null || name.Length == 0)
				return false;

			if(__TOKENS__.TryGetValue(name, out var token) && (_MASK_ >> token.Ordinal & 1) == 1)
			{
				value = token.Getter.Invoke(this);
				_MASK_ &= (ushort)~(1 << token.Ordinal);
				return true;
			}

			return false;
		}

		public bool ResetArray(string name, out object value)
		{
			value = null;

			if(name == null || name.Length == 0)
				return false;

			if(__TOKENS__.TryGetValue(name, out var token) && (_flags_[token.Ordinal / 8] >> (token.Ordinal % 8) & 1) == 1)
			{
				value = token.Getter.Invoke(this);
				_flags_[token.Ordinal / 8] = (byte)(_flags_[token.Ordinal / 8] & ~(1 << (token.Ordinal % 8)));
				return true;
			}

			return false;
		}

		public void Reset(params string[] names)
		{
			PropertyToken<UserEntity> token;

			if(names == null || names.Length == 0)
			{
				_MASK_ = 0;
				return;
			}

			for(int i = 0; i < names.Length; i++)
			{
				if(__TOKENS__.TryGetValue(names[i], out token) && (_MASK_ >> token.Ordinal & 1) == 1)
					_MASK_ &= (ushort)~(1 << token.Ordinal);
			}
		}

		public void ResetArray(params string[] names)
		{
			PropertyToken<UserEntity> token;

			if(names == null || names.Length == 0)
			{
				for(int i = 0; i < _flags_.Length; i++)
					_flags_[i] = 0;

				return;
			}

			for(int i = 0; i < names.Length; i++)
			{
				if(__TOKENS__.TryGetValue(names[i], out token) && (_flags_[token.Ordinal / 8] >> (token.Ordinal % 8) & 1) == 1)
					_flags_[token.Ordinal / 8] = (byte)(_flags_[token.Ordinal / 8] & ~(1 << (token.Ordinal % 8)));
			}
		}

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
				if(__TOKENS__.TryGetValue(names[i], out property) && property.Setter != null && (_flags_[property.Ordinal / 8] >> (property.Ordinal % 8) & 1) == 1)
					return true;
			}

			return false;
		}

		bool Zongsoft.Data.IModel.HasChanges(params string[] names)
		{
			PropertyToken<UserEntity> property;

			if(names == null || names.Length == 0)
				return _MASK_ != 0;

			for(var i = 0; i < names.Length; i++)
			{
				if(__TOKENS__.TryGetValue(names[i], out property) && property.Setter != null && (_MASK_ >> property.Ordinal & 1) == 1)
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
					dictionary[__NAMES__[i]] = __TOKENS__[__NAMES__[i]].Getter(this);
				}
			}

			return dictionary.Count == 0 ? null : dictionary;
		}

		IDictionary<string, object> Zongsoft.Data.IModel.GetChanges()
		{
			if(_MASK_ == 0)
				return null;

			var dictionary = new Dictionary<string, object>(__NAMES__.Length);

			for(int i = 0; i < __NAMES__.Length; i++)
			{
				if((_MASK_ >> i & 1) == 1)
				{
					dictionary[__NAMES__[i]] = __TOKENS__[__NAMES__[i]].Getter(this);
				}
			}

			return dictionary;
		}

		private bool TryGet(string name, out object value)
		{
			value = null;

			if(__TOKENS__.TryGetValue(name, out var property) && (property.Ordinal < 0 || (_flags_[property.Ordinal / 8] >> (property.Ordinal % 8) & 1) == 1))
			{
				value = property.Getter(this);
				return true;
			}

			return false;
		}

		bool Zongsoft.Data.IModel.TryGetValue(string name, out object value)
		{
			value = null;

			if(__TOKENS__.TryGetValue(name, out var property) && (property.Ordinal < 0 || (_MASK_ >> property.Ordinal & 1) == 1))
			{
				value = property.Getter(this);
				return true;
			}

			return false;
		}

		bool Zongsoft.Data.IModel.TrySetValue(string name, object value)
		{
			if(__TOKENS__.TryGetValue(name, out var property) && property.Setter != null)
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
