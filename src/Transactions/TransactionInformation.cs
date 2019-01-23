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
using System.Collections.Concurrent;

namespace Zongsoft.Transactions
{
	public class TransactionInformation
	{
		#region 成员字段
		private Guid _transactionId;
		private Transaction _transaction;
		private ConcurrentDictionary<string, object> _parameters;
		#endregion

		#region 构造函数
		public TransactionInformation(Transaction transaction)
		{
			if(transaction == null)
				throw new ArgumentNullException("transaction");

			_transaction = transaction;
			_transactionId = Guid.NewGuid();
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前事务的唯一编号。
		/// </summary>
		public Guid TransactionId
		{
			get
			{
				return _transactionId;
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

		/// <summary>
		/// 获取当前事务对象的父事务，如果当前事务是根事务则返回空(null)。
		/// </summary>
		public Transaction Parent
		{
			get
			{
				return _transaction.Parent;
			}
		}

		/// <summary>
		/// 获取当前事务的行为特性。
		/// </summary>
		public TransactionBehavior Behavior
		{
			get
			{
				return _transaction.Behavior;
			}
		}

		/// <summary>
		/// 获取当前事务的状态。
		/// </summary>
		public TransactionStatus Status
		{
			get
			{
				return _transaction.Status;
			}
		}

		/// <summary>
		/// 获取当前事务的环境参数。
		/// </summary>
		public ConcurrentDictionary<string, object> Parameters
		{
			get
			{
				if(_parameters == null)
					System.Threading.Interlocked.CompareExchange(ref _parameters, new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _parameters;
			}
		}
		#endregion
	}
}
