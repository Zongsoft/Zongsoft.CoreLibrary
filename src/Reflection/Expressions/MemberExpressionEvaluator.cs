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
		public object GetValue(IMemberExpression expression, object origin, Action<MemberContext> evaluate = null)
		{
			if(expression == null)
				throw new ArgumentNullException(nameof(expression));

			var result = origin;
			MemberContext context = new MemberContext(0, origin, origin, expression);

			while(context != null)
			{
				context.Member = this.GetMember(context);
				evaluate?.Invoke(context);

				if(!context.HasValue)
					context.Value = this.GetMemberValue(context);

				result = context.Value;
				context = context.Next();
			}

			return result;
		}

		public void SetValue(IMemberExpression expression, object origin, object value, Action<MemberContext> evaluate = null)
		{
			this.SetValue(expression, origin, _ => value, evaluate);
		}

		public void SetValue(IMemberExpression expression, object origin, Func<MemberContext, object> valueFactory, Action<MemberContext> evaluate = null)
		{
			if(expression == null)
				throw new ArgumentNullException(nameof(expression));

			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			var context = new MemberContext(0, origin, origin, expression);

			while(context.HasNext)
			{
				context.Member = this.GetMember(context);
				evaluate?.Invoke(context);

				if(!context.HasValue)
					context.Value = this.GetMemberValue(context);

				context = context.Next();
			}

			context.Member = this.GetMember(context);
			evaluate?.Invoke(context);
			context.Value = valueFactory(context);

			this.SetMemberValue(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual MemberInfo GetMember(MemberContext context)
		{
			switch(context.Expression.ExpressionType)
			{
				case MemberExpressionType.Constant:
					return null;
				case MemberExpressionType.Identifier:
					return this.GetMember(context.Owner, ((IdentifierExpression)context.Expression).Name);
				case MemberExpressionType.Method:
					var method = (MethodExpression)context.Expression;
					return this.GetMember(context.Owner, method.Name);
				case MemberExpressionType.Indexer:
					var indexer = (IndexerExpression)context.Expression;
					return this.GetMember(context.Owner, string.Empty);
				default:
					throw new NotSupportedException();
			}
		}

		protected virtual MemberInfo GetMember(object owner, string name)
		{
			if(owner == null)
				throw new ArgumentNullException(nameof(owner));

			var type = (owner as Type) ?? owner.GetType();
			var members = string.IsNullOrEmpty(name) ?
				type.GetDefaultMembers() :
				type.GetMember(name, MemberTypes.Property | MemberTypes.Field | MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			if(members == null || members.Length == 0)
			{
				var contracts = type.GetInterfaces();

				foreach(var contract in contracts)
				{
					var mapping = type.GetInterfaceMap(contract);

					for(int i = 0; i < mapping.InterfaceMethods.Length; i++)
					{
						if(string.Equals(mapping.InterfaceMethods[i].Name, name, StringComparison.OrdinalIgnoreCase))
							return mapping.TargetMethods[i];
					}
				}

				throw new MissingMemberException();
			}

			return members[0];
		}

		protected virtual object GetMemberValue(MemberContext context)
		{
			if(context.Expression.ExpressionType == MemberExpressionType.Constant)
				return ((ConstantExpression)context.Expression).Value;

			return Reflector.GetValue(context.Member, ref context.Owner, context.Parameters);
		}

		protected virtual void SetMemberValue(MemberContext context)
		{
			Reflector.SetValue(context.Member, ref context.Owner, context.Value, context.Parameters);
		}
		#endregion

		#region 私有方法
		private object[] GetParameters(object origin, IList<IMemberExpression> arguments)
		{
			if(arguments == null || arguments.Count == 0)
				return null;

			var parameters = new object[arguments.Count];

			for(int i = 0; i < arguments.Count; i++)
			{
				parameters[i] = this.GetValue(arguments[i], origin);
			}

			return parameters;
		}
		#endregion

		#region 嵌套结构
		public class MemberContext
		{
			public int Index;
			public int Depth;
			public object Origin;
			public object Owner;
			public object Value;
			public MemberInfo Member;
			public IMemberExpression Expression;
			public object[] Parameters;
			public MemberContext Parent;

			public MemberContext(int index, object origin, object target, IMemberExpression expression)
			{
				this.Depth = 0;
				this.Index = index;
				this.Origin = origin;
				this.Owner = target;
				this.Expression = expression;
				this.Member = null;
				this.Parameters = null;
			}

			public int Indent()
			{
				return ++this.Depth;
			}

			public int Dedent()
			{
				return --this.Depth;
			}

			public bool HasValue
			{
				get;
			}

			public bool HasNext
			{
				get => this.Expression.Next != null;
			}

			public MemberContext Next()
			{
				if(this.Expression.Next == null)
					return null;

				this.Index++;
				this.Owner = this.Value;
				this.Value = null;
				this.Member = null;
				this.Expression = this.Expression.Next;

				return this;
			}
		}
		#endregion
	}
}
