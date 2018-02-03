/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;

namespace Zongsoft.Security.Membership
{
	[AttributeUsage((AttributeTargets.Class | AttributeTargets.Method), AllowMultiple = false, Inherited = true)]
	public class AuthorizationAttribute : Attribute
	{
		#region 成员变量
		private string _schemaId;
		private string _actionId;
		private string[] _roles;
		private AuthorizationMode _mode;
		private Type _validatorType;
		private Common.IValidator<Credential> _validator;
		#endregion

		#region 构造函数
		public AuthorizationAttribute(AuthorizationMode mode)
		{
			_mode = mode;
		}

		public AuthorizationAttribute(string[] roles)
		{
			if(roles == null || roles.Length == 0)
				throw new ArgumentNullException("roles");

			_roles = roles;
			_mode = AuthorizationMode.Identity;
		}

		public AuthorizationAttribute(string schemaId) : this(schemaId, (string)null)
		{
		}

		public AuthorizationAttribute(string schemaId, string actionId)
		{
			_actionId = actionId ?? string.Empty;
			_schemaId = schemaId ?? string.Empty;

			_mode = AuthorizationMode.Requires;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取授权验证的方式。
		/// </summary>
		public AuthorizationMode Mode
		{
			get
			{
				return _mode;
			}
		}

		/// <summary>
		/// 获取操作编号。
		/// </summary>
		public string ActionId
		{
			get
			{
				return _actionId;
			}
		}

		/// <summary>
		/// 获取模式编号。
		/// </summary>
		public string SchemaId
		{
			get
			{
				return _schemaId;
			}
		}

		/// <summary>
		/// 获取验证的所属角色名数组。
		/// </summary>
		public string[] Roles
		{
			get
			{
				return _roles;
			}
		}

		/// <summary>
		/// 获取或设置凭证验证器的类型。
		/// </summary>
		public Type ValidatorType
		{
			get
			{
				return _validatorType;
			}
			set
			{
				if(_validatorType == value)
					return;

				if(value != null && !typeof(Common.IValidator<Credential>).IsAssignableFrom(value))
					throw new ArgumentException();

				_validatorType = value;
				_validator = null;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取凭证验证器实例。
		/// </summary>
		public virtual Common.IValidator<Credential> GetValidator(Func<Type, Common.IValidator<Credential>> creator = null)
		{
			if(_validator == null)
			{
				var type = this.ValidatorType;

				if(type == null)
					return null;

				if(creator == null)
					creator = _ => Activator.CreateInstance(_) as Common.IValidator<Credential>;

				lock(type)
				{
					if(_validator == null)
					{
						_validator = creator(type);
					}
				}
			}

			return _validator;
		}
		#endregion
	}
}
