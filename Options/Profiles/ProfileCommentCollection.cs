/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Options.Profiles
{
	public class ProfileCommentCollection : ICollection<ProfileComment>
	{
		#region 成员字段
		private ProfileItemCollection _items;
		#endregion

		#region 构造函数
		public ProfileCommentCollection(ProfileItemCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");

			_items = items;
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get
			{
				return _items.Count(item => item.ItemType == ProfileItemType.Comment);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return _items.IsReadOnly;
			}
		}
		#endregion

		#region 公共方法
		public ProfileComment Add(string comment, int lineNumber = -1)
		{
			if(comment == null)
				return null;

			var item = new ProfileComment(comment, lineNumber);
			_items.Add(item);
			return item;
		}

		public void Add(ProfileComment item)
		{
			_items.Add(item);
		}

		public void Clear()
		{
			foreach(var item in _items)
			{
				if(item.ItemType == ProfileItemType.Comment)
					_items.Remove(item);
			}
		}

		public bool Contains(ProfileComment item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(ProfileComment[] array, int arrayIndex)
		{
			if(array == null)
				return;

			if(arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException("arrayIndex");

			int index = 0;

			foreach(var item in _items)
			{
				if(arrayIndex + index >= array.Length)
					return;

				if(item.ItemType == ProfileItemType.Comment)
					array[arrayIndex + index++] = (ProfileComment)item;
			}
		}

		public bool Remove(ProfileComment item)
		{
			return _items.Remove(item);
		}
		#endregion

		#region 遍历枚举
		public IEnumerator<ProfileComment> GetEnumerator()
		{
			foreach(var item in _items)
			{
				if(item.ItemType == ProfileItemType.Comment)
					yield return (ProfileComment)item;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		#endregion
	}
}
