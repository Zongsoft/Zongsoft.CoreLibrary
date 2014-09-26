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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zongsoft.Services
{
	public class CommandLine : MarshalByRefObject
	{
		#region 正则表达
		private const string PATTERN = @"
		(?<prefix>\.{1,2})?
		(?(prefix)/|(?<prefix>/?))
		(?<path>([\./]?[\w-]+)+)
		(?<option>
			\s*[/-]
			(?<optName>\w+)
			(
				\s*[:=]\s*
				(
					(
						(?<quote>[""'])
						(?<optValue>.+?)
						\k<quote>
					)|(?<optValue>[^:=/\s-]+)
				)
			)?
		)*
		(\s*
			(
				(
					(?<quote>[""'])
					(?<arg>.+?)
					\k<quote>
				)|(?<arg>[^:=/\s-]+)
			)
		)*";

		private readonly static Regex _regex = new Regex(PATTERN, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
		#endregion

		#region 静态常量
		private readonly static string[] EmptyArray = new string[0];
		#endregion

		#region 成员字段
		private string _path;
		private string _name;
		private string _fullPath;
		private IDictionary<string, string> _options;
		private string[] _arguments;
		#endregion

		#region 构造函数
		public CommandLine(string fullPath, IDictionary<string, string> options, params string[] arguments)
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

		private CommandLine(string[] parts, string prefix)
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

		public string[] Arguments
		{
			get
			{
				return _arguments ?? EmptyArray;
			}
		}
		#endregion

		#region 解析方法
		/// <summary>
		/// 解析命令行文本。
		/// </summary>
		/// <param name="commandText">指定的要解析的命令行文本。</param>
		/// <returns>返回解析成功的命令行对象，如果解析失败则返回空(null)。</returns>
		public static CommandLine Parse(string commandText)
		{
			if(string.IsNullOrWhiteSpace(commandText))
				return null;

			var match = _regex.Match(commandText);

			if(!match.Success)
				return null;

			var commandLine = new CommandLine(match.Groups["path"].Value.Split('.', '/'), match.Groups["prefix"].Value);

			if(match.Groups["option"].Captures.Count > 0)
			{
				int valueIndex = 0;

				for(int i = 0; i < match.Groups["option"].Captures.Count; i++)
				{
					Capture optionCapture = match.Groups["option"].Captures[i];
					string optionValue = null;

					if(valueIndex < match.Groups["optValue"].Captures.Count)
					{
						var valueCapture = match.Groups["optValue"].Captures[valueIndex];

						if(valueCapture.Index > optionCapture.Index && valueCapture.Index < optionCapture.Index + optionCapture.Length)
						{
							optionValue = valueCapture.Value;
							valueIndex++;
						}
					}

					commandLine._options[match.Groups["optName"].Captures[i].Value] = optionValue;
				}
			}

			commandLine._arguments = new string[match.Groups["arg"].Captures.Count];

			for(int i = 0; i < commandLine._arguments.Length; i++)
			{
				commandLine._arguments[i] = match.Groups["arg"].Captures[i].Value;
			}

			return commandLine;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			string result = this.FullPath;

			if(_options != null && _options.Count > 0)
			{
				foreach(var option in _options)
				{
					if(string.IsNullOrWhiteSpace(option.Value))
						result += string.Format(" /{0}", option.Key);
					else
					{
						if(option.Value.Contains("\""))
							result += string.Format(" /{0}:'{1}'", option.Key, option.Value);
						else
							result += string.Format(" /{0}:\"{1}\"", option.Key, option.Value);
					}
				}
			}

			if(_arguments != null && _arguments.Length > 0)
			{
				foreach(var argument in _arguments)
				{
					if(argument.Contains("\""))
						result += string.Format(" '{0}'", argument);
					else
						result += string.Format(" \"{0}\"", argument);
				}
			}

			return result ?? string.Empty;
		}
		#endregion

		#region 嵌套子类
		internal class CommandLineParser : ICommandLineParser
		{
			public static readonly ICommandLineParser Instance = new CommandLineParser();

			CommandLine ICommandLineParser.Parse(string commandText)
			{
				return CommandLine.Parse(commandText);
			}
		}
		#endregion
	}
}
