/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的上下文基类。
	/// </summary>
	public abstract class DataAccessContextBase : IDataAccessContextBase, System.ComponentModel.INotifyPropertyChanged
	{
		#region 事件定义
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<DataAccessErrorEventArgs> Error;
		#endregion

		#region 成员字段
		private string _name;
		private DataAccessMethod _method;
		private IDataAccess _dataAccess;
		private IDictionary<string, object> _states;
		#endregion

		#region 构造函数
		protected DataAccessContextBase(IDataAccess dataAccess, string name, DataAccessMethod method, object state)
		{
			if(dataAccess == null)
				throw new ArgumentNullException(nameof(dataAccess));

			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name;
			_method = method;
			_dataAccess = dataAccess;

			if(state != null)
				_states = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { { string.Empty, state } };
		}

		protected DataAccessContextBase(IDataAccess dataAccess, string name, DataAccessMethod method, IDictionary<string, object> states)
		{
			if(dataAccess == null)
				throw new ArgumentNullException(nameof(dataAccess));

			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			_name = name;
			_method = method;
			_dataAccess = dataAccess;

			if(states != null && states.Count > 0)
				_states = new Dictionary<string, object>(states, StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// 获取数据访问的方法。
		/// </summary>
		public DataAccessMethod Method
		{
			get
			{
				return _method;
			}
		}

		/// <summary>
		/// 获取当前上下文关联的数据访问器。
		/// </summary>
		public IDataAccess DataAccess
		{
			get
			{
				return _dataAccess;
			}
		}

		/// <summary>
		/// 获取当前上下文关联的用户主体。
		/// </summary>
		public Zongsoft.Security.CredentialPrincipal Principal
		{
			get
			{
				return Services.ApplicationContext.Current.Principal as Zongsoft.Security.CredentialPrincipal;
			}
		}

		/// <summary>
		/// 获取一个值，指示当前上下文是否含有附加的状态数据。
		/// </summary>
		public bool HasStates
		{
			get
			{
				return _states != null && _states.Count > 0;
			}
		}

		/// <summary>
		/// 获取当前上下文的附加状态数据集。
		/// </summary>
		public IDictionary<string, object> States
		{
			get
			{
				if(_states == null)
					System.Threading.Interlocked.CompareExchange(ref _states, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _states;
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual DataAccessErrorEventArgs OnError(Exception exception)
		{
			var error = this.Error;

			if(error != null)
			{
				var args = new DataAccessErrorEventArgs(this, exception);
				error(this, args);
				return args;
			}

			return null;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
