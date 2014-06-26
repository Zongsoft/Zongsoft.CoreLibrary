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
using System.Collections.Generic;

namespace Zongsoft.Security.Membership
{
	[Serializable]
	public class Permission
	{
		#region 成员变量
		private string _applicationId;
		private string _schemaId;
		private string _actionId;
		private bool _granted;
		#endregion

		#region 构造函数
		public Permission(string applicationId)
		{
			if(string.IsNullOrWhiteSpace(applicationId))
				throw new ArgumentNullException("applicationId");

			_applicationId = applicationId.Trim();
		}

		public Permission(string applicationId, string schemaId, string actionId, bool granted) : this(applicationId)
		{
			if(string.IsNullOrEmpty(schemaId))
				throw new ArgumentNullException("schemaId");
			if(string.IsNullOrEmpty(actionId))
				throw new ArgumentNullException("actionId");

			_schemaId = schemaId;
			_actionId = actionId;
			_granted = granted;
		}
		#endregion

		#region 公共属性
		public string ApplicationId
		{
			get
			{
				return _applicationId; 
			}
			protected set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_applicationId = value.Trim();
			}
		}

		public string SchemaId
		{
			get
			{
				return _schemaId;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				_schemaId = value;
			}
		}

		public string ActionId
		{
			get
			{
				return _actionId;
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentNullException();

				_actionId = value;
			}
		}

		public bool Granted
		{
			get
			{
				return _granted;
			}
			set
			{
				_granted = value;
			}
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			var permission = (Permission)obj;

			return _granted == permission._granted &&
			       string.Equals(_applicationId, permission._applicationId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_schemaId, permission._schemaId, StringComparison.OrdinalIgnoreCase) &&
				   string.Equals(_actionId, permission._actionId, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return (_applicationId + ":" + _schemaId + ":" + _actionId + ":" + _granted.ToString()).ToLowerInvariant().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}[{2}]({3})", _schemaId, _actionId, _applicationId, _granted ? "Granted" : "Denied");
		}
		#endregion
	}
}
