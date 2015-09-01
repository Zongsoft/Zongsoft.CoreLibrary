/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Security.Membership
{
	public class AuthorizationState
	{
		#region 成员变量
		private string _schemaId;
		private string _actionId;
		#endregion

		#region 构造函数
		public AuthorizationState(string schemaId, string actionId)
		{
			if(string.IsNullOrWhiteSpace(schemaId))
				throw new ArgumentNullException("schemaId");
			if(string.IsNullOrWhiteSpace(actionId))
				throw new ArgumentNullException("actionId");

			_schemaId = schemaId.Trim();
			_actionId = actionId.Trim();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置授权目标代号。
		/// </summary>
		public string SchemaId
		{
			get
			{
				return _schemaId;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_schemaId = value.Trim();
			}
		}

		/// <summary>
		/// 获取或设置授权行为代号。
		/// </summary>
		public string ActionId
		{
			get
			{
				return _actionId;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_actionId = value.Trim();
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var other = (AuthorizationState)obj;

			return string.Equals(_schemaId, other._schemaId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_actionId, other._actionId, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (_schemaId + ":" + _actionId).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", _schemaId, _actionId);
		}
		#endregion
	}
}
