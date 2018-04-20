/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2010-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;

namespace Zongsoft.Reflection
{
	public class MemberTokenProvider
	{
		#region 单例模式
		public static readonly MemberTokenProvider Default = new MemberTokenProvider(type => Common.TypeExtension.IsScalarType(type));
		#endregion

		#region 成员字段
		private readonly Predicate<Type> _ignores;
		private readonly ConcurrentDictionary<Type, MemberTokenCollection> _cache;
		#endregion

		#region 构造函数
		public MemberTokenProvider(Predicate<Type> ignores)
		{
			_ignores = ignores;
			_cache = new ConcurrentDictionary<Type, MemberTokenCollection>();
		}
		#endregion

		#region 公共方法
		public MemberToken GetMember(Type type, string path)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			if(string.IsNullOrEmpty(path))
				return null;

			var parts = path.Split('.');
			var members = this.GetMembers(type);
			MemberToken member = null;

			for(int i = 0; i < parts.Length; i++)
			{
				var part = parts[i].Trim();

				if(string.IsNullOrEmpty(part) || part == "*")
				{
					var elementType = Common.TypeExtension.GetCollectionElementType(member == null ? type : member.Type);

					if(elementType == null)
						return null;

					members = this.GetMembers(elementType);
				}
				else
				{
					if(members != null && members.TryGet(part, out member))
						members = this.GetMembers(member.Type);
					else
						return null;
				}
			}

			return member;
		}

		public MemberTokenCollection GetMembers(Type type, MemberKind kinds = (MemberKind.Field | MemberKind.Property))
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			//如果忽略指定的类型，则返回空
			if(_ignores != null && _ignores(type))
				return null;

			return _cache.GetOrAdd(type, key => this.CreateMembers(key, kinds));
		}
		#endregion

		#region 虚拟方法
		protected virtual MemberTokenCollection CreateMembers(Type type, MemberKind kinds)
		{
			var flags = BindingFlags.Public | BindingFlags.Instance;

			if((kinds & MemberKind.Field) == MemberKind.Field)
				flags |= (BindingFlags.GetField | BindingFlags.SetField);

			if((kinds & MemberKind.Property) == MemberKind.Property)
				flags |= (BindingFlags.GetProperty | BindingFlags.SetProperty);

			//定义返回的成员描述集合
			MemberTokenCollection tokens = new MemberTokenCollection();

			foreach(var member in type.GetMembers(flags))
			{
				var token = this.CreateMember(member);

				if(token != null)
					tokens.Add(token);
			}

			return tokens;
		}

		protected virtual MemberToken CreateMember(MemberInfo member)
		{
			switch(member.MemberType)
			{
				case MemberTypes.Field:
					var field = (FieldInfo)member;

					if(!field.IsInitOnly)
						return new MemberToken(field);

					break;
				case MemberTypes.Property:
					var property = (PropertyInfo)member;

					if(property.CanRead && property.CanWrite)
						return new MemberToken(property);

					break;
			}

			return null;
		}
		#endregion
	}
}
