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
 * Copyright (C) 2016-2019 Zongsoft Corporation <http://www.zongsoft.com>
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
	public class DataSearcher<TEntity> : IDataSearcher<TEntity>, IDataSearcher
	{
		#region 成员字段
		private IDataSearcherConditioner _conditioner;
		#endregion

		#region 构造函数
		public DataSearcher(IDataService<TEntity> dataService, DataSearcherAttribute[] attributes)
		{
			this.DataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

			if(attributes != null && attributes.Length > 0)
				this.Conditioner = DataSearcherResolver.Create(attributes);
		}
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return this.DataService.Name;
			}
		}

		public IDataService<TEntity> DataService
		{
			get;
		}

		public IDataSearcherConditioner Conditioner
		{
			get => _conditioner;
			set => _conditioner = value ?? throw new ArgumentNullException();
		}
		#endregion

		#region 计数方法
		public int Count(string keyword, IDictionary<string, object> states = null)
		{
			return this.DataService.Count(
				this.Resolve(nameof(Count), keyword, states),
				string.Empty,
				states);
		}
		#endregion

		#region 存在方法
		public bool Exists(string keyword, IDictionary<string, object> states = null)
		{
			return this.DataService.Exists(
				this.Resolve(nameof(Exists), keyword, states),
				states);
		}
		#endregion

		#region 搜索方法
		public IEnumerable<TEntity> Search(string keyword, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, null, null, sortings);
		}

		public IEnumerable<TEntity> Search(string keyword, IDictionary<string, object> states, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, null, states, sortings);
		}

		public IEnumerable<TEntity> Search(string keyword, Paging paging, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, paging, null, sortings);
		}

		public IEnumerable<TEntity> Search(string keyword, string schema, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, null, null, sortings);
		}

		public IEnumerable<TEntity> Search(string keyword, string schema, IDictionary<string, object> states, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, null, states, sortings);
		}

		public IEnumerable<TEntity> Search(string keyword, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, paging, null, sortings);
		}

		public IEnumerable<TEntity> Search(string keyword, string schema, Paging paging, IDictionary<string, object> states, params Sorting[] sortings)
		{
			return this.DataService.Select(
				this.Resolve(nameof(Search), keyword, states),
				schema,
				paging,
				states,
				sortings);
		}
		#endregion

		#region 显式实现
		IEnumerable IDataSearcher.Search(string keyword, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, null, null, sortings);
		}

		IEnumerable IDataSearcher.Search(string keyword, IDictionary<string, object> states, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, null, states, sortings);
		}

		IEnumerable IDataSearcher.Search(string keyword, Paging paging, params Sorting[] sortings)
		{
			return this.Search(keyword, string.Empty, paging, null, sortings);
		}

		IEnumerable IDataSearcher.Search(string keyword, string schema, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, null, null, sortings);
		}

		IEnumerable IDataSearcher.Search(string keyword, string schema, IDictionary<string, object> states, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, null, states, sortings);
		}

		IEnumerable IDataSearcher.Search(string keyword, string schema, Paging paging, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, paging, null, sortings);
		}

		IEnumerable IDataSearcher.Search(string keyword, string schema, Paging paging, IDictionary<string, object> states, params Sorting[] sortings)
		{
			return this.Search(keyword, schema, paging, states, sortings);
		}
		#endregion

		#region 条件解析
		protected virtual ICondition Resolve(string method, string keyword, IDictionary<string, object> states = null)
		{
			var conditioner = this.Conditioner;

			if(conditioner == null)
				throw new InvalidOperationException("Missing the required keyword condition resolver.");

			return conditioner.Resolve(method, keyword, states);
		}
		#endregion

		#region 嵌套子类
		private class DataSearcherResolver : IDataSearcherConditioner
		{
			#region 私有变量
			private readonly IDictionary<string, ConditionToken> _mapping;
			#endregion

			#region 构造函数
			private DataSearcherResolver(IDictionary<string, ConditionToken> mapping)
			{
				_mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
			}
			#endregion

			#region 解析方法
			public ICondition Resolve(string method, string keyword, IDictionary<string, object> states)
			{
				if(string.IsNullOrWhiteSpace(keyword))
					return null;

				var index = keyword.IndexOf(':');

				if(!_mapping.TryGetValue(index < 0 ? string.Empty : keyword.Substring(0, index).Trim(), out var token))
					return null;

				var conditions = new ConditionCollection(token.Combination);

				foreach(var field in token.Fields)
				{
					if(field.IsExactly)
						conditions.Add(Condition.Equal(field.Name, keyword.Substring(index + 1).Trim()));
					else
						conditions.Add(Condition.Like(field.Name, "%" + keyword.Substring(index + 1).Trim() + "%"));
				}

				if(conditions.Count == 1)
					return conditions[0];
				else
					return conditions;
			}
			#endregion

			#region 静态方法
			public static DataSearcherResolver Create(DataSearcherAttribute[] attributes)
			{
				if(attributes == null || attributes.Length == 0)
					return null;

				var mapping = new Dictionary<string, ConditionToken>(StringComparer.OrdinalIgnoreCase);

				foreach(var attribute in attributes)
				{
					foreach(var pattern in attribute.Patterns)
					{
						var index = pattern.IndexOf(':');

						if(index > 0)
						{
							var keys = pattern.Substring(0, index).Split(',');
							var token = ConditionToken.Create(pattern.Substring(index + 1).Split(','));

							if(keys.Length == 1)
							{
								mapping[keys[0].Trim()] = token;
							}
							else
							{
								foreach(var key in keys)
								{
									mapping[key.Trim()] = token;
								}
							}
						}
						else
						{
							var token = ConditionToken.Create(pattern.Split(','));

							if(token.Fields.Length == 1)
								mapping[token.Fields[0].Name] = token;
							else
							{
								mapping[string.Empty] = token;

								foreach(var field in token.Fields)
								{
									mapping[field.Name] = token;
								}
							}
						}
					}
				}

				return new DataSearcherResolver(mapping);
			}
			#endregion

			#region 内部结构
			private struct ConditionToken
			{
				#region 公共字段
				/// <summary>条件组合方式</summary>
				public ConditionCombination Combination;

				/// <summary>条件字段数组</summary>
				public ConditionFieldToken[] Fields;
				#endregion

				#region 构造函数
				public ConditionToken(ConditionFieldToken[] fields)
				{
					this.Fields = fields;
					this.Combination = ConditionCombination.Or;
				}
				#endregion

				#region 静态方法
				public static ConditionToken Create(string[] fields)
				{
					if(fields == null || fields.Length == 0)
						throw new ArgumentNullException(nameof(fields));

					var tokens = new List<ConditionFieldToken>(fields.Length);

					foreach(var field in fields)
					{
						if(!string.IsNullOrWhiteSpace(field))
							tokens.Add(new ConditionFieldToken(field));
					}

					if(tokens.Count == 0)
						throw new InvalidOperationException("Missing specified search field definitions.");

					return new ConditionToken(tokens.ToArray());
				}
				#endregion

				#region 重写方法
				public override string ToString()
				{
					var text = new System.Text.StringBuilder();

					for(int i = 0; i < this.Fields.Length; i++)
					{
						if(i > 0)
							text.Append(" " + this.Combination.ToString() + " ");

						text.Append(this.Fields[i].ToString());
					}

					return text.ToString();
				}
				#endregion
			}

			private struct ConditionFieldToken
			{
				#region 公共字段
				/// <summary>字段名称</summary>
				public string Name;

				/// <summary>是否精确匹配</summary>
				public bool IsExactly;
				#endregion

				#region 构造函数
				public ConditionFieldToken(string field)
				{
					if(string.IsNullOrWhiteSpace(field))
						throw new ArgumentNullException(nameof(field));

					field = field.Trim();
					this.IsExactly = field[0] == '!' || field[field.Length - 1] == '!' || (field[0] != '?' && field[field.Length - 1] != '?');

					if(this.IsExactly)
						this.Name = field.Trim('!');
					else
						this.Name = field.Trim('?');
				}
				#endregion

				#region 重写方法
				public override string ToString()
				{
					if(this.IsExactly)
						return this.Name;
					else
						return this.Name + "?";
				}
				#endregion
			}
			#endregion
		}
		#endregion
	}
}
