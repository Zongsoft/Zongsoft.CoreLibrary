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
using System.Collections.Concurrent;
using System.Threading;

namespace Zongsoft.Transactions
{
	public class Transaction : IDisposable, IEquatable<Transaction>
	{
		#region 静态字段
		private static readonly ThreadLocal<Stack<Transaction>> _locals = new ThreadLocal<Stack<Transaction>>(() => new Stack<Transaction>());
		#endregion

		#region 私有变量
		private Queue<IEnlistment> _enlistments;
		#endregion

		#region 成员字段
		private IsolationLevel _isolationLevel;
		private TransactionInformation _information;
		#endregion

		#region 构造函数
		public Transaction() : this(IsolationLevel.ReadCommitted)
		{
		}

		public Transaction(IsolationLevel isolationLevel)
		{
			_isolationLevel = isolationLevel;

			//首先设置当前事务的父事务
			_information = new TransactionInformation(Transaction.Current);

			//创建本事务的登记集合
			_enlistments = new Queue<IEnlistment>();

			//将当前事务对象加入到事务栈中
			_locals.Value.Push(this);
		}
		#endregion

		#region 静态属性
		public static Transaction Current
		{
			get
			{
				if(_locals.IsValueCreated)
				{
					var stack = _locals.Value;

					if(stack.Count > 0)
						return stack.Peek();
				}

				return null;
				//return TransactionManager.Instance.Current;
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前事务的隔离级别。
		/// </summary>
		public IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
		}

		/// <summary>
		/// 获取当前事务的附加信息。
		/// </summary>
		public TransactionInformation Information
		{
			get
			{
				return _information;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 向当前事务登记一个事务处理过程的回调。
		/// </summary>
		/// <param name="enlistment">指定的事务处理过程的回调接口。</param>
		public void Enlist(IEnlistment enlistment)
		{
			if(enlistment == null)
				throw new ArgumentNullException("enlistment");

			//如果指定的事务处理程序已经被登记过则返回
			if(_enlistments.Contains(enlistment))
				return;

			//将指定的事务处理程序加入到列表中
			_enlistments.Enqueue(enlistment);

			//通知事务处理程序进入事务准备阶段
			//enlistment.OnEnlist(new EnlistmentContext(this, EnlistmentPhase.Prepare));
		}

		/// <summary>
		/// 提交事务。
		/// </summary>
		public void Commit()
		{
			this.DoEnlistment(EnlistmentPhase.Commit);
		}

		/// <summary>
		/// 回滚事务。
		/// </summary>
		public void Rollback()
		{
			this.DoEnlistment(EnlistmentPhase.Rollback);
		}
		#endregion

		#region 私有方法
		private void DoEnlistment(EnlistmentPhase phase)
		{
			while(_enlistments.Count > 0)
			{
				var enlistment = _enlistments.Dequeue();

				enlistment.OnEnlist(new EnlistmentContext(this, phase));
			}

			while(_locals.Value.Count > 0)
			{
				var current = _locals.Value.Pop();

				if(object.ReferenceEquals(current, this))
					return;

				current.Rollback();
			}
		}
		#endregion

		#region 处置方法
		protected virtual void Dispose(bool disposing)
		{
			this.Rollback();
		}

		public void Dispose()
		{
			this.Dispose(true);
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return object.ReferenceEquals(this, obj);
		}
		#endregion

		#region 相等比较
		bool IEquatable<Transaction>.Equals(Transaction other)
		{
			if(other == null)
				return false;

			return object.ReferenceEquals(this, other);
		}
		#endregion
	}
}
