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
		private int _index;
		private string _name;
		private string _path;
		private string _fullPath;
		private Zongsoft.IO.PathAnchor _anchor;
		private CommandOptionCollection _options;
		private string[] _arguments;
		private CommandExpression _next;
		#endregion

		#region 构造函数
		public CommandExpression(Zongsoft.IO.PathAnchor anchor, string name, string path, IDictionary<string, string> options, params string[] arguments)
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

			if(options == null || options.Count == 0)
				_options = new CommandOptionCollection();
			else
				_options = new CommandOptionCollection(options);

			_arguments = arguments ?? new string[0];
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

		public CommandOptionCollection Options
		{
			get
			{
				return _options;
			}
		}

		public string[] Arguments
		{
			get
			{
				return _arguments;
			}
		}

		public int Index
		{
			get
			{
				return _index;
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
				if(object.ReferenceEquals(value, this))
					throw new ArgumentException();

				var current = _next = value;

				if(current == null)
					return;

				//创建更新栈（以避免循环引用导致的死锁）
				var stack = new HashSet<CommandExpression>();

				while(current != null)
				{
					if(!stack.Add(current))
					{
						_next = null;
						throw new InvalidOperationException("The operation has been declined. Because a circular references error occurred.");
					}

					current = current._next;
				}

				//更新后趋命令表达式的序号
				value.SetIndex(_index + 1);
			}
		}
		#endregion

		#region 解析方法
		public static CommandExpression Parse(string text)
		{
			return CommandExpressionParser.Instance.Parse(text);
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			string result = this.FullPath;

			if(_options.Count > 0)
			{
				foreach(var option in _options)
				{
					if(string.IsNullOrWhiteSpace(option.Value))
						result += string.Format(" /{0}", option.Key);
					else
					{
						if(option.Value.Contains("\""))
							result += $" -{option.Key}:'{option.Value}'";
						else
							result += $" -{option.Key}:\"{option.Value}\"";
					}
				}
			}

			if(_arguments.Length > 0)
			{
				foreach(var argument in _arguments)
				{
					if(argument.Contains("\""))
						result += $" '{argument}'";
					else
						result += $" \"{argument}\"";
				}
			}

			var next = this.Next;

			if(next == null)
				return result;
			else
				return result + " | " + next.ToString();
		}
		#endregion

		#region 私有方法
		private void SetIndex(int value)
		{
			if(value < 0)
				return;

			//设置当前表达式的序号
			_index = value;

			//获取下一个表达式
			var current = _next;

			while(current != null)
			{
				current._index = ++value;
				current = current._next;
			}
		}
		#endregion
	}
}
