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
using System.Linq;
using System.Text;

namespace Zongsoft.Services
{
	[Obsolete]
	public class WorkerDecorator : MarshalByRefObject, IWorker
	{
		#region 事件定义
		public event EventHandler Started;
		public event CancelEventHandler Starting;

		public event EventHandler Stopped;
		public event CancelEventHandler Stopping;
		#endregion

		#region 成员变量
		private string _name;
		private string _title;
		private string _description;
		private bool _disabled;
		private string[] _dependences;
		private Lazy<IWorker> _resolver;
		#endregion

		#region 构造函数
		public WorkerDecorator(Lazy<IWorker> resolver) : this(resolver, null)
		{
		}

		public WorkerDecorator(Lazy<IWorker> resolver, string name)
		{
			if(resolver == null)
				throw new ArgumentNullException("resolver");

			_name = name;
			_disabled = false;
			_resolver = resolver;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_name))
				{
					var worker = _resolver.Value;

					if(worker != null)
						return worker.Name;
				}

				return _name ?? string.Empty;
			}
		}

		public bool Disabled
		{
			get
			{
				return _disabled;
			}
			set
			{
				if(_disabled == value)
					return;

				_disabled = value;

				if(value)
					this.Stop();
			}
		}

		public WorkerState State
		{
			get
			{
				if(_resolver.IsValueCreated)
					return _resolver.Value.State;

				return WorkerState.Stopped;
			}
		}

		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value ?? string.Empty;
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
				_description = value ?? string.Empty;
			}
		}

		public string[] Dependences
		{
			get
			{
				return _dependences;
			}
		}
		#endregion

		#region 公共方法
		public void SetDependences(string dependences)
		{
			if(string.IsNullOrWhiteSpace(dependences))
				_dependences = new string[0];
			else
				_dependences = dependences.Split(',');
		}

		public void Start()
		{
			this.Start(new string[0]);
		}

		public void Start(string[] args)
		{
			if(_disabled)
				return;

			var worker = _resolver.Value;

			if(worker == null)
				throw new InvalidOperationException();

			worker.Start(args);
		}

		public void Stop()
		{
			if(_resolver.IsValueCreated)
				_resolver.Value.Stop();
		}
		#endregion
	}
}
