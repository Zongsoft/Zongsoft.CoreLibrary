/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
		#region 常量定义
		internal const int OPERATION_NONE = 0;
		internal const int OPERATION_ROLLBACK = 1;
		internal const int OPERATION_ABORT = 2;
		#endregion

		#region 静态字段
		[ThreadStatic]
		private static Transaction _current;
		#endregion

		#region 私有变量
		private int _operation;
		private Queue<IEnlistment> _enlistments;
		#endregion

		#region 成员字段
		private int _isCompleted;
		private Transaction _parent;
		private TransactionBehavior _behavior;
		private IsolationLevel _isolationLevel;
		private TransactionStatus _status;
		private TransactionInformation _information;
		#endregion

		#region 构造函数
		public Transaction() : this(TransactionBehavior.Followed, IsolationLevel.ReadCommitted)
		{
		}

		public Transaction(IsolationLevel isolationLevel) : this(TransactionBehavior.Followed, isolationLevel)
		{
		}

		public Transaction(TransactionBehavior behavior) : this(behavior, IsolationLevel.ReadCommitted)
		{
		}

		public Transaction(TransactionBehavior behavior, IsolationLevel isolationLevel)
		{
			_status = TransactionStatus.Active;
			_behavior = behavior;
			_isolationLevel = isolationLevel;

			switch(behavior)
			{
				case TransactionBehavior.Followed:
				case TransactionBehavior.Required:
					//如果当前环境事务为空，则将当前事务置为环境事务
					if(_current == null)
						_current = this;
					else
						_parent = _current;

					break;
				case TransactionBehavior.RequiresNew:
					//始终将当前事务置为环境事务
					_current = this;

					break;
				case TransactionBehavior.Suppress:
					throw new NotSupportedException();
			}

			//首先设置当前事务的父事务
			_information = new TransactionInformation(this);

			//创建本事务的登记集合
			_enlistments = new Queue<IEnlistment>();
		}
		#endregion

		#region 静态属性
		/// <summary>
		/// 获取当前环境事务。
		/// </summary>
		public static Transaction Current
		{
			get
			{
				return _current;
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
		/// 获取当前事务是否已终结。
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				return _isCompleted != 0;
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

		#region 内部属性
		internal int Operation
		{
			get
			{
				return _operation;
			}
			set
			{
				_operation = value;
			}
		}

		internal Transaction Parent
		{
			get
			{
				return _parent;
			}
		}

		internal TransactionBehavior Behavior
		{
			get
			{
				return _behavior;
			}
		}

		internal TransactionStatus Status
		{
			get
			{
				return _status;
			}
		}
		#endregion

		#region 静态方法
		public static Transaction ReadUncommitted(TransactionBehavior behavior = TransactionBehavior.Followed)
		{
			return new Transaction(behavior, IsolationLevel.ReadUncommitted);
		}

		public static Transaction ReadCommitted(TransactionBehavior behavior = TransactionBehavior.Followed)
		{
			return new Transaction(behavior, IsolationLevel.ReadCommitted);
		}

		public static Transaction RepeatableRead(TransactionBehavior behavior = TransactionBehavior.Followed)
		{
			return new Transaction(behavior, IsolationLevel.RepeatableRead);
		}

		public static Transaction Serializable(TransactionBehavior behavior = TransactionBehavior.Followed)
		{
			return new Transaction(behavior, IsolationLevel.Serializable);
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 向当前事务登记一个事务处理过程的回调。
		/// </summary>
		/// <param name="enlistment">指定的事务处理过程的回调接口。</param>
		/// <returns>如果注册成功则返回真(True)，否则返回假(False)。</returns>
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public bool Enlist(IEnlistment enlistment)
		{
			if(enlistment == null)
				throw new ArgumentNullException("enlistment");

			//如果指定的事务处理程序已经被登记过则返回
			if(_enlistments.Contains(enlistment))
				return false;

			//将指定的事务处理程序加入到列表中
			_enlistments.Enqueue(enlistment);

			//通知事务处理程序进入事务准备阶段
			//enlistment.OnEnlist(new EnlistmentContext(this, EnlistmentPhase.Prepare));

			return true;
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
			var isCompleted = System.Threading.Interlocked.Exchange(ref _isCompleted, 1);

			if(isCompleted != 0)
				return;

			//如果当前事务是跟随模式，并且具有父事务则当前事务不用通知投票者(订阅者)，而是交由父事务处理
			if(_behavior == TransactionBehavior.Followed && _parent != null)
			{
				switch(phase)
				{
					case EnlistmentPhase.Abort:
						_parent._operation = OPERATION_ABORT;
						break;
					case EnlistmentPhase.Rollback:
						_parent._operation = OPERATION_ROLLBACK;
						break;
				}

				//更新当前事务的状态
				this.UpdateStatus(phase);

				//退出当前子事务
				return;
			}

			switch(_operation)
			{
				case OPERATION_ABORT:
					phase = EnlistmentPhase.Abort;
					break;
				case OPERATION_ROLLBACK:
					phase = EnlistmentPhase.Rollback;
					break;
			}

			while(_enlistments.Count > 0)
			{
				var enlistment = _enlistments.Dequeue();

				enlistment.OnEnlist(new EnlistmentContext(this, phase));
			}

			//更新当前事务的状态
			this.UpdateStatus(phase);
		}

		private void UpdateStatus(EnlistmentPhase phase)
		{
			switch(phase)
			{
				case EnlistmentPhase.Abort:
				case EnlistmentPhase.Rollback:
					_status = TransactionStatus.Aborted;
					break;
				case EnlistmentPhase.Commit:
					_status = TransactionStatus.Committed;
					break;
				default:
					_status = TransactionStatus.Undetermined;
					break;
			}
		}
		#endregion

		#region 处置方法
		protected virtual void Dispose(bool disposing)
		{
			this.Rollback();

			//如果结束的是环境事务则置空环境事务的指针
			if(object.ReferenceEquals(_current, this))
				_current = null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion

		#region 重写方法
		public override bool Equals(object obj)
		{
			if(obj == null || obj.GetType() != this.GetType())
				return false;

			return object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return _information.TransactionId.GetHashCode();
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
