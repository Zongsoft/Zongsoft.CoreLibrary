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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public class DataAccessFilterCollection : ICollection<IDataAccessFilter>
	{
		#region 私有变量
		private readonly ISet<IDataAccessFilter> _items;
		private readonly IDictionary<DataAccessMethod, ICollection<IDataAccessFilter>> _globals;
		private readonly IDictionary<string, FilterToken> _tokens;
		#endregion

		#region 构造函数
		public DataAccessFilterCollection()
		{
			_items = new HashSet<IDataAccessFilter>();
			_globals = new Dictionary<DataAccessMethod, ICollection<IDataAccessFilter>>();
			_tokens = new Dictionary<string, FilterToken>(StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region 公共属性
		public int Count
		{
			get => _items.Count;
		}

		public bool IsReadOnly
		{
			get => false;
		}
		#endregion

		#region 公共方法
		public void InvokeFiltering(IDataAccessContextBase context)
		{
			if(_globals != null && _globals.Count > 0)
			{
				if(_globals.TryGetValue(context.Method, out var filters) && filters != null && filters.Count > 0)
				{
					foreach(var filter in filters)
						filter.OnFiltering(context);
				}
			}

			if(_tokens != null && _tokens.Count > 0)
			{
				if(_tokens.TryGetValue(context.Name, out var token) && token.TryGetFilters(context.Method, out var filters))
				{
					foreach(var filter in filters)
						filter.OnFiltering(context);
				}
			}
		}

		public void InvokeFiltered(IDataAccessContextBase context)
		{
			if(_tokens != null && _tokens.Count > 0)
			{
				if(_tokens.TryGetValue(context.Name, out var token) && token.TryGetFilters(context.Method, out var filters))
				{
					foreach(var filter in filters)
						filter.OnFiltered(context);
				}
			}

			if(_globals != null && _globals.Count > 0)
			{
				if(_globals.TryGetValue(context.Method, out var filters) && filters != null && filters.Count > 0)
				{
					foreach(var filter in filters)
						filter.OnFiltered(context);
				}
			}
		}

		public void Add(IDataAccessFilter item)
		{
			if(item == null)
				throw new ArgumentNullException(nameof(item));

			if(!_items.Add(item))
				return;

			if(string.IsNullOrWhiteSpace(item.Name))
			{
				foreach(var method in item.Methods)
				{
					if(_globals.TryGetValue(method, out var filters))
						filters.Add(item);
					else
						_globals.Add(method, new List<IDataAccessFilter>(new[] { item }));
				}
			}
			else
			{
				if(_tokens.TryGetValue(item.Name, out var token))
					token.Add(item);
				else
					_tokens.Add(item.Name, new FilterToken(item));
			}
		}

		public void Clear()
		{
			_items.Clear();
			_globals.Clear();
			_tokens.Clear();
		}

		public bool Remove(IDataAccessFilter item)
		{
			if(item == null)
				return false;

			if(!_items.Remove(item))
				return false;

			if(string.IsNullOrWhiteSpace(item.Name))
			{
				if(item.Methods == null || item.Methods.Length == 0)
				{
					foreach(var global in _globals.Values)
					{
						global.Remove(item);
					}
				}
				else
				{
					for(int i = 0; i < item.Methods.Length; i++)
					{
						if(_globals.TryGetValue(item.Methods[i], out var global))
						{
							global.Remove(item);
						}
					}
				}
			}
			else
			{
				if(_tokens.TryGetValue(item.Name, out var token))
				{
					token.Remove(item);
					return true;
				}
			}

			return false;
		}

		public bool Contains(IDataAccessFilter item)
		{
			if(item == null)
				return false;

			return _items.Contains(item);
		}

		void ICollection<IDataAccessFilter>.CopyTo(IDataAccessFilter[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException(nameof(array));

			_items.CopyTo(array, arrayIndex);
		}
		#endregion

		#region 遍历枚举
		public IEnumerator<IDataAccessFilter> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion

		#region 嵌套子类
		private class FilterToken
		{
			#region 成员字段
			private ICollection<IDataAccessFilter> _counts;
			private ICollection<IDataAccessFilter> _exists;
			private ICollection<IDataAccessFilter> _selects;
			private ICollection<IDataAccessFilter> _deletes;
			private ICollection<IDataAccessFilter> _inserts;
			private ICollection<IDataAccessFilter> _updates;
			private ICollection<IDataAccessFilter> _upserts;
			private ICollection<IDataAccessFilter> _executes;
			private ICollection<IDataAccessFilter> _increments;
			#endregion

			#region 构造函数
			public FilterToken(IDataAccessFilter filter)
			{
				this.Add(filter);
			}
			#endregion

			#region 私有属性
			private ICollection<IDataAccessFilter> Counts
			{
				get => this.EnsureFilters(ref _counts);
			}

			private ICollection<IDataAccessFilter> Exists
			{
				get => this.EnsureFilters(ref _exists);
			}

			private ICollection<IDataAccessFilter> Selects
			{
				get => this.EnsureFilters(ref _selects);
			}

			private ICollection<IDataAccessFilter> Deletes
			{
				get => this.EnsureFilters(ref _deletes);
			}

			private ICollection<IDataAccessFilter> Inserts
			{
				get => this.EnsureFilters(ref _inserts);
			}

			private ICollection<IDataAccessFilter> Updates
			{
				get => this.EnsureFilters(ref _updates);
			}

			private ICollection<IDataAccessFilter> Upserts
			{
				get => this.EnsureFilters(ref _upserts);
			}

			private ICollection<IDataAccessFilter> Executes
			{
				get => this.EnsureFilters(ref _executes);
			}

			private ICollection<IDataAccessFilter> Increments
			{
				get => this.EnsureFilters(ref _increments);
			}
			#endregion

			#region 公共方法
			public void Add(IDataAccessFilter filter)
			{
				if(filter.Methods == null || filter.Methods.Length == 0)
				{
					this.Counts.Add(filter);
					this.Exists.Add(filter);
					this.Selects.Add(filter);
					this.Deletes.Add(filter);
					this.Inserts.Add(filter);
					this.Updates.Add(filter);
					this.Upserts.Add(filter);
					this.Executes.Add(filter);
					this.Increments.Add(filter);
				}
				else
				{
					for(int i = 0; i < filter.Methods.Length; i++)
					{
						switch(filter.Methods[i])
						{
							case DataAccessMethod.Count:
								this.Counts.Add(filter);
								break;
							case DataAccessMethod.Exists:
								this.Exists.Add(filter);
								break;
							case DataAccessMethod.Select:
								this.Selects.Add(filter);
								break;
							case DataAccessMethod.Delete:
								this.Deletes.Add(filter);
								break;
							case DataAccessMethod.Insert:
								this.Inserts.Add(filter);
								break;
							case DataAccessMethod.Update:
								this.Updates.Add(filter);
								break;
							case DataAccessMethod.Upsert:
								this.Upserts.Add(filter);
								break;
							case DataAccessMethod.Execute:
								this.Executes.Add(filter);
								break;
							case DataAccessMethod.Increment:
								this.Increments.Add(filter);
								break;
						}
					}
				}
			}

			public void Remove(IDataAccessFilter filter)
			{
				if(filter.Methods == null || filter.Methods.Length == 0)
				{
					_counts?.Remove(filter);
					_exists?.Remove(filter);
					_selects?.Remove(filter);
					_deletes?.Remove(filter);
					_inserts?.Remove(filter);
					_updates?.Remove(filter);
					_upserts?.Remove(filter);
					_executes?.Remove(filter);
					_increments?.Remove(filter);
				}
				else
				{
					for(int i = 0; i < filter.Methods.Length; i++)
					{
						switch(filter.Methods[i])
						{
							case DataAccessMethod.Count:
								_counts?.Remove(filter);
								break;
							case DataAccessMethod.Exists:
								_exists?.Remove(filter);
								break;
							case DataAccessMethod.Select:
								_selects?.Remove(filter);
								break;
							case DataAccessMethod.Delete:
								_deletes?.Remove(filter);
								break;
							case DataAccessMethod.Insert:
								_inserts?.Remove(filter);
								break;
							case DataAccessMethod.Update:
								_updates?.Remove(filter);
								break;
							case DataAccessMethod.Upsert:
								_upserts?.Remove(filter);
								break;
							case DataAccessMethod.Execute:
								_executes?.Remove(filter);
								break;
							case DataAccessMethod.Increment:
								_increments?.Remove(filter);
								break;
						}
					}
				}
			}

			public bool TryGetFilters(DataAccessMethod method, out ICollection<IDataAccessFilter> filters)
			{
				filters = null;

				switch(method)
				{
					case DataAccessMethod.Count:
						filters = _counts;
						break;
					case DataAccessMethod.Exists:
						filters = _exists;
						break;
					case DataAccessMethod.Select:
						filters = _selects;
						break;
					case DataAccessMethod.Delete:
						filters = _deletes;
						break;
					case DataAccessMethod.Insert:
						filters = _inserts;
						break;
					case DataAccessMethod.Update:
						filters = _updates;
						break;
					case DataAccessMethod.Upsert:
						filters = _upserts;
						break;
					case DataAccessMethod.Execute:
						filters = _executes;
						break;
					case DataAccessMethod.Increment:
						filters = _increments;
						break;
				}

				return filters != null && filters.Count > 0;
			}
			#endregion

			#region 私有方法
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			private ICollection<IDataAccessFilter> EnsureFilters(ref ICollection<IDataAccessFilter> filters)
			{
				if(filters == null)
				{
					lock(this)
					{
						if(filters == null)
							filters = new List<IDataAccessFilter>();
					}
				}

				return filters;
			}
			#endregion
		}
		#endregion
	}
}
