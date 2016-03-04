/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public class DataService<TEntity> : IDataService<TEntity>
	{
		#region 成员字段
		private string _name;
		private IDataAccess _dataAccess;
		private Zongsoft.Services.IServiceProvider _serviceProvider;
		#endregion

		#region 构造函数
		public DataService(IDataAccess dataAccess)
		{
			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			_dataAccess = dataAccess;
		}

		public DataService(string name, IDataAccess dataAccess)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(dataAccess == null)
				throw new ArgumentNullException("dataAccess");

			_name = name.Trim();
			_dataAccess = dataAccess;
		}

		public DataService(Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_serviceProvider = serviceProvider;
		}

		public DataService(string name, Zongsoft.Services.IServiceProvider serviceProvider)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if(serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			_name = name.Trim();
			_serviceProvider = serviceProvider;
		}
		#endregion

		#region 公共属性
		public virtual string Name
		{
			get
			{
				if(string.IsNullOrWhiteSpace(_name))
				{
					var attribute = (Metadata.DataEntityAttribute)Attribute.GetCustomAttribute(typeof(TEntity), typeof(Metadata.DataEntityAttribute));
					_name = attribute == null ? typeof(TEntity).Name : attribute.Name;
				}

				return _name;
			}
		}

		public virtual IDataAccess DataAccess
		{
			get
			{
				if(_dataAccess == null)
				{
					var serviceProvider = this.ServiceProvider;

					if(serviceProvider != null)
						_dataAccess = serviceProvider.Resolve<IDataAccess>();
				}

				return _dataAccess;
			}
		}

		public virtual Zongsoft.Services.IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
			set
			{
				_serviceProvider = value;
			}
		}
		#endregion

		#region 执行方法
		public object Execute(IDictionary<string, object> inParameters)
		{
			return this.EnsureDataAccess().Execute(this.Name, inParameters);
		}

		public object Execute(IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			return this.EnsureDataAccess().Execute(this.Name, inParameters, out outParameters);
		}
		#endregion

		#region 存在方法
		public bool Exists(ICondition condition)
		{
			return this.EnsureDataAccess().Exists(this.Name, condition);
		}
		#endregion

		#region 计数方法
		public int Count(ICondition condition, string includes = null)
		{
			return this.EnsureDataAccess().Count(this.Name, condition, includes);
		}

		public int Count(ICondition condition, Expression<Func<TEntity, object>> includes)
		{
			return this.EnsureDataAccess().Count(this.Name, condition, includes);
		}
		#endregion

		#region 查询方法
		public object Get<TKey>(TKey key, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key), sorting);
			return this.GetResult(result, 1);
		}

		public object Get<TKey>(TKey key, string scope, Paging paging = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key), scope, paging, sorting);
			return this.GetResult(result, 1);
		}

		public object Get<TKey>(TKey key, Paging paging, string scope = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key), paging, scope, sorting);
			return this.GetResult(result, 1);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2), sorting);
			return this.GetResult(result, 2);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string scope, Paging paging = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2), scope, paging, sorting);
			return this.GetResult(result, 2);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string scope = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2), paging, scope, sorting);
			return this.GetResult(result, 2);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2, key3), sorting);
			return this.GetResult(result, 3);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string scope, Paging paging = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2, key3), scope, paging, sorting);
			return this.GetResult(result, 3);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string scope = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2, key3), paging, scope, sorting);
			return this.GetResult(result, 3);
		}

		public IEnumerable<TEntity> Select(ICondition condition = null, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, scope, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, scope, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, Grouping grouping, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, scope, grouping, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, paging, scope, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, grouping, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, grouping, scope, sorting);
		}
		#endregion

		#region 删除方法
		public int Delete<TKey>(TKey key)
		{
			return this.EnsureDataAccess().Delete(this.Name, this.ConvertKey(key));
		}

		public int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2)
		{
			return this.EnsureDataAccess().Delete(this.Name, this.ConvertKey(key1, key2));
		}

		public int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3)
		{
			return this.EnsureDataAccess().Delete(this.Name, this.ConvertKey(key1, key2, key3));
		}

		public int Delete(ICondition condition)
		{
			return this.EnsureDataAccess().Delete(this.Name, condition);
		}

		public int Delete(ICondition condition, string cascades)
		{
			return this.EnsureDataAccess().Delete(this.Name, condition, cascades);
		}

		public int Delete(ICondition condition, Expression<Func<TEntity, object>> cascades)
		{
			return this.EnsureDataAccess().Delete(this.Name, condition, cascades);
		}
		#endregion

		#region 插入方法
		public int Insert(TEntity entity, string scope = null)
		{
			return this.EnsureDataAccess().Insert(this.Name, entity, scope);
		}

		public int Insert(TEntity entity, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes)
		{
			return this.EnsureDataAccess().Insert(this.Name, entity, includes, excludes);
		}

		public int Insert(IEnumerable<TEntity> entities, string scope = null)
		{
			return this.EnsureDataAccess().Insert(this.Name, entities, scope);
		}

		public int Insert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null)
		{
			return this.EnsureDataAccess().Insert(this.Name, entities, includes, excludes);
		}
		#endregion

		#region 修改方法
		public int Update(TEntity entity, ICondition condition = null, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entity, condition, scope);
		}

		public int Update(TEntity entity, ICondition condition, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entity, condition, includes, excludes);
		}

		public int Update(IEnumerable<TEntity> entities, ICondition condition = null, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entities, condition, scope);
		}

		public int Update(IEnumerable<TEntity> entities, ICondition condition, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entities, condition, includes, excludes);
		}
		#endregion

		#region 条件转换
		protected virtual ICondition ConvertKey<TKey>(TKey key)
		{
			var properties = this.GetEntityKeys();

			if(properties.Length < 1)
				throw new InvalidOperationException("Missing key(s) about '" + typeof(TEntity).FullName + "' type.");

			return Condition.Equal(properties[0].Name, key);
		}

		protected virtual ICondition ConvertKey<TKey1, TKey2>(TKey1 key1, TKey2 key2)
		{
			var properties = this.GetEntityKeys();

			if(properties.Length < 2)
				throw new InvalidOperationException("Not match keys about '" + typeof(TEntity).FullName + "' type.");

			return Condition.Equal(properties[0].Name, key1) & Condition.Equal(properties[1].Name, key2);
		}

		protected virtual ICondition ConvertKey<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3)
		{
			var properties = this.GetEntityKeys();

			if(properties.Length < 3)
				throw new InvalidOperationException("Not match keys about '" + typeof(TEntity).FullName + "' type.");

			return Condition.Equal(properties[0].Name, key1) & Condition.Equal(properties[1].Name, key2) & Condition.Equal(properties[2].Name, key3);
		}
		#endregion

		#region 私有方法
		private IDataAccess EnsureDataAccess()
		{
			var result = this.DataAccess;

			if(result == null)
				throw new InvalidOperationException("The value of 'DataAccess' property is null.");

			return result;
		}

		private Property[] GetEntityKeys()
		{
			var result = new List<Property>();
			var properties = typeof(TEntity).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			foreach(var property in properties)
			{
				if(Attribute.IsDefined(property, typeof(Metadata.DataEntityKeyAttribute), true))
					result.Add(new Property(property.Name, property.PropertyType));
			}

			return result.ToArray();
		}

		private object GetResult(IEnumerable<TEntity> result, int keyCount)
		{
			if(this.GetEntityKeys().Length == keyCount)
				return result.FirstOrDefault();

			return result;
		}
		#endregion

		#region 嵌套子类
		private class Property
		{
			public Property(string name, Type propertyType)
			{
				this.Name = name;
				this.PropertyType = propertyType;
			}

			public string Name;
			public Type PropertyType;
		}
		#endregion
	}
}
