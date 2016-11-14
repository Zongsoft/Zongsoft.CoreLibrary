/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2016 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Services
{
	public class CommandExpression
	{
		#region 成员字段
		private string _name;
		private string _path;
		private string _fullPath;
		private Zongsoft.IO.PathAnchor _anchor;
		private IDictionary<string, string> _options;
		private ICollection<string> _arguments;
		private CommandExpression _next;
		#endregion

		#region 构造函数
		public CommandExpression(string fullPath, IDictionary<string, string> options = null, params string[] arguments)
		{
			if(string.IsNullOrWhiteSpace(fullPath))
				throw new ArgumentNullException("fullPath");

			var parts = fullPath.Split('/', '.');

			for(int i = parts.Length - 1; i >= 0; i--)
			{
				if(string.IsNullOrWhiteSpace(parts[i]))
					continue;

				if(_name == null)
					_name = parts[i].Trim();
				else
				{
					if(_path == null)
						_path = parts[i].Trim();
					else
						_path = parts[i].Trim() + "/" + _path;
				}
			}

			if(_name == null)
				_name = "/";
			else
				_path = "/" + _path;

			if(_name == "/")
				_fullPath = "/";
			else
			{
				if(_path == "/")
					_fullPath = _path + _name;
				else
					_fullPath = _path + "/" + _name;
			}

			if(_options == null)
			{
				if(options == null)
					_options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				else
					_options = new Dictionary<string, string>(options, StringComparer.OrdinalIgnoreCase);
			}
			else
			{
				if(options != null && options.Count > 0)
				{
					foreach(var option in options)
						_options[option.Key] = option.Value;
				}
			}

			_arguments = arguments;
		}

		private CommandExpression(string[] parts, string prefix)
		{
			if(parts == null)
				throw new ArgumentNullException("parts");

			_name = string.Empty;
			_path = string.Empty;

			for(int i = parts.Length - 1; i >= 0; i--)
			{
				if(string.IsNullOrWhiteSpace(parts[i]))
					continue;

				if(string.IsNullOrEmpty(_name))
					_name = parts[i].Trim();
				else
				{
					if(string.IsNullOrEmpty(_path))
						_path = parts[i].Trim();
					else
						_path = parts[i].Trim() + "/" + _path;
				}
			}

			if(prefix == "..")
				_path = ".." + (string.IsNullOrEmpty(_path) ? "" : "/") + _path;
			else if(prefix == "/")
				_path = "/" + _path;

			if(string.IsNullOrWhiteSpace(_path))
				_fullPath = _name;
			else
				_fullPath = _path.TrimEnd('/') + "/" + _name;

			_options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public Zongsoft.IO.PathAnchor Anchor
		{
			get
			{
				return _anchor;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string Path
		{
			get
			{
				return _path;
			}
		}

		public string FullPath
		{
			get
			{
				return _fullPath;
			}
		}

		public IDictionary<string, string> Options
		{
			get
			{
				return _options;
			}
		}

		public ICollection<string> Arguments
		{
			get
			{
				if(_arguments == null)
					System.Threading.Interlocked.CompareExchange(ref _arguments, new List<string>(), null);

				return _arguments;
			}
		}

		public CommandExpression Next
		{
			get
			{
				return _next;
			}
			set
			{
				_next = value;
			}
		}
		#endregion
	}
}
