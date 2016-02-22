/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2012 Zongsoft Corporation <http://www.zongsoft.com>
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
	[Obsolete]
	[Serializable]
	public class Schema
	{
		#region 成员变量
		private string _name;
		private string _title;
		private string _description;
		private bool _visible;
		private ActionCollection _actions;
		#endregion

		#region 构造函数
		public Schema()
		{
			_visible = true;
		}

		public Schema(string name) : this(name, name, string.Empty, true)
		{
		}

		public Schema(string name, string title) : this(name, title, string.Empty, true)
		{
		}

		public Schema(string name, string title, string description) : this(name, title, description, true)
		{
		}

		public Schema(string name, string title, string description, bool visible)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			_name = name;
			_title = string.IsNullOrEmpty(title) ? name : title;
			_description = description;
			_visible = visible;
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			Schema target = obj as Schema;

			if((target == null) && (this != null))
				return false;

			return string.Equals(_name, target.Name, StringComparison.OrdinalIgnoreCase);
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

				_name = value;
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
				if(!string.IsNullOrEmpty(value))
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

		[System.ComponentModel.DefaultValue(true)]
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

		public ActionCollection Actions
		{
			get
			{
				if(_actions == null)
					System.Threading.Interlocked.CompareExchange(ref _actions, new ActionCollection(), null);

				return _actions;
			}
		}
		#endregion
	}
}
