using System;
using System.Collections.Generic;

namespace Zongsoft.Samples.Models
{
	public abstract class EmployeeBase : IEmployee
	{
		protected EmployeeBase()
		{
		}

		public abstract bool? Gender
		{
			get;
			set;
		}
		public abstract DateTime Birthdate
		{
			get;
			set;
		}
		public abstract decimal Salary
		{
			get;
			set;
		}
		public abstract ushort? DepartmentId
		{
			get;
			set;
		}
		public abstract int Property17
		{
			get;
			set;
		}
		public abstract int Property18
		{
			get;
			set;
		}
		public abstract int Property19
		{
			get;
			set;
		}
		public abstract int Property20
		{
			get;
			set;
		}
		public abstract int Property21
		{
			get;
			set;
		}
		public abstract int Property22
		{
			get;
			set;
		}
		public abstract int Property23
		{
			get;
			set;
		}
		public abstract int Property24
		{
			get;
			set;
		}
		public abstract int Property25
		{
			get;
			set;
		}
		public abstract int Property26
		{
			get;
			set;
		}
		public abstract int Property27
		{
			get;
			set;
		}
		public abstract int Property28
		{
			get;
			set;
		}
		public abstract int Property29
		{
			get;
			set;
		}
		public abstract int Property30
		{
			get;
			set;
		}
		public abstract int Property31
		{
			get;
			set;
		}
		public abstract int Property32
		{
			get;
			set;
		}
		public abstract int Property33
		{
			get;
			set;
		}
		public abstract int Property34
		{
			get;
			set;
		}
		public abstract int Property35
		{
			get;
			set;
		}
		public abstract int Property36
		{
			get;
			set;
		}
		public abstract int Property37
		{
			get;
			set;
		}
		public abstract int Property38
		{
			get;
			set;
		}
		public abstract int Property39
		{
			get;
			set;
		}
		public abstract int Property40
		{
			get;
			set;
		}
		public abstract int Property41
		{
			get;
			set;
		}
		public abstract int Property42
		{
			get;
			set;
		}
		public abstract int Property43
		{
			get;
			set;
		}
		public abstract int Property44
		{
			get;
			set;
		}
		public abstract int Property45
		{
			get;
			set;
		}
		public abstract int Property46
		{
			get;
			set;
		}
		public abstract int Property47
		{
			get;
			set;
		}
		public abstract int Property48
		{
			get;
			set;
		}
		public abstract int Property49
		{
			get;
			set;
		}
		public abstract int Property50
		{
			get;
			set;
		}
		public abstract int Property51
		{
			get;
			set;
		}
		public abstract int Property52
		{
			get;
			set;
		}
		public abstract int Property53
		{
			get;
			set;
		}
		public abstract int Property54
		{
			get;
			set;
		}
		public abstract int Property55
		{
			get;
			set;
		}
		public abstract int Property56
		{
			get;
			set;
		}
		public abstract int Property57
		{
			get;
			set;
		}
		public abstract int Property58
		{
			get;
			set;
		}
		public abstract int Property59
		{
			get;
			set;
		}
		public abstract int Property60
		{
			get;
			set;
		}
		public abstract int Property61
		{
			get;
			set;
		}
		public abstract int Property62
		{
			get;
			set;
		}
		public abstract int Property63
		{
			get;
			set;
		}
		public abstract int Property64
		{
			get;
			set;
		}
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
		public string AvatarUrl
		{
			get => string.IsNullOrEmpty(this.Avatar) ? null : "http://" + this.Avatar;
		}
		[Zongsoft.Data.Model.Property(Data.Model.PropertyImplementationMode.Singleton, typeof(UserExtension))]
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
}
