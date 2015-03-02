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
using System.Text;

namespace Zongsoft.Options.Profiles
{
	public class ProfileComment : ProfileItem
	{
		#region 私有变量
		private StringBuilder _text;
		#endregion

		#region 构造函数
		public ProfileComment(string text, int lineNumber = -1) : base(lineNumber)
		{
			if(string.IsNullOrEmpty(text))
				_text = new StringBuilder();
			else
				_text = new StringBuilder(text);
		}
		#endregion

		#region 公共属性
		public string Text
		{
			get
			{
				return _text.ToString();
			}
		}

		public string[] Lines
		{
			get
			{
				return _text.ToString().Split('\r', '\n');
			}
		}

		public override ProfileItemType ItemType
		{
			get
			{
				return ProfileItemType.Comment;
			}
		}
		#endregion

		#region 公共方法
		public void Append(string text)
		{
			if(string.IsNullOrEmpty(text))
				return;

			_text.Append(text);
		}

		public void AppendFormat(string format, params object[] args)
		{
			if(string.IsNullOrEmpty(format))
				return;

			_text.AppendFormat(format, args);
		}

		public void AppendLine(string text)
		{
			if(text == null)
				_text.AppendLine();
			else
				_text.AppendLine(text);
		}
		#endregion
	}
}
