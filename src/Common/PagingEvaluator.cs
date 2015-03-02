/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2012 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.Text;

namespace Zongsoft.Common
{
	public class PagingEvaluator
	{
		#region 成员变量
		private int _rangeLength;
		private int _pageSize;
		#endregion

		#region 构造函数
		public PagingEvaluator() : this(11, 20)
		{
		}

		public PagingEvaluator(int rangeLength) : this(rangeLength, 20)
		{
		}

		public PagingEvaluator(int rangeLength, int pageSize)
		{
			this.PageSize = pageSize;
			this.RangeLength = rangeLength;
		}
		#endregion

		#region 单例模式
		private static PagingEvaluator _instance;
		public static PagingEvaluator Instance
		{
			get
			{
				if(_instance == null)
					System.Threading.Interlocked.CompareExchange(ref _instance, new PagingEvaluator(), null);

				return _instance;
			}
		}
		#endregion

		#region 公共属性
		public int RangeLength
		{
			get
			{
				return _rangeLength;
			}
			set
			{
				if(value < 2)
					throw new ArgumentOutOfRangeException();

				if(_rangeLength != value)
					_rangeLength = value;
			}
		}

		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				if(value < 1)
					throw new ArgumentOutOfRangeException();

				_pageSize = value;
			}
		}
		#endregion

		#region 公共方法
		public PagingRange Evaluate(int pageIndex, int pageCount)
		{
			return Evaluate(pageIndex, pageCount, _pageSize);
		}

		public PagingRange Evaluate(int pageIndex, int pageCount, int pageSize)
		{
			PagingRange range = new PagingRange();

			if(pageCount <= _rangeLength)
			{
				range.StartIndex = 1;
				range.FinishIndex = pageCount;

				return range;
			}

			int semiLength = (int)Math.Floor(_rangeLength / 2.0);

			range.StartIndex = pageIndex - semiLength;
			range.FinishIndex = pageIndex + semiLength;

			if(range.StartIndex < 1)
			{
				range.FinishIndex -= range.StartIndex - 1;
				range.StartIndex = 1;

				return range;
			}

			if(range.FinishIndex > pageCount)
			{
				range.StartIndex -= range.FinishIndex - pageCount;
				range.FinishIndex = pageCount;
			}

			return range;
		}
		#endregion

		#region 嵌套子类
		public struct PagingRange
		{
			#region 成员变量
			private int _startIndex;
			private int _finishIndex;
			#endregion

			#region 构造函数
			public PagingRange(int startIndex, int finishIndex)
			{
				_startIndex = startIndex;
				_finishIndex = finishIndex;
			}
			#endregion

			#region 公共属性
			public int StartIndex
			{
				get
				{
					return _startIndex;
				}
				internal set
				{
					_startIndex = value;
				}
			}

			public int FinishIndex
			{
				get
				{
					return _finishIndex;
				}
				internal set
				{
					_finishIndex = value;
				}
			}
			#endregion
		}
		#endregion
	}
}
