/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据排序的设置项。
	/// </summary>
	public struct Sorting : IEquatable<Sorting>
	{
		#region 成员字段
		private SortingMode _mode;
		private string _name;
		#endregion

		#region 构造函数
		public Sorting(string name, SortingMode mode = SortingMode.Ascending)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name;
			_mode = mode;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示排序方式。
		/// </summary>
		public SortingMode Mode
		{
			get
			{
				return _mode;
			}
		}

		/// <summary>
		/// 获取排序项名称（即排序的成员名）。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 创建一个正序的排序设置项。
		/// </summary>
		/// <param name="name">指定的排序项名称。</param>
		/// <returns>返回创建成果的排序设置项实例。</returns>
		public static Sorting Ascending(string name)
		{
			return new Sorting(name, SortingMode.Ascending);
		}

		/// <summary>
		/// 创建一个倒序的排序设置项。
		/// </summary>
		/// <param name="name">指定的排序项名称。</param>
		/// <returns>返回创建成果的排序设置项实例。</returns>
		public static Sorting Descending(string name)
		{
			return new Sorting(name, SortingMode.Descending);
		}
		#endregion

		#region 重写方法
		public bool Equals(Sorting other)
		{
			return other._mode == _mode &&
			       string.Equals(other._name, _name, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return this.Equals((Sorting)obj);
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode() ^ _mode.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", _mode, _name);
		}
		#endregion

		#region 操作符重载
		public static Sorting[] operator +(Sorting[] sortings, Sorting value)
		{
			if(sortings == null || sortings.Length == 0)
				return new Sorting[] { value };

			foreach(var sorting in sortings)
			{
				if(string.Equals(sorting._name, value._name, StringComparison.OrdinalIgnoreCase))
					return sortings;
			}

			var result = new Sorting[sortings.Length + 1];
			Array.Copy(sortings, 0, result, 0, sortings.Length);
			result[result.Length - 1] = value;

			return result;
		}

		public static Sorting[] operator +(Sorting a, Sorting b)
		{
			if(string.Equals(a._name, b._name, StringComparison.OrdinalIgnoreCase))
				return new Sorting[] { b };
			else
				return new Sorting[] { a, b };
		}
		#endregion
	}

	/// <summary>
	/// 表示排序方式的枚举。
	/// </summary>
	public enum SortingMode
	{
		/// <summary>正序</summary>
		Ascending,
		/// <summary>倒序</summary>
		Descending,
	}
}
