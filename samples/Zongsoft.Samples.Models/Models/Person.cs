using System;
using System.ComponentModel;
using System.Collections.Generic;

using Zongsoft.Data;

namespace Zongsoft.Samples.Models
{
	public abstract class Person : Zongsoft.Data.IModel, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public abstract string Name
		{
			get; protected set;
		}

		public abstract string FullName
		{
			get; set;
		}

		//protected void OnPropertyChanged(string propertyName)
		//{
		//	this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		//}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			this.PropertyChanged?.Invoke(this, args);
		}

		//protected abstract int GetCount();
		//protected abstract IDictionary<string, object> GetChanges();
		//protected abstract bool HasChanges(string[] names);
		//protected abstract bool Reset(string name, out object value);
		//protected abstract void Reset(string[] names);
		//protected abstract bool TryGetValue(string name, out object value);
		//protected abstract bool TrySetValue(string name, object value);

		#region 显式实现
		int IModel.GetCount()
		{
			//return this.Count();
			throw new NotImplementedException();
		}

		IDictionary<string, object> IModel.GetChanges()
		{
			//return this.GetChanges();
			throw new NotImplementedException();
		}

		bool IModel.HasChanges(params string[] names)
		{
			//return this.HasChanges(names);
			throw new NotImplementedException();
		}

		bool IModel.Reset(string name, out object value)
		{
			//return this.Reset(name, out value);
			throw new NotImplementedException();
		}

		void IModel.Reset(params string[] names)
		{
			//this.Reset(names);
			throw new NotImplementedException();
		}

		bool IModel.TryGetValue(string name, out object value)
		{
			//return this.TryGetValue(name, out value);
			throw new NotImplementedException();
		}

		bool IModel.TrySetValue(string name, object value)
		{
			//return this.TrySetValue(name, value);
			throw new NotImplementedException();
		}
		#endregion
	}

	public sealed class PersonEx : Person
	{
		private readonly object _syncRoot;

		public PersonEx()
		{
			_syncRoot = new object();
		}

		public override string Name
		{
			get => throw new NotImplementedException();
			protected set => throw new NotImplementedException();
		}

		public override string FullName
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
