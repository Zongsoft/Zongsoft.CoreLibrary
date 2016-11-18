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
		private IList<string> _arguments;
		private CommandExpression _next;
		#endregion

		#region 构造函数
		internal CommandExpression(Zongsoft.IO.PathAnchor anchor, string name, string path, IDictionary<string, string> options = null, params string[] arguments)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			//修缮传入的路径参数值
			path = path.Trim('/', ' ', '\t', '\r', '\n');

			_anchor = anchor;
			_name = name.Trim();

			switch(anchor)
			{
				case IO.PathAnchor.Root:
					if(string.IsNullOrEmpty(path))
						_path = "/";
					else
						_path = "/" + path + "/";
					break;
				case IO.PathAnchor.Current:
					if(string.IsNullOrEmpty(path))
						_path = "./";
					else
						_path = "./" + path + "/";
					break;
				case IO.PathAnchor.Parent:
					if(string.IsNullOrEmpty(path))
						_path = "../";
					else
						_path = "../" + path + "/";
					break;
				default:
					if(string.IsNullOrEmpty(path))
						_path = string.Empty;
					else
						_path = path + "/";
					break;
			}

			_fullPath = _path + _name;

			if(options != null)
				_options = new Dictionary<string, string>(options, StringComparer.OrdinalIgnoreCase);

			if(arguments != null)
				_arguments = new List<string>(arguments);
		}

		[Obsolete]
		internal CommandExpression(string fullPath, IDictionary<string, string> options = null, params string[] arguments)
		{
			if(string.IsNullOrWhiteSpace(fullPath))
				throw new ArgumentNullException("fullPath");

			var parts = fullPath.Split('/');

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

			if(options != null)
				_options = new Dictionary<string, string>(options, StringComparer.OrdinalIgnoreCase);

			if(arguments != null)
				_arguments = new List<string>(arguments);
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

		public bool HasOptions
		{
			get
			{
				return _options?.Count > 0;
			}
		}

		public bool HasArguments
		{
			get
			{
				return _arguments?.Count > 0;
			}
		}

		public IDictionary<string, string> Options
		{
			get
			{
				if(_options == null)
					System.Threading.Interlocked.CompareExchange(ref _options, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), null);

				return _options;
			}
		}

		public IList<string> Arguments
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

		#region 解析方法
		public static CommandExpression Parse(string text)
		{
			return CommandExpressionParser.Parse(text);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var next = this.Next;

			if(next == null)
				return _fullPath;
			else
				return _fullPath + " | " + next.ToString();
		}
		#endregion
	}
}
