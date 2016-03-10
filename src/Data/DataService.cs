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
		public string Name
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
			protected set
			{
				if(string.IsNullOrWhiteSpace(value))
					throw new ArgumentNullException();

				_name = value.Trim();
			}
		}

		[Zongsoft.Services.ServiceDependency]
		public IDataAccess DataAccess
		{
			get
			{
				return _dataAccess;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_dataAccess = value;
			}
		}

		public Zongsoft.Services.IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_serviceProvider = value;
			}
		}
		#endregion

		#region 执行方法
		public object Execute(IDictionary<string, object> inParameters)
		{
			IDictionary<string, object> outParameters;
			return this.Execute(inParameters, out outParameters);
		}

		public virtual object Execute(IDictionary<string, object> inParameters, out IDictionary<string, object> outParameters)
		{
			return this.EnsureDataAccess().Execute(this.Name, inParameters, out outParameters);
		}
		#endregion

		#region 存在方法
		public virtual bool Exists(ICondition condition)
		{
			return this.EnsureDataAccess().Exists(this.Name, condition);
		}
		#endregion

		#region 计数方法
		public virtual int Count(ICondition condition, string includes = null)
		{
			return this.EnsureDataAccess().Count(this.Name, condition, includes);
		}

		public int Count(ICondition condition, Expression<Func<TEntity, object>> includes)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 查询方法
		public object Get<TKey>(TKey key, params Sorting[] sorting)
		{
			return this.Get<TKey>(key, string.Empty, null, sorting);
		}

		public virtual object Get<TKey>(TKey key, string scope, Paging paging = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key), scope, paging, sorting);
			return this.GetResult(result, 1);
		}

		public object Get<TKey>(TKey key, Paging paging, string scope = null, params Sorting[] sorting)
		{
			return this.Get<TKey>(key, scope, paging, sorting);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, params Sorting[] sorting)
		{
			return this.Get<TKey1, TKey2>(key1, key2, string.Empty, null, sorting);
		}

		public virtual object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, string scope, Paging paging = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2), scope, paging, sorting);
			return this.GetResult(result, 2);
		}

		public object Get<TKey1, TKey2>(TKey1 key1, TKey2 key2, Paging paging, string scope = null, params Sorting[] sorting)
		{
			return this.Get<TKey1, TKey2>(key1, key2, scope, paging, sorting);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, params Sorting[] sorting)
		{
			return this.Get<TKey1, TKey2, TKey3>(key1, key2, key3, string.Empty, null, sorting);
		}

		public virtual object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string scope, Paging paging = null, params Sorting[] sorting)
		{
			var result = this.EnsureDataAccess().Select<TEntity>(this.Name, this.ConvertKey(key1, key2, key3), scope, paging, sorting);
			return this.GetResult(result, 3);
		}

		public object Get<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, Paging paging, string scope = null, params Sorting[] sorting)
		{
			return this.Get<TKey1, TKey2, TKey3>(key1, key2, key3, scope, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition = null, params Sorting[] sorting)
		{
			return this.Select(condition, null, string.Empty, null, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, params Sorting[] sorting)
		{
			return this.Select(condition, null, scope, null, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, string scope, Paging paging, params Sorting[] sorting)
		{
			return this.Select(condition, null, scope, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, params Sorting[] sorting)
		{
			return this.Select(condition, null, null, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Paging paging, string scope, params Sorting[] sorting)
		{
			return this.Select(condition, null, scope, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, params Sorting[] sorting)
		{
			return this.Select(condition, grouping, string.Empty, null, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, params Sorting[] sorting)
		{
			return this.Select(condition, grouping, scope, null, sorting);
		}

		public virtual IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, string scope, Paging paging, params Sorting[] sorting)
		{
			return this.EnsureDataAccess().Select<TEntity>(this.Name, condition, grouping, scope, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, params Sorting[] sorting)
		{
			return this.Select(condition, grouping, null, paging, sorting);
		}

		public IEnumerable<TEntity> Select(ICondition condition, Grouping grouping, Paging paging, string scope, params Sorting[] sorting)
		{
			return this.Select(condition, grouping, scope, paging, sorting);
		}
		#endregion

		#region 删除方法
		public virtual int Delete<TKey>(TKey key, string cascades = null)
		{
			return this.EnsureDataAccess().Delete(this.Name, this.ConvertKey(key), cascades);
		}

		public virtual int Delete<TKey1, TKey2>(TKey1 key1, TKey2 key2, string cascades = null)
		{
			return this.EnsureDataAccess().Delete(this.Name, this.ConvertKey(key1, key2), cascades);
		}

		public virtual int Delete<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3, string cascades = null)
		{
			return this.EnsureDataAccess().Delete(this.Name, this.ConvertKey(key1, key2, key3), cascades);
		}

		public int Delete(ICondition condition)
		{
			return this.Delete(condition, string.Empty);
		}

		public virtual int Delete(ICondition condition, string cascades)
		{
			return this.EnsureDataAccess().Delete(this.Name, condition, cascades);
		}

		public int Delete(ICondition condition, Expression<Func<TEntity, object>> cascades)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 插入方法
		public virtual int Insert(TEntity entity, string scope = null)
		{
			return this.EnsureDataAccess().Insert(this.Name, entity, scope);
		}

		public int Insert(TEntity entity, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes)
		{
			throw new NotImplementedException();
		}

		public virtual int Insert(IEnumerable<TEntity> entities, string scope = null)
		{
			return this.EnsureDataAccess().Insert(this.Name, entities, scope);
		}

		public int Insert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region 修改方法
		public virtual int Update<TKey>(TEntity entity, TKey key, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entity, this.ConvertKey(key), scope);
		}

		public virtual int Update<TKey1, TKey2>(TEntity entity, TKey1 key1, TKey2 key2, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entity, this.ConvertKey(key1, key2), scope);
		}

		public virtual int Update<TKey1, TKey2, TKey3>(TEntity entity, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entity, this.ConvertKey(key1, key2, key3), scope);
		}

		public virtual int Update<TKey>(IEnumerable<TEntity> entities, TKey key, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entities, this.ConvertKey(key), scope);
		}

		public virtual int Update<TKey1, TKey2>(IEnumerable<TEntity> entities, TKey1 key1, TKey2 key2, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entities, this.ConvertKey(key1, key2), scope);
		}

		public virtual int Update<TKey1, TKey2, TKey3>(IEnumerable<TEntity> entities, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entities, this.ConvertKey(key1, key2, key3), scope);
		}

		public virtual int Update(object entity, ICondition condition = null, string scope = null)
		{
			return this.EnsureDataAccess().Update<object>(this.Name, entity, condition, scope);
		}

		public virtual int Update<TKey>(object entity, TKey key, string scope = null)
		{
			return this.EnsureDataAccess().Update<object>(this.Name, entity, this.ConvertKey(key), scope);
		}

		public virtual int Update<TKey1, TKey2>(object entity, TKey1 key1, TKey2 key2, string scope = null)
		{
			return this.EnsureDataAccess().Update<object>(this.Name, entity, this.ConvertKey(key1, key2), scope);
		}

		public virtual int Update<TKey1, TKey2, TKey3>(object entity, TKey1 key1, TKey2 key2, TKey3 key3, string scope = null)
		{
			return this.EnsureDataAccess().Update<object>(this.Name, entity, this.ConvertKey(key1, key2, key3), scope);
		}

		public virtual int Update(TEntity entity, ICondition condition = null, string scope = null)
		{
			return this.Update(new TEntity[] { entity }, condition, scope);
		}

		public int Update(TEntity entity, ICondition condition, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null)
		{
			throw new NotImplementedException();
		}

		public virtual int Update(IEnumerable<TEntity> entities, ICondition condition = null, string scope = null)
		{
			return this.EnsureDataAccess().Update(this.Name, entities, condition, scope);
		}

		public int Update(IEnumerable<TEntity> entities, ICondition condition, Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, object>> excludes = null)
		{
			throw new NotImplementedException();
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
				throw new InvalidOperationException("No matched key(s) about '" + typeof(TEntity).FullName + "' type.");

			return Condition.Equal(properties[0].Name, key1) & Condition.Equal(properties[1].Name, key2);
		}

		protected virtual ICondition ConvertKey<TKey1, TKey2, TKey3>(TKey1 key1, TKey2 key2, TKey3 key3)
		{
			var properties = this.GetEntityKeys();

			if(properties.Length < 3)
				throw new InvalidOperationException("No matched key(s) about '" + typeof(TEntity).FullName + "' type.");

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
