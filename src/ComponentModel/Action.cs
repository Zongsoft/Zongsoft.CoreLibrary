/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class Action
	{
		#region 成员变量
		private string _name;
		private string _title;
		private string _description;
		#endregion

		#region 构造函数
		public Action()
		{
		}

		public Action(string name, string title) : this(name, title, null)
		{
		}

		public Action(string name, string title, string description)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			_name = name;
			_title = title;
			_description = description;
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			Action target = obj as Action;

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
				if(string.IsNullOrEmpty(value))
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
		#endregion

		#region 静态属性
		private static ActionCollection _defaults;
		public static ActionCollection Defaults
		{
			get
			{
				if(_defaults == null)
					System.Threading.Interlocked.CompareExchange(ref _defaults, new ActionCollection(), null);

				return _defaults;
			}
		}
		#endregion
	}
}
