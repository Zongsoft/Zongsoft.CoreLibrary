﻿/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2016-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Services
{
	public class ApplicationContext : IApplicationContext, IApplicationModule
	{
		#region 单例字段
		private volatile static IApplicationContext _current;
		#endregion

		#region 事件声明
		public event EventHandler Exiting;
		public event EventHandler Starting;
		public event EventHandler Started;
		public event EventHandler Initializing;
		public event EventHandler Initialized;
		#endregion

		#region 成员字段
		private string _name;
		private string _title;
		private ISettingsProvider _settings;
		private readonly IList<IApplicationModule> _modules;
		private readonly IList<IApplicationInitializer> _initializers;
		private readonly IDictionary<string, object> _states;
		#endregion

		#region 构造函数
		protected ApplicationContext(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_modules = new List<IApplicationModule>();
			_initializers = new List<IApplicationInitializer>();
			_states = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 单例属性
		/// <summary>
		/// 获取当前应用程序的<see cref="IApplicationContext"/>上下文。
		/// </summary>
		public static IApplicationContext Current
		{
			get
			{
				return _current;
			}
			protected set
			{
				_current = value ?? throw new ArgumentNullException();
			}
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get => _name;
		}

		public string Title
		{
			get => _title;
			set => _title = value;
		}

		public virtual string ApplicationDirectory
		{
			get
			{
				return AppDomain.CurrentDomain.BaseDirectory;
			}
		}

		public ISettingsProvider Settings
		{
			get
			{
				if(_settings == null)
					System.Threading.Interlocked.CompareExchange(ref _settings, OptionManager.Default.Settings, null);

				return _settings;
			}
			protected set
			{
				_settings = value;
			}
		}

		public IOptionProvider Options
		{
			get
			{
				return OptionManager.Default;
			}
		}

		public OptionConfiguration Configuration
		{
			get => null;
		}

		public IServiceProvider Services
		{
			get => ServiceProviderFactory.Instance.Default;
		}

		public System.Security.Principal.IPrincipal Principal
		{
			get;
			set;
		}

		public ICollection<IApplicationModule> Modules
		{
			get => _modules;
		}

		public ICollection<IApplicationInitializer> Initializers
		{
			get => _initializers;
		}

		public IDictionary<string, object> States
		{
			get => _states;
		}
		#endregion

		#region 公共方法
		public string EnsureDirectory(string relativePath)
		{
			string fullPath = this.ApplicationDirectory;

			if(string.IsNullOrWhiteSpace(relativePath))
				return fullPath;

			var parts = relativePath.Split('/', '\\', Path.DirectorySeparatorChar);

			foreach(var part in parts)
			{
				if(string.IsNullOrWhiteSpace(part))
					continue;

				fullPath = Path.Combine(fullPath, part);

				if(!Directory.Exists(fullPath))
					Directory.CreateDirectory(fullPath);
			}

			return fullPath;
		}
		#endregion

		#region 激发事件
		protected virtual void OnExiting(EventArgs args)
		{
			this.Exiting?.Invoke(this, args);
		}

		protected virtual void OnStarting(EventArgs args)
		{
			this.Starting?.Invoke(this, args);
		}

		protected virtual void OnStarted(EventArgs args)
		{
			this.Started?.Invoke(this, args);
		}

		protected virtual void OnInitializing(EventArgs args)
		{
			this.Initializing?.Invoke(this, args);
		}

		protected virtual void OnInitialized(EventArgs args)
		{
			this.Initialized?.Invoke(this, args);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return string.Format("[{0}] {1}", _name, _title);
		}
		#endregion
	}
}