/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Diagnostics
{
	public class LoggerHandlerPredication : Zongsoft.Services.IPredication<LogEntry>
	{
		#region 成员字段
		private string _source;
		private LogLevel? _minLevel;
		private LogLevel? _maxLevel;
		private Type _exceptionType;
		#endregion

		#region 公共属性
		public string Source
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
			}
		}

		public Type ExceptionType
		{
			get
			{
				return _exceptionType;
			}
			set
			{
				_exceptionType = value;
			}
		}

		public LogLevel? MinLevel
		{
			get
			{
				return _minLevel;
			}
			set
			{
				_minLevel = value;
			}
		}

		public LogLevel? MaxLevel
		{
			get
			{
				return _maxLevel;
			}
			set
			{
				_maxLevel = value;
			}
		}
		#endregion

		#region 断言方法
		public bool Predicate(LogEntry entry)
		{
			if(entry == null)
				return false;

			if(!string.IsNullOrWhiteSpace(this.Source))
			{
				var matched = true;
				var source = this.Source.Trim();

				if(source[0] == '*' || source[source.Length - 1] == '*')
				{
					if(source[0] == '*')
					{
						if(source[source.Length - 1] == '*')
							matched = entry.Source.Contains(source.Trim('*'));
						else
							matched = entry.Source.EndsWith(source.Trim('*'));
					}
					else
					{
						matched = entry.Source.StartsWith(source.Trim('*'));
					}
				}
				else
				{
					matched &= string.Equals(entry.Source, source, StringComparison.OrdinalIgnoreCase);
				}

				if(!matched)
					return false;
			}

			if(this.MinLevel.HasValue)
			{
				if(entry.Level < this.MinLevel.Value)
					return false;
			}

			if(this.MaxLevel.HasValue)
			{
				if(entry.Level > this.MaxLevel.Value)
					return false;
			}

			if(this.ExceptionType != null)
			{
				if(entry.Exception == null)
					return false;

				bool result;

				if(entry.Exception.GetType() == typeof(AggregateException))
				{
					result = ((AggregateException)entry.Exception).InnerExceptions.Any(ex => this.ExceptionType.IsAssignableFrom(ex.GetType()));
				}
				else
				{
					result = this.ExceptionType.IsAssignableFrom(entry.Exception.GetType());
				}

				if(!result)
					return false;
			}

			return true;
		}

		bool Zongsoft.Services.IPredication.Predicate(object parameter)
		{
			return this.Predicate(parameter as LogEntry);
		}
		#endregion
	}
}
