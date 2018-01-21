/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;

namespace Zongsoft.Data
{
	/// <summary>
	/// 提供数据范围操作的类。
	/// </summary>
	public class Scoping : ICollection<string>
	{
		#region 成员字段
		private ISet<string> _items;
		#endregion

		#region 构造函数
		private Scoping(IEnumerable<string> items)
		{
			if(items == null)
				_items = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			else
				_items = new HashSet<string>(items, StringComparer.OrdinalIgnoreCase);
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
		#endregion

		#region 公共方法
		public bool Contains(string item)
		{
			if(string.IsNullOrWhiteSpace(item))
				return false;

			return _items.Contains(item);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool Remove(string item)
		{
			if(string.IsNullOrEmpty(item))
				return false;

			return _items.Remove(item);
		}

		public Scoping Include(string item)
		{
			if(string.IsNullOrEmpty(item))
				return this;

			return this.Add(item.Trim('!'));
		}

		public Scoping Exclude(string item)
		{
			if(string.IsNullOrEmpty(item))
				return this;

			item = item.Trim();

			if(item == "*")
				item = "!";
			else if(item.Length > 0 && item[0] != '!')
				item = "!" + item;

			return this.Add(item);
		}

		public Scoping Add(string scope)
		{
			if(string.IsNullOrEmpty(scope))
				return this;

			var parts = scope.Split(',');

			foreach(var part in parts)
			{
				var item = part.Trim();

				if(string.IsNullOrEmpty(item))
					continue;

				if(item.Length == 1)
				{
					switch(item)
					{
						case "!":
							_items.Clear();
							break;
						case "*":
							_items.Remove("*");
							break;
					}
				}
				else
				{
					if(item[0] == '!')
					{
						var name = item.Substring(1).Trim();
						item = "!" + name;

						_items.Remove(item);
						_items.Remove(name);
					}
					else
					{
						_items.Remove(item);
						_items.Remove("!" + item);
					}
				}

				_items.Add(item);
			}

			return this;
		}

		public string[] ToArray(Func<IEnumerable<string>> starThunk = null)
		{
			if(starThunk == null)
				return _items.ToArray();

			var result = new HashSet<string>(starThunk(), StringComparer.OrdinalIgnoreCase);

			foreach(var item in _items)
			{
				if(string.IsNullOrEmpty(item))
					continue;

				if(item.Length == 1)
				{
					switch(item)
					{
						case "!":
							result.Clear();
							break;
						case "*":
							result.UnionWith(starThunk());
							break;
					}
				}
				else
				{
					if(item[0] == '!')
						result.Remove(item.Substring(1));
				}
			}

			return result.ToArray();
		}
		#endregion

		#region 静态方法
		public static Scoping Parse(string text)
		{
			var scoping = new Scoping(null);
			scoping.Add(text);
			return scoping;
		}
		#endregion

		#region 显式实现
		void ICollection<string>.Add(string text)
		{
			this.Add(text);
		}

		bool ICollection<string>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<string>.CopyTo(string[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}
		#endregion

		#region 遍历枚举
		public IEnumerator<string> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(_items.Count == 0)
				return string.Empty;

			var text = new System.Text.StringBuilder();

			foreach(var item in _items)
			{
				if(text.Length > 0)
					text.Append(", ");

				text.Append(item);
			}

			return text.ToString();
		}
		#endregion
	}
}
