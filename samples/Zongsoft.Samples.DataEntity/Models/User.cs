using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.DataEntity.Models
{
	public class User : IUserEntity, System.ComponentModel.INotifyPropertyChanged
	{
		#region 事件定义
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
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
			set => _userId = value;
		}

		public string Namespace
		{
			get => _namespace;
			set => _namespace = value;
		}

		public string Name
		{
			get => _name;
			set => _name = value;
		}

		public string FullName
		{
			get => _fullName;
			set => _fullName = value;
		}

		public string Email
		{
			get => _email;
			set => _email = value;
		}

		public string PhoneNumber
		{
			get => _phoneNumber;
			set => _phoneNumber = value;
		}

		public string Avatar
		{
			get => _avatar;
			set => _avatar = value;
		}

		public string AvatarUrl
		{
			get => null;
		}

		public byte Status
		{
			get => _status;
			set => _status = value;
		}

		public DateTime? StatusTimestamp
		{
			get => _statusTimestamp;
			set => _statusTimestamp = value;
		}

		public string PrincipalId
		{
			get => _principalId;
			set => _principalId = value;
		}

		public DateTime CreatedTime
		{
			get => _createdTime;
			set => _createdTime = value;
		}

		public string Description
		{
			get => _description;
			set => _description = value;
		}
		#endregion

		#region 公共方法
		public bool HasChanges(params string[] names)
		{
			return false;
		}

		public IDictionary<string, object> GetChanges()
		{
			return null;
		}

		public bool TryGet(string name, out object value)
		{
			value = null;
			return false;
		}

		public bool TrySet(string name, object value)
		{
			return false;
		}
		#endregion
	}
}
