/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class DataSearchKeyAttribute : Attribute, IDataSearchKey
	{
		#region 成员字段
		private DataSearchKey[] _keys;
		#endregion

		#region 构造函数
		public DataSearchKeyAttribute(params string[] keys)
		{
			if(keys == null || keys.Length == 0)
				throw new ArgumentNullException(nameof(keys));

			var list = new List<DataSearchKey>(keys.Length);

			foreach(var key in keys)
			{
				if(string.IsNullOrWhiteSpace(key))
					continue;

				var parts = key.Split(':', '=');
				var tags = parts[0].Split(',').Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim().ToLowerInvariant());
				var fields = parts.Length > 1 ? parts[1].Split(',').Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()) : tags;

				list.Add(new DataSearchKey(tags.ToArray(), fields.ToArray()));
			}

			_keys = list.ToArray();
		}
		#endregion

		#region 公共属性
		public DataSearchKey[] Keys
		{
			get
			{
				return _keys;
			}
		}
		#endregion

		#region 公共方法
		public ICondition GetSearchKey(string keyword, params string[] tags)
		{
			ICollection<DataSearchKey> keys;

			if(tags == null || tags.Length == 0)
				keys = this.FindSearchKeys(new string[] { "Key" });
			else
				keys = this.FindSearchKeys(tags);

			if(keys == null || keys.Count == 0)
				return null;

			var conditions = new ConditionCollection(ConditionCombination.Or);

			foreach(var key in keys)
			{
				conditions.Add(key.ToCondition(keyword));
			}

			return conditions;
		}
		#endregion

		#region 私有方法
		private ICollection<DataSearchKey> FindSearchKeys(string[] tags)
		{
			if(tags == null || tags.Length == 0)
				return null;

			var result = new List<DataSearchKey>();

			foreach(var key in this.Keys)
			{
				foreach(var tag in tags)
				{
					if(key.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
					{
						result.Add(key);
						break;
					}
				}
			}

			return result;
		}
		#endregion

		#region 嵌套子类
		public class DataSearchKey
		{
			public readonly string[] Tags;
			public readonly string[] Fields;

			internal DataSearchKey(string[] tags, string[] fields)
			{
				if(tags == null || tags.Length == 0)
					throw new ArgumentNullException(nameof(tags));
				if(fields == null || fields.Length == 0)
					throw new ArgumentNullException(nameof(fields));

				this.Tags = tags;
				this.Fields = fields;
			}

			internal ICondition ToCondition(string keyword)
			{
				var conditions = new ConditionCollection(ConditionCombination.Or);

				foreach(var field in this.Fields)
				{
					conditions.Add(new Condition(field, keyword));
				}

				if(conditions.Count == 1)
					return conditions[0];
				else
					return conditions;
			}
		}
		#endregion
	}
}
