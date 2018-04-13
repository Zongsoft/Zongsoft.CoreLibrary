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
	public class DataCountContext : DataAccessContextBase
	{
		#region 成员字段
		private int _result;
		private ICondition _condition;
		private string _includes;
		#endregion

		#region 构造函数
		public DataCountContext(IDataAccess dataAccess, string name, ICondition condition, string includes, object state = null) : base(dataAccess, name, DataAccessMethod.Count, state)
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
				_result = value;
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
				_condition = value;
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
				_includes = value;
			}
		}
		#endregion
	}

	public class DataExistenceContext : DataAccessContextBase
	{
		#region 成员字段
		private ICondition _condition;
		private bool _result;
		#endregion

		#region 构造函数
		public DataExistenceContext(IDataAccess dataAccess, string name, ICondition condition, object state = null) : base(dataAccess, name, DataAccessMethod.Exists, state)
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
				_condition = value;
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
				_result = value;
			}
		}
		#endregion
	}

	public class DataExecutionContext : DataAccessContextBase
	{
		#region 成员字段
		private bool _isScalar;
		private object _result;
		private Type _resultType;
		private IDictionary<string, object> _inParameters;
		private IDictionary<string, object> _outParameters;
		#endregion

		#region 构造函数
		public DataExecutionContext(IDataAccess dataAccess, string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters, object state = null) : base(dataAccess, name, DataAccessMethod.Execute, state)
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
				if(value == null)
					throw new ArgumentNullException();

				_resultType = value;
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
				_result = value;
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
				_outParameters = value;
			}
		}
		#endregion
	}

	public class DataIncrementContext : DataAccessContextBase
	{
		#region 成员字段
		private string _member;
		private ICondition _condition;
		private int _interval;
		private long _result;
		#endregion

		#region 构造函数
		public DataIncrementContext(IDataAccess dataAccess, string name, string member, ICondition condition, int interval = 1, object state = null) : base(dataAccess, name, DataAccessMethod.Increment, state)
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

				_member = value;
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
				if(value == null)
					throw new ArgumentNullException();

				_condition = value;
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
				_interval = value;
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
				_result = value;
			}
		}
		#endregion
	}

	public class DataSelectionContext : DataAccessContextBase
	{
		#region 成员字段
		private Type _entityType;
		private IEnumerable _result;
		private IEnumerable _filteringResult;
		private ICondition _condition;
		private string _scope;
		private Paging _paging;
		private Grouping _grouping;
		private Sorting[] _sortings;
		private Func<DataSelectionContext, object, bool> _resultFilter;
		#endregion

		#region 构造函数
		public DataSelectionContext(IDataAccess dataAccess, string name, Type entityType, Grouping grouping, ICondition condition, string scope, Paging paging, Sorting[] sortings, object state = null) : base(dataAccess, name, DataAccessMethod.Select, state)
		{
			_entityType = entityType ?? typeof(object);
			_grouping = grouping;
			_condition = condition;
			_scope = scope;
			_paging = paging;
			_sortings = sortings;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取查询要返回的实体类型。
		/// </summary>
		public Type EntityType
		{
			get
			{
				return _entityType;
			}
		}

		/// <summary>
		/// 获取或设置查询操作的结果。
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
							var type = typeof(DataFilterEnumerable<,>).MakeGenericType(this.GetType(), _entityType);
							_filteringResult = (IEnumerable)System.Activator.CreateInstance(type, new object[] { this, _result, _resultFilter });
						}
					}
				}

				return _filteringResult;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_result = value;
				_filteringResult = null;
			}
		}

		/// <summary>
		/// 获取或设置查询结果的过滤器。
		/// </summary>
		public Func<DataSelectionContext, object, bool> ResultFilter
		{
			get
			{
				return _resultFilter;
			}
			set
			{
				_resultFilter = value;
				_filteringResult = null;
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
				_condition = value;
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
				_grouping = value;
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
				_paging = value;
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
				_scope = value;
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
				_sortings = value;
			}
		}
		#endregion
	}

	public class DataDeletionContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private ICondition _condition;
		private string[] _cascades;
		#endregion

		#region 构造函数
		public DataDeletionContext(IDataAccess dataAccess, string name, ICondition condition, string[] cascades, object state = null) : base(dataAccess, name, DataAccessMethod.Delete, state)
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
				_count = value;
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
				_condition = value;
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
				_cascades = value;
			}
		}
		#endregion
	}

	public class DataInsertionContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private string _scope;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		public DataInsertionContext(IDataAccess dataAccess, string name, bool isMultiple, object data, string scope, object state = null) : base(dataAccess, name, DataAccessMethod.Insert, state)
		{
			_data = data;
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
				_count = value;
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
				_data = value;
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
				_scope = value;
			}
		}
		#endregion
	}

	public class DataUpdationContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private ICondition _condition;
		private string _scope;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		public DataUpdationContext(IDataAccess dataAccess, string name, bool isMultiple, object data, ICondition condition, string scope, object state = null) : base(dataAccess, name, DataAccessMethod.Update, state)
		{
			_data = data;
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
				_count = value;
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
				_data = value;
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
				_condition = value;
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
				_scope = value;
			}
		}
		#endregion
	}

	public class DataUpsertionContext : DataAccessContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private ICondition _condition;
		private string _scope;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		public DataUpsertionContext(IDataAccess dataAccess, string name, bool isMultiple, object data, ICondition condition, string scope, object state = null) : base(dataAccess, name, DataAccessMethod.Update, state)
		{
			_data = data;
			_condition = condition;
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
				_count = value;
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
				_data = value;
			}
		}

		/// <summary>
		/// 获取或设置操作的条件。
		/// </summary>
		public ICondition Condition
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
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
				_scope = value;
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
