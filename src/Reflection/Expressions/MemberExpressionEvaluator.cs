/*
 *   _____                                ______
 *  /_   /  ____  ____  ____  _________  / __/ /_
 *    / /  / __ \/ __ \/ __ \/ ___/ __ \/ /_/ __/
 *   / /__/ /_/ / / / / /_/ /\_ \/ /_/ / __/ /_
 *  /____/\____/_/ /_/\__  /____/\____/_/  \__/
 *                   /____/
 *
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@qq.com>
 *
 * Copyright (C) 2010-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

namespace Zongsoft.Reflection.Expressions
{
	public class MemberExpressionEvaluator
	{
		#region 单例字段
		public static readonly MemberExpressionEvaluator Default = new MemberExpressionEvaluator();
		#endregion

		#region 公共方法
		public object Evaluate(IMemberExpression expression, object origin)
		{
			var target = origin;

			while(expression != null)
			{
				target = this.OnEvaluate(expression, target);
				expression = expression.Next;
			}

			return target;
		}
		#endregion

		#region 虚拟方法
		protected virtual object OnEvaluate(IMemberExpression expression, object target)
		{
			MemberInfo member = null;

			switch(expression.ExpressionType)
			{
				case MemberExpressionType.Constant:
					return ((ConstantExpression)expression).Value;
				case MemberExpressionType.Identifier:
					member = this.GetMember(target, ((IdentifierExpression)expression).Name);
					return this.GetValue(member, (object)target, null);
				case MemberExpressionType.Method:
					var method = (MethodExpression)expression;
					member = this.GetMember(target, ((MethodExpression)expression).Name);
					return this.GetValue(member, target, this.GetParameters(target, method.Arguments));
				case MemberExpressionType.Indexer:
					var indexer = (IndexerExpression)expression;
					member = this.GetMember(target, null);
					return this.GetValue(member, target, this.GetParameters(target, indexer.Arguments));
				default:
					throw new NotSupportedException();
			}
		}

		protected virtual MemberInfo GetMember(object owner, string name)
		{
			if(owner == null)
				throw new ArgumentNullException(nameof(owner));

			var type = (owner as Type) ?? owner.GetType();
			var members = string.IsNullOrEmpty(name) ? type.GetDefaultMembers() : type.GetMember(name);

			if(members == null || members.Length == 0)
				throw new MissingMemberException();

			return members[0];
		}

		protected virtual object GetValue(MemberInfo member, object target, params object[] parameters)
		{
			return Reflector.GetValue(member, target, parameters);
		}
		#endregion

		#region 私有方法
		private object[] GetParameters(object origin, IList<IMemberExpression> arguments)
		{
			if(arguments == null || arguments.Count == 0)
				return null;

			var parameters = new object[arguments.Count];

			for(int i=0; i<arguments.Count; i++)
			{
				parameters[i] = this.Evaluate(arguments[i], origin);
			}

			return parameters;
		}
		#endregion
	}
}
