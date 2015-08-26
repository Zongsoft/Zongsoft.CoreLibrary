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

namespace Zongsoft.Transactions
{
	public class EnlistmentContext
	{
		#region 成员字段
		private EnlistmentPhase _phase;
		private Transaction _transaction;
		#endregion

		#region 构造函数
		internal EnlistmentContext(Transaction transaction, EnlistmentPhase phase)
		{
			if(transaction == null)
				throw new ArgumentNullException("transaction");

			_transaction = transaction;
			_phase = phase;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前事务阶段。
		/// </summary>
		public EnlistmentPhase Phase
		{
			get
			{
				return _phase;
			}
		}

		/// <summary>
		/// 获取当前的事务对象。
		/// </summary>
		public Transaction Transaction
		{
			get
			{
				return _transaction;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 将当前事务更改为跟随父事务。
		/// </summary>
		/// <returns></returns>
		public bool Follow()
		{
			var parent = _transaction.Parent;

			if(parent == null)
				return false;

			switch(this.Phase)
			{
				case EnlistmentPhase.Abort:
					parent.Operation = Transaction.OPERATION_ABORT;
					break;
				case EnlistmentPhase.Rollback:
					parent.Operation = Transaction.OPERATION_ROLLBACK;
					break;
			}

			return true;
		}
		#endregion
	}
}
