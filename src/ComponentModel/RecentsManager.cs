/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2007-2008 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Win32;

namespace Zongsoft.ComponentModel
{
	public class RecentsManager
	{
		#region 私有常量
		private const string DEFAULT_ROOT_PATH = @"Software\Zongsoft\Common\Recents";
		#endregion

		#region 静态字段
		public static readonly RecentsManager Default = new RecentsManager(DEFAULT_ROOT_PATH);
		#endregion

		#region 成员变量
		private string _rootPath;
		private readonly Dictionary<string, IList<string>> _recents;
		#endregion

		#region 构造函数
		public RecentsManager() : this(DEFAULT_ROOT_PATH)
		{
		}

		public RecentsManager(string rootPath)
		{
			this.RootPath = rootPath;

			_recents = new Dictionary<string, IList<string>>();
		}
		#endregion

		#region 公共属性
		public string RootPath
		{
			get
			{
				return _rootPath;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				_rootPath = value.Replace('/', '\\');
				_rootPath = _rootPath.Trim('\\', ' ');

				if(string.IsNullOrEmpty(_rootPath))
					throw new ArgumentException();
			}
		}

		public IList<string> this[string recentsPath]
		{
			get
			{
				string name = this.FormateKey(recentsPath);

				if(_recents.ContainsKey(name))
					return _recents[name];
				else
					return null;
			}
		}
		#endregion

		#region 公共方法
		public void Clear(string recentsPath)
		{
			RegistryKey sourceKey = GetRecentsKey(recentsPath);

			string[] valueNames = sourceKey.GetValueNames();
			foreach(string name in valueNames)
				sourceKey.DeleteValue(name, false);
		}

		public IList<string> Load(string recentsPath)
		{
			string name = this.FormateKey(recentsPath);

			if(_recents.ContainsKey(name))
				return _recents[name];
			else
				return this.Refresh(recentsPath);
		}

		public IList<string> Refresh(string recentsPath)
		{
			string name = this.FormateKey(recentsPath);

			_recents[name] = this.GetRecentList(recentsPath);
			return _recents[name];
		}

		public void SaveAll()
		{
			foreach(string recentsPath in _recents.Keys)
				this.Save(recentsPath);
		}

		public void Save(string recentsPath)
		{
			string name = this.FormateKey(recentsPath);

			if(_recents.ContainsKey(name))
				this.SetRecentList(recentsPath, _recents[name]);
		}

		public bool Update(string recentsPath, string value)
		{
			return this.Update(recentsPath, value, false);
		}

		public bool Update(string recentsPath, string value, bool throwExceptionOnPathNotFound)
		{
			if(string.IsNullOrEmpty(value))
				return false;

			string name = this.FormateKey(recentsPath);

			if(!_recents.ContainsKey(name))
			{
				if(throwExceptionOnPathNotFound)
					throw new KeyNotFoundException();
				else
					_recents[name] = this.GetRecentList(recentsPath);
			}

			int index = _recents[name].IndexOf(value);

			if(index == 0)
				return false;

			if(index < 0)
			{
				_recents[name].Insert(0, value);
			}
			else
			{
				_recents[name].RemoveAt(index);
				_recents[name].Insert(0, value);
			}

			return true;
		}
		#endregion

		#region 私有方法
		private string FormateKey(string recentsPath)
		{
			if(recentsPath == null)
				return string.Empty;

			return recentsPath.Trim().ToLower();
		}

		private string GetFullPath(string recentsPath)
		{
			if(string.IsNullOrEmpty(recentsPath))
				return this.RootPath;

			string fullPath = this.RootPath;
			string formattedPath = recentsPath.Replace('/', '\\').Trim();

			if(string.IsNullOrEmpty(formattedPath))
				return this.RootPath;

			if(formattedPath.StartsWith(@"\", StringComparison.Ordinal))
				fullPath += formattedPath;
			else
				fullPath += @"\" + formattedPath;

			return fullPath;
		}

		private IEnumerable<string> GetOrderedValueNames(RegistryKey recentsKey)
		{
			if(recentsKey == null)
				throw new ArgumentNullException("recentsKey");

			string[] names = recentsKey.GetValueNames();
			if(names == null || names.Length <= 0)
				return new string[0];

			IEnumerable<string> query = names
				.Where(name => Regex.IsMatch(name, @"\d+", (RegexOptions.Singleline | RegexOptions.IgnoreCase)))
				.OrderBy((name) => int.Parse(name));

			return query;
		}

		private RegistryKey GetRecentsKey(string recentsPath)
		{
			return Registry.CurrentUser.CreateSubKey(GetFullPath(recentsPath));
		}

		private IList<string> GetRecentList(string recentsPath)
		{
			RegistryKey sourceKey = GetRecentsKey(recentsPath);
			IEnumerable<string> orderedValueNames = GetOrderedValueNames(sourceKey);

			List<string> values = new List<string>(orderedValueNames.Count());
			foreach(string name in orderedValueNames)
			{
				values.Add(sourceKey.GetValue(name, string.Empty).ToString());
			}

			return values;
		}

		private void SetRecentList(string recentsPath, IList<string> recents)
		{
			if(recents == null)
				throw new ArgumentNullException("recents");

			if(recents.Count <= 0)
				return;

			this.Clear(recentsPath);

			RegistryKey sourceKey = GetRecentsKey(recentsPath);

			for(int i = 0; i < recents.Count; i++)
				sourceKey.SetValue(i.ToString(), recents[i], RegistryValueKind.String);
		}
		#endregion
	}
}
