/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2010 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Zongsoft.Data
{
	[Serializable]
	public class ParameterCollection : NameObjectCollectionBase
	{
		#region 构造函数
		public ParameterCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
		}
		#endregion

		#region 公共属性
		public Parameter this[int index]
		{
			get
			{
				return (Parameter)base.BaseGet(index);
			}
		}

		public Parameter this[string name]
		{
			get
			{
				return (Parameter)base.BaseGet(name);
			}
		}
		#endregion

		#region 公共方法
		public void Clear()
		{
			base.BaseClear();
		}

		public bool Contains(string name)
		{
			return (base.BaseGet(name) != null);
		}

		public void Add(string name, object value)
		{
			this.Add(name, value, ParameterDirection.Input);
		}

		public void Add(string name, object value, ParameterDirection direction)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			base.BaseAdd(name, new Parameter(name, value, direction));
		}

		public int IndexOf(string name)
		{
			string[] keys = base.BaseGetAllKeys();
			return Array.IndexOf<string>(keys, name);
		}

		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}

		public Parameter[] ToArray()
		{
			return this.ToList().ToArray();
		}

		public Dictionary<string, Parameter> ToDictionary()
		{
			Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>(this.Count);

			for(int i = 0; i < this.Count; i++)
				parameters.Add(this.BaseGetKey(i), this[i]);

			return parameters;
		}

		public List<Parameter> ToList()
		{
			List<Parameter> parameters = new List<Parameter>(this.Count);

			for(int i = 0; i < this.Count; i++)
				parameters.Add(this[i]);

			return parameters;
		}
		#endregion

		#region 重写方法
		public override System.Collections.IEnumerator GetEnumerator()
		{
			for(int i = 0; i < this.Count; i++)
			{
				yield return this[i];
			}
		}
		#endregion
	}
}
