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
	[Obsolete("Please use the Scope class.")]
	public class Scoping : ICollection<string>
	{
		#region 成员字段
		private bool _shouldInitialize;
		private ISet<string> _items;
		#endregion

		#region 构造函数
		private Scoping()
		{
			_shouldInitialize = true;
			_items = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取范围包含的元素数量。
		/// </summary>
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
			if(string.IsNullOrEmpty(item))
				return false;

			return _items.Contains(item);
		}

		public void Clear()
		{
			_items.Clear();
			_shouldInitialize = true;
		}

		public bool Remove(string item)
		{
			if(string.IsNullOrEmpty(item))
				return false;

			return _items.Remove(item);
		}

		public Scoping Include(string scope)
		{
			if(string.IsNullOrEmpty(scope))
				return this;

			IEnumerable<string> GetParts(string[] parts)
			{
				for(int i = 0; i < parts.Length; i++)
				{
					var part = parts[i].Trim();

					if(string.IsNullOrEmpty(part))
						continue;

					yield return part.TrimStart('!');
				}
			}

			this.AddCore(GetParts(scope.Split(',')));
			return this;
		}

		public Scoping Exclude(string scope)
		{
			if(string.IsNullOrEmpty(scope))
				return this;

			IEnumerable<string> GetParts(string[] parts)
			{
				for(int i=0; i<parts.Length; i++)
				{
					var part = parts[i].Trim();

					if(string.IsNullOrEmpty(part))
						continue;

					if(part == "*")
						yield return "!";
					else if(part.Length > 0 && part[0] != '!')
						yield return "!" + part;
					else
						yield return part;
				}
			}

			this.AddCore(GetParts(scope.Split(',')));
			return this;
		}

		public Scoping Add(string scope)
		{
			if(string.IsNullOrEmpty(scope))
				return this;

			//调用增加元素核心方法
			this.AddCore(scope.Split(','));

			return this;
		}

		/// <summary>
		/// 将当前范围转换映射成元素数值。
		/// </summary>
		/// <param name="resolve">指定的模式解析函数，第一个参数为空表示初始化；否则即为通配符（譬如：“*” 星号表示所有可用元素）。</param>
		/// <returns>返回映射后的元素集。</returns>
		public IEnumerable<string> Map(Func<string, IEnumerable<string>> resolve = null)
		{
			//如果没有指定解析函数，则返回内部集
			if(resolve == null)
			{
				foreach(var item in _items)
					yield return item;

				//终止内部遍历
				yield break;
			}

			//如果需要初始化解析
			if(_shouldInitialize)
			{
				//调用解析函数进行初始化
				var items = resolve(_items.Count > 0 ? string.Empty : null);

				if(items != null)
				{
					foreach(var item in items)
					{
						if(!this.IsIgnorable(item))
							yield return item;
					}
				}
			}

			foreach(var item in _items)
			{
				if(item == "*")
				{
					var children = resolve(item);

					if(children != null)
					{
						foreach(var child in children)
						{
							if(!this.IsIgnorable(child))
								yield return child;
						}
					}
				}
				else
				{
					//只返回非排除成员（排除成员即以惊叹号打头的成员）
					if(item.Length > 0 && item[0] != '!')
						yield return item;
				}
			}
		}
		#endregion

		#region 静态方法
		public static Scoping Parse(string text)
		{
			var scoping = new Scoping();

			if(!string.IsNullOrWhiteSpace(text))
				scoping.AddCore(text.Split(','));

			return scoping;
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(_items.Count == 0)
				return string.Empty;

			return string.Join(", ", _items);
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

		#region 私有方法
		private bool IsIgnorable(string item)
		{
			if(string.IsNullOrEmpty(item))
				return true;

			return _items.Contains(item) || _items.Contains("!" + item);
		}

		private void AddCore(IEnumerable<string> parts)
		{
			if(parts == null)
				return;

			foreach(var temp in parts)
			{
				var part = temp.Trim();

				if(string.IsNullOrEmpty(part))
					continue;

				if(part.Length == 1)
				{
					switch(part)
					{
						case "!":
							_items.Clear();
							_shouldInitialize = false;

							break;
						case "*":
							_items.Remove("*");
							_shouldInitialize = false;

							break;
					}
				}
				else
				{
					if(part[0] == '!')
					{
						var name = part.Substring(1);

						_items.Remove(part);
						_items.Remove(name);

						var removes = new List<string>();

						foreach(var item in _items)
						{
							if(item.StartsWith(part + ".", StringComparison.OrdinalIgnoreCase) ||
							   item.StartsWith(name + ".", StringComparison.OrdinalIgnoreCase))
								removes.Add(item);
						}

						_items.ExceptWith(removes);
					}
					else
					{
						_items.Remove(part);
						_items.Remove("!" + part);
					}
				}

				if(part != "!")
					_items.Add(part);
			}
		}
		#endregion
	}
}
