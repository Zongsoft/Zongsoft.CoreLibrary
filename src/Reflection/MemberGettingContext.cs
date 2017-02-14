/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
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
using System.Reflection;

namespace Zongsoft.Reflection
{
	/// <summary>
	/// 表示在成员访问程序中的操作上下文。
	/// </summary>
	public class MemberGettingContext
	{
		#region 成员字段
		private object _owner;
		private MemberToken _memberToken;
		private MemberInfo _memberInfo;
		private MemberGettingContext _parent;
		#endregion

		#region 构造函数
		internal MemberGettingContext(object owner, MemberToken member, MemberGettingContext parent = null)
		{
			if(owner == null)
				throw new ArgumentNullException(nameof(owner));
			if(member == null)
				throw new ArgumentNullException(nameof(member));

			_owner = owner;
			_parent = parent;
			_memberToken = member;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前访问上下文的父元素上下文。
		/// </summary>
		public MemberGettingContext Parent
		{
			get
			{
				return _parent;
			}
		}

		/// <summary>
		/// 获取解析过程中当前成员的所有者对象。
		/// </summary>
		public object Owner
		{
			get
			{
				return _owner;
			}
		}

		/// <summary>
		/// 获取当前访问的成员标志。
		/// </summary>
		public MemberToken MemberToken
		{
			get
			{
				return _memberToken;
			}
		}

		/// <summary>
		/// 获取当前访问的成员信息。
		/// </summary>
		public MemberInfo MemberInfo
		{
			get
			{
				if(_memberInfo == null)
					_memberInfo = MemberAccess.GetMemberInfo(_owner is Type ? (Type)_owner : _owner.GetType(), _memberToken);

				return _memberInfo;
			}
		}

		/// <summary>
		/// 获取当前访问成员的类型。
		/// </summary>
		public Type MemberType
		{
			get
			{
				if(_owner is Type)
					return (Type)_owner;

				return MemberAccess.GetMemberType(this.MemberInfo);
			}
		}
		#endregion

		#region 公共方法
		public object GetMemberValue()
		{
			object value;

			if(MemberAccess.TryGetMemberValueCore(_owner, _memberToken, out value))
				return value;

			throw new InvalidOperationException(string.Format("The '{0}' member is not exists in the '{1}' type.", _memberToken, (_owner is Type ? ((Type)_owner).FullName : _owner.GetType().FullName)));
		}

		public bool TryGetMemberValue(out object value)
		{
			return MemberAccess.TryGetMemberValueCore(_owner, _memberToken, out value);
		}
		#endregion
	}
}
