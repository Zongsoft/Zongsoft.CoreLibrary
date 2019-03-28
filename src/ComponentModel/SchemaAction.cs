/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.ComponentModel
{
	public class SchemaAction : IEquatable<SchemaAction>
	{
		#region 成员变量
		private string _name;
		private string _title;
		private string _description;
		private bool _visible;
		#endregion

		#region 构造函数
		public SchemaAction(string name) : this(name, null, null)
		{
		}

		public SchemaAction(string name, string title) : this(name, title, null)
		{
		}

		public SchemaAction(string name, string title, string description)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			_name = name.Trim();
			_title = string.IsNullOrWhiteSpace(title) ? _name : title;
			_description = description;
			_visible = true;
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}
		#endregion

		#region 重写方法
		public bool Equals(SchemaAction action)
		{
			if(action == null)
				return false;

			return string.Equals(_name, action.Name, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return string.Equals(_name, ((SchemaAction)obj).Name, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			if(_name == null)
				return base.GetHashCode();

			return _name.ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			if(_name == null)
				return string.Empty;

			return _name;
		}
		#endregion
	}
}
