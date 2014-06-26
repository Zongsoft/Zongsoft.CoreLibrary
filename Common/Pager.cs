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
	public class Pager<T>
	{
		#region 成员变量
		private IEnumerable<T> _source;

		private int _pageCount;
		private int _pageIndex;
		private int _pageSize;
		#endregion

		#region 构造函数
		public Pager(IEnumerable<T> source) : this(source, 1, 10)
		{
		}

		public Pager(IEnumerable<T> source, int? pageIndex) : this(source, pageIndex, 10)
		{
		}

		public Pager(IEnumerable<T> source, int? pageIndex, int pageSize)
		{
			if(source == null)
				throw new ArgumentNullException("source");

			//重置页数
			this.ResetPageCount();

			//初始化属性
			this.Source = source;
			this.PageSize = pageSize;
			this.PageIndex = pageIndex ?? 1;
			this.PageRange = new PageRangeSettings(this);
		}
		#endregion

		#region 保护属性
		protected IEnumerable<T> Source
		{
			get
			{
				return _source;
			}
			private set
			{
				if(value == null)
					throw new ArgumentNullException();

				if(_source != value)
				{
					_source = value;

					//重置页数，将导致后续的分页运算重新计算页数
					this.ResetPageCount();
				}
			}
		}
		#endregion

		#region 公共属性
		public IEnumerable<T> Result
		{
			get
			{
				int pageIndex = _pageIndex;

				pageIndex = Math.Max(1, _pageIndex);
				pageIndex = Math.Min(this.PageCount, _pageIndex);

				this.PageIndex = pageIndex;

				return this.Source
						.Skip((pageIndex - 1) * this.PageSize)
						.Take(this.PageSize);
			}
		}

		public PageRangeSettings PageRange
		{
			get;
			private set;
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

				if(_pageSize != value)
				{
					_pageSize = value;

					//重置页数，将导致后续的分页运算重新计算页数
					this.ResetPageCount();
				}
			}
		}

		public int PageIndex
		{
			get
			{
				return _pageIndex;
			}
			private set
			{
				if(value < 1)
					throw new ArgumentOutOfRangeException();

				if(_pageIndex != value)
					_pageIndex = value;
			}
		}

		public int PageCount
		{
			get
			{
				if(_pageCount < 0)
					_pageCount = (int)Math.Ceiling(this.Source.Count() / (double)this.PageSize);

				return _pageCount;
			}
		}

		public bool HasPrevious
		{
			get
			{
				return this.PageIndex > 1 && this.PageCount > 1;
			}
		}

		public bool HasNext
		{
			get
			{
				return this.PageIndex < this.PageCount;
			}
		}
		#endregion

		#region 私有方法
		private void ResetPageCount()
		{
			_pageCount = -1;
		}
		#endregion

		#region 嵌套子类
		public class PageRangeSettings
		{
			#region 成员变量
			private Pager<T> _paging;
			private int _rangeLength;
			#endregion

			#region 构造函数
			public PageRangeSettings(Pager<T> paging) : this(paging, 10)
			{
			}

			public PageRangeSettings(Pager<T> paging, int rangeLength)
			{
				if(paging == null)
					throw new ArgumentNullException("paging");

				_paging = paging;
				this.RangeLength = rangeLength;
			}
			#endregion

			#region 公共属性
			public Pager<T> Paging
			{
				get
				{
					return _paging;
				}
			}

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

			public int StartIndex
			{
				get
				{
					int startIndex, finishIndex;
					this.GetPageRange(out startIndex, out finishIndex);

					return startIndex;
				}
			}

			public int FinishIndex
			{
				get
				{
					int startIndex, finishIndex;
					this.GetPageRange(out startIndex, out finishIndex);

					return finishIndex;
				}
			}
			#endregion

			#region 公共方法
			public void GetPageRange(out int startIndex, out int finishIndex)
			{
				if(_paging.PageCount <= this.RangeLength)
				{
					startIndex = 1;
					finishIndex = _paging.PageCount;

					return;
				}

				int semiLength = (int)Math.Floor(this.RangeLength / 2.0);

				startIndex = _paging.PageIndex - semiLength;
				finishIndex = _paging.PageIndex + semiLength;

				if(startIndex < 1)
				{
					finishIndex -= startIndex - 1;
					startIndex = 1;

					return;
				}

				if(finishIndex > _paging.PageCount)
				{
					startIndex -= finishIndex - _paging.PageCount;
					finishIndex = _paging.PageCount;
				}
			}
			#endregion
		}
		#endregion
	}
}
