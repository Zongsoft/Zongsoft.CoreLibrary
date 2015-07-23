/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;
using System.ComponentModel;

namespace Zongsoft.Services.Composition
{
	public abstract class ExecutionHandlerBase : MarshalByRefObject, IExecutionHandler, INotifyPropertyChanged
	{
		#region 事件定义
		public event EventHandler EnabledChanged;
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<ExecutionPipelineExecutedEventArgs> Executed;
		public event EventHandler<ExecutionPipelineExecutingEventArgs> Executing;
		#endregion

		#region 成员字段
		private string _name;
		private bool _enabled;
		private IPredication _predication;
		private ExecutionFilterCollection _filters;
		#endregion

		#region 构造函数
		protected ExecutionHandlerBase()
		{
			_name = this.GetType().Name;
		}

		protected ExecutionHandlerBase(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置处理程序的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			protected set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				if(string.Equals(_name, value.Trim(), StringComparison.Ordinal))
					return;

				_name = value.Trim();

				//激发“PropertyChanged”事件
				this.OnPropertyChanged("Name");
			}
		}

		/// <summary>
		/// 获取或设置当前处理程序的断言对象，该断言决定处理程序是否可用。
		/// </summary>
		public IPredication Predication
		{
			get
			{
				return _predication;
			}
			set
			{
				if(_predication == value)
					return;

				_predication = value;

				//激发“PropertyChanged”事件
				this.OnPropertyChanged("Predication");
			}
		}

		/// <summary>
		/// 获取或设置当前处理程序是否可用。
		/// </summary>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if(_enabled == value)
					return;

				_enabled = value;

				//激发“EnabledChanged”事件
				this.OnEnabledChanged(EventArgs.Empty);

				//激发“PropertyChanged”事件
				this.OnPropertyChanged("Enabled");
			}
		}

		public ExecutionFilterCollection Filters
		{
			get
			{
				if(_filters == null)
					System.Threading.Interlocked.CompareExchange(ref _filters, new ExecutionFilterCollection(), null);

				return _filters;
			}
		}
		#endregion

		#region 公共方法
		public virtual bool CanHandle(IExecutionPipelineContext context)
		{
			//如果断言对象是空则返回是否可用变量的值
			if(_predication == null)
				return _enabled;

			//返回断言对象的断言测试的值
			return _enabled && _predication.Predicate(context);
		}

		public void Handle(IExecutionPipelineContext context)
		{
			//在执行之前首先判断是否可以执行
			if(!this.CanHandle(context))
				return;

			//创建“Executing”事件参数
			var executingArgs = new ExecutionPipelineExecutingEventArgs(context);

			//激发“Executing”事件
			this.OnExecuting(executingArgs);

			if(executingArgs.Cancel)
				return;

			//执行过滤器的前半截
			var filters = ExecutionUtility.InvokeFiltersExecuting(_filters, filter => filter.OnExecuting(context));

			//执行当前处理请求
			this.OnExecute(context);

			//执行过滤器的后半截
			ExecutionUtility.InvokeFiltersExecuted(filters, filter => filter.OnExecuted(context));

			//激发“Executed”事件
			this.OnExecuted(new ExecutionPipelineExecutedEventArgs(context));
		}
		#endregion

		#region 事件激发
		protected virtual void OnEnabledChanged(EventArgs e)
		{
			var enabledChanged = this.EnabledChanged;

			if(enabledChanged != null)
				enabledChanged(this, e);
		}

		protected virtual void OnExecuted(ExecutionPipelineExecutedEventArgs e)
		{
			var executed = this.Executed;

			if(executed != null)
				executed(this, e);
		}

		protected virtual void OnExecuting(ExecutionPipelineExecutingEventArgs e)
		{
			var executing = this.Executing;

			if(executing != null)
				executing(this, e);
		}

		protected void OnPropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var propertyChanged = this.PropertyChanged;

			if(propertyChanged != null)
				propertyChanged(this, e);
		}
		#endregion

		#region 抽象方法
		protected abstract void OnExecute(IExecutionPipelineContext context);
		#endregion
	}
}
