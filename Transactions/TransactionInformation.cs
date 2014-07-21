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

namespace Zongsoft.Transactions
{
	public class TransactionInformation
	{
		#region 成员字段
		private Guid _transactionId;
		private IDictionary<string, object> _arguments;
		private Transaction _parent;
		#endregion

		#region 构造函数
		public TransactionInformation(Transaction parent)
		{
			_transactionId = Guid.NewGuid();
			_parent = parent;
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
		/// 获取当前事务的环境参数。
		/// </summary>
		public IDictionary<string, object> Arguments
		{
			get
			{
				if(_arguments == null)
					System.Threading.Interlocked.CompareExchange(ref _arguments, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

				return _arguments;
			}
		}

		/// <summary>
		/// 获取当前事务对象的父事务，如果当前事务是根事务则返回空(null)。
		/// </summary>
		public Transaction Parent
		{
			get
			{
				return _parent;
			}
		}
		#endregion
	}
}
