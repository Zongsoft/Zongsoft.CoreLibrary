/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class DataCountContextBase : DataAccessContextBase
	{
		#region 成员字段
		private int _result;
		private ICondition _condition;
		private string _includes;
		#endregion

		#region 构造函数
		protected DataCountContextBase(IDataAccess dataAccess, string name, ICondition condition, string includes, object state = null) : base(dataAccess, name, DataAccessMethod.Count, state)
		{
			_condition = condition;
			_includes = includes;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置计数操作的结果。
		/// </summary>
		public int Result
		{
			get
			{
				return _result;
			}
			set
			{
				if(_result == value)
					return;

				_result = value;
				this.OnPropertyChanged(nameof(Result));
			}
		}

		/// <summary>
		/// 获取或设置计数操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(_condition == value)
					return;

				_condition = value;
				this.OnPropertyChanged(nameof(Condition));
			}
		}

		/// <summary>
		/// 获取或设置计数操作的包含成员。
		/// </summary>
		public string Includes
		{
			get
			{
				return _includes;
			}
			set
			{
				if(_includes == value)
					return;

				_includes = value;
				this.OnPropertyChanged(nameof(Includes));
			}
		}
		#endregion
	}

	public class DataExistContextBase : DataAccessContextBase
	{
		#region 成员字段
		private ICondition _condition;
		private bool _result;
		#endregion

		#region 构造函数
		protected DataExistContextBase(IDataAccess dataAccess, string name, ICondition condition, object state = null) : base(dataAccess, name, DataAccessMethod.Exists, state)
		{
			_condition = condition;
		}
		#endregion

		#region 公共属性
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(_condition == value)
					return;

				_condition = value;
				this.OnPropertyChanged(nameof(Condition));
			}
		}

		public bool Result
		{
			get
			{
				return _result;
			}
			set
			{
				if(_result == value)
					return;

				_result = value;
				this.OnPropertyChanged(nameof(Result));
			}
		}
		#endregion
	}

	public class DataExecuteContextBase : DataAccessContextBase
	{
		#region 成员字段
		private bool _isScalar;
		private object _result;
		private Type _resultType;
		private IDictionary<string, object> _inParameters;
		private IDictionary<string, object> _outParameters;
		#endregion

		#region 构造函数
		protected DataExecuteContextBase(IDataAccess dataAccess, string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters, object state = null) : base(dataAccess, name, DataAccessMethod.Execute, state)
		{
			if(resultType == null)
				throw new ArgumentNullException(nameof(resultType));

			_isScalar = isScalar;
			_resultType = resultType;
			_inParameters = inParameters;
			_outParameters = outParameters;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示是否为返回单值。
		/// </summary>
		public bool IsScalar
		{
			get
			{
				return _isScalar;
			}
		}

		/// <summary>
		/// 获取或设置执行结果的类型。
		/// </summary>
		public Type ResultType
		{
			get
			{
				return _resultType;
			}
			set
			{
				if(_resultType == value)
					return;

				_resultType = value ?? throw new ArgumentNullException();
				this.OnPropertyChanged(nameof(ResultType));
			}
		}

		/// <summary>
		/// 获取或设置执行操作的结果。
		/// </summary>
		public object Result
		{
			get
			{
				return _result;
			}
			set
			{
				if(_result == value)
					return;

				_result = value;
				this.OnPropertyChanged(nameof(Result));
			}
		}

		/// <summary>
		/// 获取执行操作的输入参数。
		/// </summary>
		public IDictionary<string, object> InParameters
		{
			get
			{
				return _inParameters;
			}
		}

		/// <summary>
		/// 获取或设置执行操作的输出参数。
		/// </summary>
		public IDictionary<string, object> OutParameters
		{
			get
			{
				return _outParameters;
			}
			set
			{
				if(_outParameters == value)
					return;

				_outParameters = value;
				this.OnPropertyChanged(nameof(OutParameters));
			}
		}
		#endregion
	}

	public class DataIncrementContextBase : DataAccessContextBase
	{
		#region 成员字段
		private string _member;
		private ICondition _condition;
		private int _interval;
		private long _result;
		#endregion

		#region 构造函数
		protected DataIncrementContextBase(IDataAccess dataAccess, string name, string member, ICondition condition, int interval = 1, object state = null) : base(dataAccess, name, DataAccessMethod.Increment, state)
		{
			if(string.IsNullOrEmpty(member))
				throw new ArgumentNullException(nameof(member));

			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			_member = member;
			_condition = condition;
			_interval = interval;
		}
		#endregion

		#region 公共属性
		public string Member
		{
			get
			{
				return _member;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				if(_member == value)
					return;

				_member = value;
				this.OnPropertyChanged(nameof(Member));
			}
		}

		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(_condition == value)
					return;

				_condition = value ?? throw new ArgumentNullException();
				this.OnPropertyChanged(nameof(Condition));
			}
		}

		public int Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				if(_interval == value)
					return;

				_interval = value;
				this.OnPropertyChanged(nameof(Interval));
			}
		}

		public long Result
		{
			get
			{
				return _result;
			}
			set
			{
				if(_result == value)
					return;

				_result = value;
				this.OnPropertyChanged(nameof(Result));
			}
		}
		#endregion
	}

	public class DataSelectContextBase : DataAccessContextBase
	{
		#region 成员字段
		private Type _elementType;
		private IEnumerable _result;
		private IEnumerable _filteringResult;
		private ICondition _condition;
		private string _scope;
		private Paging _paging;
		private Grouping _grouping;
		private Sorting[] _sortings;
		private Func<DataSelectContextBase, object, bool> _resultFilter;
		#endregion

		#region 构造函数
		protected DataSelectContextBase(IDataAccess dataAccess, string name, Type elementType, Grouping grouping, ICondition condition, string scope, Paging paging, Sorting[] sortings, object state = null) : base(dataAccess, name, DataAccessMethod.Select, state)
		{
			_elementType = elementType ?? typeof(object);
			_grouping = grouping;
			_condition = condition;
			_scope = scope;
			_paging = paging;
			_sortings = sortings;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取查询要返回的结果集元素类型。
		/// </summary>
		public Type ElementType
		{
			get
			{
				return _elementType;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的结果集。
		/// </summary>
		public IEnumerable Result
		{
			get
			{
				var filter = _resultFilter;

				if(filter == null)
					return _result;

				if(_filteringResult == null)
				{
					lock(this)
					{
						if(_filteringResult == null)
						{
							var type = typeof(DataFilterEnumerable<,>).MakeGenericType(this.GetType(), _elementType);
							_filteringResult = (IEnumerable)System.Activator.CreateInstance(type, new object[] { this, _result, _resultFilter });
						}
					}
				}

				return _filteringResult;
			}
			set
			{
				if(_result == value)
					return;

				_result = value ?? throw new ArgumentNullException();
				_filteringResult = null;
				this.OnPropertyChanged(nameof(Result));
			}
		}

		/// <summary>
		/// 获取或设置查询结果的过滤器。
		/// </summary>
		public Func<DataSelectContextBase, object, bool> ResultFilter
		{
			get
			{
				return _resultFilter;
			}
			set
			{
				if(_resultFilter == value)
					return;

				_resultFilter = value;
				_filteringResult = null;
				this.OnPropertyChanged(nameof(ResultFilter));
			}
		}

		/// <summary>
		/// 获取或设置查询操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(_condition == value)
					return;

				_condition = value;
				this.OnPropertyChanged(nameof(Condition));
			}
		}

		/// <summary>
		/// 获取或设置查询操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				if(_scope == value)
					return;

				_scope = value;
				this.OnPropertyChanged(nameof(Scope));
			}
		}

		/// <summary>
		/// 获取或设置查询操作的分组。
		/// </summary>
		public Grouping Grouping
		{
			get
			{
				return _grouping;
			}
			set
			{
				if(_grouping == value)
					return;

				_grouping = value;
				this.OnPropertyChanged(nameof(Grouping));
			}
		}

		/// <summary>
		/// 获取或设置查询操作的分页设置。
		/// </summary>
		public Paging Paging
		{
			get
			{
				return _paging;
			}
			set
			{
				if(_paging == value)
					return;

				_paging = value;
				this.OnPropertyChanged(nameof(Paging));
			}
		}

		/// <summary>
		/// 获取或设置查询操作的排序设置。
		/// </summary>
		public Sorting[] Sortings
		{
			get
			{
				return _sortings;
			}
			set
			{
				if(_sortings == value)
					return;

				_sortings = value;
				this.OnPropertyChanged(nameof(Sortings));
			}
		}
		#endregion
	}

	public class DataDeleteContextBase : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private ICondition _condition;
		private string[] _cascades;
		#endregion

		#region 构造函数
		protected DataDeleteContextBase(IDataAccess dataAccess, string name, ICondition condition, string[] cascades, object state = null) : base(dataAccess, name, DataAccessMethod.Delete, state)
		{
			_condition = condition;
			_cascades = cascades;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置删除操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				if(_count == value)
					return;

				_count = value;
				this.OnPropertyChanged(nameof(Count));
			}
		}

		/// <summary>
		/// 获取或设置删除操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(_condition == value)
					return;

				_condition = value;
				this.OnPropertyChanged(nameof(Condition));
			}
		}

		/// <summary>
		/// 获取或设置删除操作的关联成员。
		/// </summary>
		public string[] Cascades
		{
			get
			{
				return _cascades;
			}
			set
			{
				if(_cascades == value)
					return;

				_cascades = value;
				this.OnPropertyChanged(nameof(Cascades));
			}
		}
		#endregion
	}

	public class DataInsertContextBase : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private string _scope;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		protected DataInsertContextBase(IDataAccess dataAccess, string name, bool isMultiple, object data, string scope, object state = null) : base(dataAccess, name, DataAccessMethod.Insert, state)
		{
			_data = data ?? throw new ArgumentNullException(nameof(data));
			_scope = scope;
			_isMultiple = isMultiple;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示是否为批量新增操作。
		/// </summary>
		public bool IsMultiple
		{
			get
			{
				return _isMultiple;
			}
		}

		/// <summary>
		/// 获取或设置插入操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				if(_count == value)
					return;

				_count = value;
				this.OnPropertyChanged(nameof(Count));
			}
		}

		/// <summary>
		/// 获取或设置插入操作的数据。
		/// </summary>
		public object Data
		{
			get
			{
				return _data;
			}
			set
			{
				if(_data == value)
					return;

				_data = value;
				this.OnPropertyChanged(nameof(Data));
			}
		}

		/// <summary>
		/// 获取插入数据的元素类型。
		/// </summary>
		public virtual Type ElementType
		{
			get
			{
				if(_isMultiple && _data is IEnumerable)
					return Common.TypeExtension.GetElementType(_data.GetType());

				return _data.GetType();
			}
		}

		/// <summary>
		/// 获取或设置插入操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				if(_scope == value)
					return;

				_scope = value;
				this.OnPropertyChanged(nameof(Scope));
			}
		}
		#endregion
	}

	public class DataUpdateContextBase : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private ICondition _condition;
		private string _scope;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		protected DataUpdateContextBase(IDataAccess dataAccess, string name, bool isMultiple, object data, ICondition condition, string scope, object state = null) : base(dataAccess, name, DataAccessMethod.Update, state)
		{
			_data = data ?? throw new ArgumentNullException(nameof(data));
			_condition = condition;
			_scope = scope;
			_isMultiple = isMultiple;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示是否为批量更新操作。
		/// </summary>
		public bool IsMultiple
		{
			get
			{
				return _isMultiple;
			}
		}

		/// <summary>
		/// 获取或设置更新操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				if(_count == value)
					return;

				_count = value;
				this.OnPropertyChanged(nameof(Count));
			}
		}

		/// <summary>
		/// 获取或设置更新操作的数据。
		/// </summary>
		public object Data
		{
			get
			{
				return _data;
			}
			set
			{
				if(_data == value)
					return;

				_data = value;
				this.OnPropertyChanged(nameof(Data));
			}
		}

		/// <summary>
		/// 获取更新数据的元素类型。
		/// </summary>
		public virtual Type ElementType
		{
			get
			{
				if(_isMultiple && _data is IEnumerable)
					return Common.TypeExtension.GetElementType(_data.GetType());

				return _data.GetType();
			}
		}

		/// <summary>
		/// 获取或设置更新操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				if(_condition == value)
					return;

				_condition = value;
				this.OnPropertyChanged(nameof(Condition));
			}
		}

		/// <summary>
		/// 获取或设置更新操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				if(_scope == value)
					return;

				_scope = value;
				this.OnPropertyChanged(nameof(Scope));
			}
		}
		#endregion
	}

	public class DataUpsertContextBase : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private string _scope;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		protected DataUpsertContextBase(IDataAccess dataAccess, string name, bool isMultiple, object data, string scope, object state = null) : base(dataAccess, name, DataAccessMethod.Upsert, state)
		{
			_data = data ?? throw new ArgumentNullException(nameof(data));
			_scope = scope;
			_isMultiple = isMultiple;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取一个值，指示是否为批量操作。
		/// </summary>
		public bool IsMultiple
		{
			get
			{
				return _isMultiple;
			}
		}

		/// <summary>
		/// 获取或设置操作的受影响记录数。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
			set
			{
				if(_count == value)
					return;

				_count = value;
				this.OnPropertyChanged(nameof(Count));
			}
		}

		/// <summary>
		/// 获取或设置操作的数据。
		/// </summary>
		public object Data
		{
			get
			{
				return _data;
			}
			set
			{
				if(_data == value)
					return;

				_data = value;
				this.OnPropertyChanged(nameof(Data));
			}
		}

		/// <summary>
		/// 获取操作数据的元素类型。
		/// </summary>
		public virtual Type ElementType
		{
			get
			{
				if(_isMultiple && _data is IEnumerable)
					return Common.TypeExtension.GetElementType(_data.GetType());

				return _data.GetType();
			}
		}

		/// <summary>
		/// 获取或设置操作的包含成员。
		/// </summary>
		public string Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				if(_scope == value)
					return;

				_scope = value;
				this.OnPropertyChanged(nameof(Scope));
			}
		}
		#endregion
	}

	#region 过滤遍历器类
	internal class DataFilterEnumerable<TContext, TEntity> : IEnumerable<TEntity> where TContext : DataAccessContextBase
	{
		private TContext _context;
		private IEnumerable _source;
		private Func<TContext, object, bool> _filter;

		public DataFilterEnumerable(TContext context, IEnumerable source, Func<TContext, object, bool> filter)
		{
			if(context == null)
				throw new ArgumentNullException(nameof(context));
			if(source == null)
				throw new ArgumentNullException(nameof(source));

			_context = context;
			_source = source;
			_filter = filter;
		}

		public IEnumerator<TEntity> GetEnumerator()
		{
			return new DataFilterIterator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new DataFilterIterator(this);
		}

		private class DataFilterIterator : IEnumerator<TEntity>
		{
			private DataFilterEnumerable<TContext, TEntity> _owner;
			private IEnumerator _iterator;

			public DataFilterIterator(DataFilterEnumerable<TContext, TEntity> owner)
			{
				_owner = owner;
				_iterator = owner._source.GetEnumerator();
			}

			public TEntity Current
			{
				get
				{
					var iterator = _iterator;

					if(iterator == null)
						throw new ObjectDisposedException(this.GetType().FullName);

					return (TEntity)iterator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					var iterator = _iterator;

					if(iterator == null)
						throw new ObjectDisposedException(this.GetType().FullName);

					return iterator.Current;
				}
			}

			public bool MoveNext()
			{
				var iterator = _iterator;

				if(iterator == null)
					throw new ObjectDisposedException(this.GetType().FullName);

				var filter = _owner._filter;

				while(iterator.MoveNext())
				{
					if(filter == null || filter(_owner._context, iterator.Current))
						return true;
				}

				return false;
			}

			public void Reset()
			{
				var iterator = _iterator;

				if(iterator == null)
					throw new ObjectDisposedException(this.GetType().FullName);

				iterator.Reset();
			}

			public void Dispose()
			{
				_iterator = null;
				_owner = null;
			}
		}
	}
	#endregion
}
