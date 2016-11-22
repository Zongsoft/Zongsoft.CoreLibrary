/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	public class CommandOptionCollection : IEnumerable<KeyValuePair<string, string>>
	{
		#region 成员字段
		private bool _requiredDeclared;
		private IDictionary<string, CommandOptionAttribute> _attributes;
		private IDictionary<string, string> _items;
		#endregion

		#region 构造函数
		public CommandOptionCollection()
		{
			_items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public CommandOptionCollection(IEnumerable<KeyValuePair<string, string>> items)
		{
			if(items == null)
				throw new ArgumentNullException(nameof(items));

			_items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			foreach(var item in items)
				_items[item.Key] = item.Value;
		}
		#endregion

		#region 内部属性
		internal bool RequiredDeclared
		{
			get
			{
				return _requiredDeclared;
			}
			set
			{
				_requiredDeclared = value;
			}
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public string this[string key]
		{
			get
			{
				if(string.IsNullOrWhiteSpace(key))
					throw new ArgumentNullException(nameof(key));

				return _items[key];
			}
			set
			{
				if(string.IsNullOrWhiteSpace(key))
					throw new ArgumentNullException(nameof(key));

				//确认指定的选项名是否存在并且选项值是否有效
				this.EnsureOptionValue(key, value);

				_items[key] = value;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return _items.Keys;
			}
		}

		public ICollection<string> Values
		{
			get
			{
				return _items.Values;
			}
		}
		#endregion

		#region 公共方法
		public bool Contains(string name)
		{
			return _items.ContainsKey(name);
		}

		public object GetValue(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			string value;

			if(_items.TryGetValue(name, out value))
				return value;

			CommandOptionAttribute attribute;

			if(_attributes.TryGetValue(name, out attribute))
				return Common.Convert.ConvertValue(attribute.DefaultValue, attribute.Type);
			else
				throw new KeyNotFoundException($"The '{name}' command option is not existed.");
		}

		public T GetValue<T>(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			string value;

			if(_items.TryGetValue(name, out value))
				return Common.Convert.ConvertValue<T>(value);

			CommandOptionAttribute attribute;

			if(_attributes.TryGetValue(name, out attribute))
				return Common.Convert.ConvertValue<T>(attribute.DefaultValue);
			else
				throw new KeyNotFoundException($"The '{name}' command option is not existed.");
		}

		public bool TryGetValue(string name, out object value)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			string result;

			if(_items.TryGetValue(name, out result))
			{
				CommandOptionAttribute attribute;

				if(_attributes.TryGetValue(name, out attribute))
					value = Common.Convert.ConvertValue(result, attribute.Type);
				else
					value = result;

				return true;
			}

			value = null;
			return false;
		}

		public bool TryGetValue<T>(string name, out T value)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			string result;

			if(_items.TryGetValue(name, out result))
			{
				value = Common.Convert.ConvertValue<T>(result);
				return true;
			}

			value = default(T);
			return false;
		}
		#endregion

		#region 静态方法
		public static CommandOptionCollection GetOptions(ICommand command, IEnumerable<KeyValuePair<string, string>> options)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 私有方法
		private void EnsureOptionValue(string name, string value)
		{
			if(!_requiredDeclared || _attributes == null)
				return;

			CommandOptionAttribute attribute;

			if(!_attributes.TryGetValue(name, out attribute))
				throw new CommandOptionException($"The '{name}' command option is not declared.");

			if(attribute.Type == null)
				return;

			object temp;

			if(attribute.Converter != null)
			{
				if(!attribute.Converter.CanConvertFrom(typeof(string)))
					throw new CommandOptionValueException(name, value);

				try
				{
					attribute.Converter.ConvertFrom(value);
				}
				catch
				{
					throw new CommandOptionValueException(name, value);
				}
			}
			else
			{
				if(!Common.Convert.TryConvertValue(value, attribute.Type, out temp))
					throw new CommandOptionValueException(name, value);
			}
		}
		#endregion

		#region 枚举遍历
		IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion
	}
}
