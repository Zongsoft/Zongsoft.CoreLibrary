/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;

namespace Zongsoft.Security.Membership
{
	public class Authorization : IAuthorization
	{
		#region 成员字段
		private IPermissionProvider _permissionProvider;
		private IRoleProvider _roleProvider;
		#endregion

		#region 公共属性
		public IPermissionProvider PermissionProvider
		{
			get
			{
				return _permissionProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_permissionProvider = value;
			}
		}

		public IRoleProvider RoleProvider
		{
			get
			{
				return _roleProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_roleProvider = value;
			}
		}
		#endregion

		#region 公共方法
		public bool IsAuthorized(int userId, string schemaId, string actionId)
		{
			if(string.IsNullOrWhiteSpace(schemaId))
				throw new ArgumentNullException("schemaId");

			if(string.IsNullOrWhiteSpace(actionId))
				throw new ArgumentNullException("actionId");

			//获取指定的安全凭证对应的有效的授权状态集
			var states = this.GetAuthorizedStates(userId, MemberType.User);

			return states != null && states.Any(state => string.Equals(state.SchemaId, schemaId, StringComparison.OrdinalIgnoreCase) &&
			                                             string.Equals(state.ActionId, actionId, StringComparison.OrdinalIgnoreCase));
		}

		public IEnumerable<AuthorizedState> GetAuthorizedStates(int memberId, MemberType memberType)
		{
			return this.GetAuthorizedStatesCore(memberId, memberType);
		}
		#endregion

		#region 虚拟方法
		protected virtual IEnumerable<AuthorizedState> GetAuthorizedStatesCore(int memberId, MemberType memberType)
		{
			if(_roleProvider == null)
				throw new InvalidOperationException("The value of 'RoleProvider' property is null.");

			if(_permissionProvider == null)
				throw new InvalidOperationException("The value of 'PermissionProvider' property is null.");

			var stack = new Stack<IEnumerable<Role>>();

			//递归获取当前成员所属角色信息，并将其所属上级角色依次压入指定的栈中
			this.RecursiveRoles(null, stack, _roleProvider.GetRoles(memberId, memberType));

			//创建授权状态集
			var grantedStates = new HashSet<AuthorizedState>();
			var deniedStates = new HashSet<AuthorizedState>();
			var states = new HashSet<AuthorizedState>();

			while(stack.Count > 0)
			{
				//从栈中弹出某个层级的角色集合
				var roles = stack.Pop();

				foreach(var role in roles)
				{
					//获取指定角色的授权集合
					this.SlicePermission(role.RoleId, MemberType.Role, grantedStates, deniedStates);
				}

				//将最终的授权结果集与显式授予集进行合并
				states.UnionWith(grantedStates);
				//从最终的授权结果集中删除显式拒绝集
				states.ExceptWith(deniedStates);

				//必须将当前层级的显式授予集清空
				grantedStates.Clear();
				//必须将当前层级的显式拒绝集清空
				deniedStates.Clear();
			}

			//获取指定成员的授权集合
			this.SlicePermission(memberId, memberType, grantedStates, deniedStates);

			//将最终的授权结果集与显式授予集进行合并
			states.UnionWith(grantedStates);
			//从最终的授权结果集中删除显式拒绝集
			states.ExceptWith(deniedStates);

			//将显式授予集清空
			grantedStates.Clear();
			//将显式拒绝集清空
			deniedStates.Clear();

			return states;
		}
		#endregion

		#region 私有方法
		private void SlicePermission(int memberId, MemberType memberType, HashSet<AuthorizedState> grantedStates, HashSet<AuthorizedState> deniedStates)
		{
			var permissions = _permissionProvider.GetPermissions(memberId, memberType);

			foreach(var permission in permissions)
			{
				if(permission.Granted)
					grantedStates.Add(new AuthorizedState(permission.SchemaId, permission.ActionId));
				else
					deniedStates.Add(new AuthorizedState(permission.SchemaId, permission.ActionId));
			}
		}

		private void RecursiveRoles(HashSet<string> hashSet, Stack<IEnumerable<Role>> stack, IEnumerable<Role> roles)
		{
			if(roles == null)
				return;

			var availableRoles = new List<Role>();

			if(hashSet == null)
				hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			//对传入的角色集进行是否有循环引用的检测和过滤
			foreach(var role in roles)
			{
				string key = (role.Namespace + ":" + role.Name).ToLowerInvariant();

				//如果当前角色没有循环引用
				if(hashSet.Add(key))
					availableRoles.Add(role);
			}

			//将过滤过的没有循环引用的角色集加入到当前栈中
			stack.Push(availableRoles);

			//创建父级角色列表
			var parents = new List<Role>();

			foreach(var role in availableRoles)
			{
				//获取指定角色所属的的父级角色集
				roles = _roleProvider.GetRoles(role.RoleId, MemberType.Role);

				if(roles != null)
					parents.AddRange(roles);
			}

			//如果当前角色集的所有父级角色集不为空则递归调用
			if(parents != null && parents.Count > 0)
				this.RecursiveRoles(hashSet, stack, parents);
		}
		#endregion
	}
}
