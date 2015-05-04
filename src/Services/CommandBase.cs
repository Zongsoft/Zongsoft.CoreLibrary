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
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	/// <summary>
	/// 提供实现<see cref="ICommand"/>接口功能的基类，建议需要完成<see cref="ICommand"/>接口功能的实现者从此类继承。
	/// </summary>
	/// <typeparam name="TContext">指定命令的执行上下文类型。</typeparam>
	public abstract class CommandBase<TContext> : MarshalByRefObject, ICommand<TContext>, IPredication<TContext>, IMatchable, INotifyPropertyChanged where TContext : CommandContextBase
	{
		#region 事件定义
		public event EventHandler EnabledChanged;
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler<CommandExecutedEventArgs> Executed;
		public event EventHandler<CommandExecutingEventArgs> Executing;
		#endregion

		#region 成员字段
		private string _name;
		private bool _enabled;
		private IPredication _predication;
		#endregion

		#region 构造函数
		protected CommandBase() : this(null, true)
		{
		}

		protected CommandBase(string name) : this(name, true)
		{
		}

		protected CommandBase(string name, bool enabled)
		{
			_enabled = enabled;
			_predication = null;
			this.Name = string.IsNullOrWhiteSpace(name) ? Common.StringExtension.TrimStringEnd(this.GetType().Name, "Command", StringComparison.OrdinalIgnoreCase) : name;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置命令的名称。
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

				if(value.Contains(".") || value.Contains("/"))
					throw new ArgumentException();

				if(string.Equals(_name, value.Trim(), StringComparison.Ordinal))
					return;

				_name = value.Trim();

				//激发“PropertyChanged”事件
				this.OnPropertyChanged("Name");
			}
		}

		/// <summary>
		/// 获取或设置当前命令的断言对象，该断言决定当前命令是否可用。
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
		/// 获取或设置当前命令是否可用。
		/// </summary>
		/// <remarks>
		///		该属性作为当前命令是否可被执行的备选方案，命令是否可被执行由<see cref="CanExecute"/>方法决定，该方法的不同实现方式可能导致不同的判断逻辑。有关默认的判断逻辑请参考<seealso cref="CanExecute"/>方法的帮助。
		/// </remarks>
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
		#endregion

		#region 虚拟方法
		/// <summary>
		/// 判断命令是否为指定要匹配的名称。
		/// </summary>
		/// <param name="parameter">要匹配的参数，如果参数为空(null)则返回真；如果参数为字符串则返回其当前命令名进行不区分大小写匹对值；否则返回假(false)。</param>
		/// <returns>如果匹配成功则返回真(true)，否则返回假(false)。</returns>
		protected virtual bool IsMatch(object parameter)
		{
			if(parameter == null)
				return false;

			if(parameter is string)
				return string.Equals((string)parameter, _name, StringComparison.OrdinalIgnoreCase);

			return false;
		}

		protected virtual void OnEnabledChanged(EventArgs e)
		{
			var enabledChanged = this.EnabledChanged;

			if(enabledChanged != null)
				enabledChanged(this, e);
		}

		protected virtual void OnExecuted(CommandExecutedEventArgs e)
		{
			var executed = this.Executed;

			if(executed != null)
				executed(this, e);
		}

		protected virtual void OnExecuting(CommandExecutingEventArgs e)
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

		#region 公共方法
		/// <summary>
		/// 判断命令是否可被执行。
		/// </summary>
		/// <param name="context">判断命令是否可被执行的上下文对象。</param>
		/// <returns>如果返回真(true)则表示命令可被执行，否则表示不可执行。</returns>
		/// <remarks>
		///		<para>本方法为虚拟方法，可由子类更改基类的默认实现方式。</para>
		///		<para>如果<seealso cref="Predication"/>属性为空(null)，则返回<see cref="Enabled"/>属性值；否则返回由<see cref="Predication"/>属性指定的断言对象的断言方法的值。</para>
		/// </remarks>
		public virtual bool CanExecute(TContext context)
		{
			//如果断言对象是空则返回是否可用变量的值
			if(_predication == null)
				return _enabled;

			//返回断言对象的断言测试的值
			return _enabled && _predication.Predicate(context);
		}

		/// <summary>
		/// 执行命令。
		/// </summary>
		/// <param name="context">执行命令的上下文对象。</param>
		/// <returns>返回执行的返回结果。</returns>
		/// <remarks>
		///		<para>本方法的实现中首先调用<see cref="CanExecute"/>方法，以确保阻止非法的调用。</para>
		/// </remarks>
		public object Execute(TContext context)
		{
			//在执行之前首先判断是否可以执行
			if(!this.CanExecute(context))
				return null;

			//创建事件参数对象
			var executingArgs = new CommandExecutingEventArgs(context);
			//激发“Executing”事件
			this.OnExecuting(executingArgs);

			//如果事件处理程序要取消后续操作，则返回退出
			if(executingArgs.Cancel)
				return executingArgs.Result;

			try
			{
				//执行具体的工作
				this.OnExecute(context);
			}
			catch(Exception ex)
			{
				var executedArgs = new CommandExecutedEventArgs(context, ex);

				//激发“Executed”事件
				this.OnExecuted(executedArgs);

				if(!executedArgs.ExceptionHandled)
					throw;

				return executedArgs.Result;
			}

			//激发“Executed”事件
			this.OnExecuted(new CommandExecutedEventArgs(context));

			//返回执行成功的结果
			return context == null ? null : context.Result;
		}
		#endregion

		#region 抽象方法
		protected virtual TContext CreateContext(object parameter, IDictionary<string, object> items)
		{
			return parameter as TContext;
		}

		protected abstract void OnExecute(TContext context);
		#endregion

		#region 显式实现
		/// <summary>
		/// 判断命令是否可被执行。
		/// </summary>
		/// <param name="parameter">判断命令是否可被执行的参数对象。</param>
		/// <returns>如果返回真(true)则表示命令可被执行，否则表示不可执行。</returns>
		/// <remarks>
		///		<para>本方法为虚拟方法，可由子类更改基类的默认实现方式。</para>
		///		<para>如果<seealso cref="Predication"/>属性为空(null)，则返回<see cref="Enabled"/>属性值；否则返回由<see cref="Predication"/>属性指定的断言对象的断言方法的值。</para>
		/// </remarks>
		bool ICommand.CanExecute(object parameter)
		{
			if(parameter == null && default(TContext) == null)
				return this.CanExecute(default(TContext));

			if(parameter is TContext)
				return this.CanExecute((TContext)parameter);

			return this.CanExecute(this.CreateContext(parameter, null));
		}

		object ICommand.Execute(object parameter)
		{
			if(parameter == null && default(TContext) == null)
				return this.Execute(default(TContext));

			if(parameter is TContext)
				return this.Execute((TContext)parameter);

			return this.Execute(this.CreateContext(parameter, null));
		}

		/// <summary>
		/// 判断命令是否可被执行。
		/// </summary>
		/// <param name="parameter">判断命令是否可被执行的参数。</param>
		/// <returns>如果返回真(true)则表示命令可被执行，否则表示不可执行。</returns>
		/// <remarks>
		///		<para>本显式实现为调用<see cref="CanExecute"/>虚拟方法。</para>
		/// </remarks>
		bool IPredication.Predicate(object parameter)
		{
			return ((ICommand)this).CanExecute(parameter);
		}

		bool IPredication<TContext>.Predicate(TContext context)
		{
			return this.CanExecute(context);
		}

		/// <summary>
		/// 判断命令是否为指定要匹配的名称。
		/// </summary>
		/// <param name="parameter">要匹配的参数，如果参数为空(null)则返回真；如果参数为字符串则返回其当前命令名进行不区分大小写匹对值；否则返回假(false)。</param>
		/// <returns>如果匹配成功则返回真(true)，否则返回假(false)。</returns>
		/// <remarks>该显式实现为调用<see cref="IsMatch"/>虚拟方法。</remarks>
		bool IMatchable.IsMatch(object parameter)
		{
			return this.IsMatch(parameter);
		}
		#endregion
	}
}
