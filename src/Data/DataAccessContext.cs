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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class DataCountContextBase : DataAccessContextBase
	{
		#region 成员字段
		private int _result;
		private ICondition _condition;
		private string _member;
		#endregion

		#region 构造函数
		protected DataCountContextBase(IDataAccess dataAccess, string name, ICondition condition, string member, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Count, states)
		{
			_condition = condition;
			_member = member;
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

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
		/// 获取当前计数操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
		}

		/// <summary>
		/// 获取或设置计数操作的包含成员。
		/// </summary>
		public string Member
		{
			get
			{
				return _member;
			}
			set
			{
				if(_member == value)
					return;

				_member = value;
				this.OnPropertyChanged(nameof(Member));
			}
		}
		#endregion

		#region 公共方法
		public ICondition Validate(ICondition criteria = null)
		{
			var validator = this.Validator;

			return validator == null ?
				criteria ?? this.Condition :
				validator.Validate(this, criteria ?? this.Condition);
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
		protected DataExistContextBase(IDataAccess dataAccess, string name, ICondition condition, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Exists, states)
		{
			_condition = condition;
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

		/// <summary>
		/// 获取或设置判断操作的条件。
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
		/// 获取当前判断操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
		}

		/// <summary>
		/// 获取判断操作的结果，即指定条件的数据是否存在。
		/// </summary>
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

		#region 公共方法
		public ICondition Validate(ICondition criteria = null)
		{
			var validator = this.Validator;

			return validator == null ?
				criteria ?? this.Condition :
				validator.Validate(this, criteria ?? this.Condition);
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
		protected DataExecuteContextBase(IDataAccess dataAccess, string name, bool isScalar, Type resultType, IDictionary<string, object> inParameters, IDictionary<string, object> outParameters, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Execute, states)
		{
			_isScalar = isScalar;
			_resultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
			_inParameters = inParameters;
			_outParameters = outParameters;
			this.Command = DataContextUtility.GetCommand(name, dataAccess.Metadata);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的命令元数据。
		/// </summary>
		public Metadata.IDataCommand Command
		{
			get;
		}

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

	public class DataIncrementContextBase : DataAccessContextBase, IDataMutateContextBase
	{
		#region 成员字段
		private int _count;
		private string _member;
		private ICondition _condition;
		private int _interval;
		private long _result;
		#endregion

		#region 构造函数
		protected DataIncrementContextBase(IDataAccess dataAccess, string name, string member, ICondition condition, int interval = 1, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Increment, states)
		{
			if(string.IsNullOrEmpty(member))
				throw new ArgumentNullException(nameof(member));

			if(condition == null)
				throw new ArgumentNullException(nameof(condition));

			_member = member;
			_interval = interval;
			_condition = condition ?? throw new ArgumentNullException(nameof(condition));
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

		public int Count
		{
			get => _count;
			set
			{
				if(_count == value)
					return;

				_count = value;
				this.OnPropertyChanged(nameof(Count));
			}
		}

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

		/// <summary>
		/// 获取当前递增(减)操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
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

		#region 公共方法
		public ICondition Validate(ICondition criteria = null)
		{
			var validator = this.Validator;

			return validator == null ?
				criteria ?? this.Condition :
				validator.Validate(this, criteria ?? this.Condition);
		}
		#endregion

		#region 显式实现
		object IDataMutateContextBase.Data
		{
			get => this.Interval;
			set
			{
				if(Common.Convert.TryConvertValue<int>(value, out var interval))
					this.Interval = interval;
			}
		}

		bool IDataMutateContextBase.IsMultiple
		{
			get => false;
		}
		#endregion
	}

	public class DataSelectContextBase : DataAccessContextBase
	{
		#region 委托定义
		public delegate bool FilterDelegate(DataSelectContextBase context, ref object data);
		#endregion

		#region 成员字段
		private IEnumerable _result;
		private ICondition _condition;
		private ISchema _schema;
		private Paging _paging;
		private Grouping _grouping;
		private Sorting[] _sortings;
		private FilterDelegate _resultFilter;
		#endregion

		#region 构造函数
		protected DataSelectContextBase(IDataAccess dataAccess, string name, Type entityType, Grouping grouping, ICondition condition, ISchema schema, Paging paging, Sorting[] sortings, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Select, states)
		{
			_grouping = grouping;
			_condition = condition;
			_schema = schema;
			_paging = paging;
			_sortings = sortings;
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.EntityType = entityType ?? typeof(object);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

		/// <summary>
		/// 获取查询要返回的结果集元素类型。
		/// </summary>
		public Type EntityType
		{
			get;
		}

		/// <summary>
		/// 获取或设置查询操作的结果集。
		/// </summary>
		public IEnumerable Result
		{
			get
			{
				bool OnFilter(ref object data)
				{
					return _resultFilter(this, ref data);
				}

				if(_resultFilter != null && _result is IPageable source)
					Pageable.Filter(source, this.EntityType, OnFilter);

				return _result;
			}
			set
			{
				if(_result == value)
					return;

				_result = value ?? throw new ArgumentNullException();
				this.OnPropertyChanged(nameof(Result));
			}
		}

		/// <summary>
		/// 获取或设置查询结果的过滤器。
		/// </summary>
		public FilterDelegate ResultFilter
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
		/// 获取当前查询操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
		}

		/// <summary>
		/// 获取或设置查询操作的结果数据模式（即查询结果的形状结构）。
		/// </summary>
		public ISchema Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				if(_schema == value)
					return;

				_schema = value;
				this.OnPropertyChanged(nameof(Schema));
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

		#region 公共方法
		public ICondition Validate(ICondition criteria = null)
		{
			var validator = this.Validator;

			return validator == null ?
				criteria ?? this.Condition :
				validator.Validate(this, criteria ?? this.Condition);
		}
		#endregion
	}

	public class DataDeleteContextBase : DataAccessContextBase, IDataMutateContextBase
	{
		#region 成员字段
		private int _count;
		private ICondition _condition;
		private ISchema _schema;
		#endregion

		#region 构造函数
		protected DataDeleteContextBase(IDataAccess dataAccess, string name, ICondition condition, ISchema schema, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Delete, states)
		{
			_condition = condition;
			_schema = schema;
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

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
		/// 获取当前删除操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
		}

		/// <summary>
		/// 获取或设置删除操作的数据模式（即删除数据的形状结构）。
		/// </summary>
		public ISchema Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				if(_schema == value)
					return;

				_schema = value;
				this.OnPropertyChanged(nameof(Schema));
			}
		}
		#endregion

		#region 公共方法
		public ICondition Validate(ICondition criteria = null)
		{
			var validator = this.Validator;

			return validator == null ?
				criteria ?? this.Condition :
				validator.Validate(this, criteria ?? this.Condition);
		}
		#endregion

		#region 显式实现
		object IDataMutateContextBase.Data
		{
			get => null;
			set { }
		}

		bool IDataMutateContextBase.IsMultiple
		{
			get => false;
		}
		#endregion
	}

	public class DataInsertContextBase : DataAccessContextBase, IDataMutateContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private ISchema _schema;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		protected DataInsertContextBase(IDataAccess dataAccess, string name, bool isMultiple, object data, ISchema schema, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Insert, states)
		{
			_data = data ?? throw new ArgumentNullException(nameof(data));
			_schema = schema;
			_isMultiple = isMultiple;
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

		/// <summary>
		/// 获取当前新增操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
		}

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
		public virtual Type EntityType
		{
			get
			{
				if(_isMultiple && _data is IEnumerable)
					return Common.TypeExtension.GetElementType(_data.GetType());

				return _data.GetType();
			}
		}

		/// <summary>
		/// 获取或设置插入操作的数据模式（即插入的数据形状结构）。
		/// </summary>
		public ISchema Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				if(_schema == value)
					return;

				_schema = value;
				this.OnPropertyChanged(nameof(Schema));
			}
		}
		#endregion

		#region 公共方法
		public bool Validate(Metadata.IDataEntityProperty property, out object value)
		{
			return this.Validate(DataAccessMethod.Insert, property, out value);
		}

		public bool Validate(DataAccessMethod method, Metadata.IDataEntityProperty property, out object value)
		{
			var validator = this.Validator;

			if(validator != null)
			{
				switch(method)
				{
					case DataAccessMethod.Insert:
						return validator.OnInsert(this, property, out value);
					case DataAccessMethod.Update:
						return validator.OnUpdate(this, property, out value);
				}
			}

			value = null;
			return false;
		}
		#endregion
	}

	public class DataUpdateContextBase : DataAccessContextBase, IDataMutateContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private ICondition _condition;
		private ISchema _schema;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		protected DataUpdateContextBase(IDataAccess dataAccess, string name, bool isMultiple, object data, ICondition condition, ISchema schema, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Update, states)
		{
			_data = data ?? throw new ArgumentNullException(nameof(data));
			_condition = condition;
			_schema = schema;
			_isMultiple = isMultiple;
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

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
		public virtual Type EntityType
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
		/// 获取当前更新操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
		}

		/// <summary>
		/// 获取或设置更新操作的数据模式（即更新的数据形状结构）。
		/// </summary>
		public ISchema Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				if(_schema == value)
					return;

				_schema = value;
				this.OnPropertyChanged(nameof(Schema));
			}
		}
		#endregion

		#region 公共方法
		public ICondition Validate(ICondition criteria = null)
		{
			var validator = this.Validator;

			return validator == null ?
				criteria ?? this.Condition :
				validator.Validate(this, criteria ?? this.Condition);
		}

		public bool Validate(Metadata.IDataEntityProperty property, out object value)
		{
			return this.Validate(DataAccessMethod.Update, property, out value);
		}

		public bool Validate(DataAccessMethod method, Metadata.IDataEntityProperty property, out object value)
		{
			var validator = this.Validator;

			if(validator != null)
			{
				switch(method)
				{
					case DataAccessMethod.Insert:
						return validator.OnInsert(this, property, out value);
					case DataAccessMethod.Update:
						return validator.OnUpdate(this, property, out value);
				}
			}

			value = null;
			return false;
		}
		#endregion
	}

	public class DataUpsertContextBase : DataAccessContextBase, IDataMutateContextBase
	{
		#region 成员字段
		private int _count;
		private object _data;
		private ISchema _schema;
		private bool _isMultiple;
		#endregion

		#region 构造函数
		protected DataUpsertContextBase(IDataAccess dataAccess, string name, bool isMultiple, object data, ISchema schema, IDictionary<string, object> states = null) : base(dataAccess, name, DataAccessMethod.Upsert, states)
		{
			_data = data ?? throw new ArgumentNullException(nameof(data));
			_schema = schema;
			_isMultiple = isMultiple;
			this.Entity = DataContextUtility.GetEntity(name, dataAccess.Metadata);
			this.Validator = dataAccess.Validator;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取数据访问对应的实体元数据。
		/// </summary>
		public Metadata.IDataEntity Entity
		{
			get;
		}

		/// <summary>
		/// 获取当前写入操作的验证器。
		/// </summary>
		public IDataValidator Validator
		{
			get;
		}

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
		public virtual Type EntityType
		{
			get
			{
				if(_isMultiple && _data is IEnumerable)
					return Common.TypeExtension.GetElementType(_data.GetType());

				return _data.GetType();
			}
		}

		/// <summary>
		/// 获取或设置操作的数据模式（即更新或新增的数据形状结构）。
		/// </summary>
		public ISchema Schema
		{
			get
			{
				return _schema;
			}
			set
			{
				if(_schema == value)
					return;

				_schema = value;
				this.OnPropertyChanged(nameof(Schema));
			}
		}
		#endregion

		#region 公共方法
		public bool Validate(DataAccessMethod method, Metadata.IDataEntityProperty property, out object value)
		{
			var validator = this.Validator;

			if(validator != null)
			{
				switch(method)
				{
					case DataAccessMethod.Insert:
						return validator.OnInsert(this, property, out value);
					case DataAccessMethod.Update:
						return validator.OnUpdate(this, property, out value);
				}
			}

			value = null;
			return false;
		}
		#endregion
	}

	internal static class DataContextUtility
	{
		public static Metadata.IDataEntity GetEntity(string name, Metadata.IDataMetadataContainer metadata)
		{
			if(metadata.Entities.TryGet(name, out var entity))
				return entity;

			throw new DataException($"The specified '{name}' entity mapping does not exist.");
		}

		public static Metadata.IDataCommand GetCommand(string name, Metadata.IDataMetadataContainer metadata)
		{
			if(metadata.Commands.TryGet(name, out var command))
				return command;

			throw new DataException($"The specified '{name}' command mapping does not exist.");
		}
	}

	#region 过滤遍历器类
	internal class DataFilterEnumerable<TContext, TEntity> : IEnumerable<TEntity> where TContext : IDataAccessContextBase
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
