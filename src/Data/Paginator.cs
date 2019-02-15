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
	public static class Paginator
	{
		#region 公共方法
		public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, System.Linq.Expressions.Expression<Func<TSource, TResult>> map)
		{
			return (IEnumerable<TResult>)Map(source, Common.ExpressionUtility.GetMemberName(map));
		}

		public static IEnumerable Map<TSource>(this IEnumerable<TSource> source, string path)
		{
			if(source == null)
				throw new ArgumentNullException(nameof(source));

			if(string.IsNullOrEmpty(path))
				return source;

			var paginator = source as IPaginator;

			if(paginator == null)
				throw new ArgumentException($"The specified data source does not implement the '{nameof(IPaginator)}' interface.");

			object Map(TSource entity)
			{
				if(entity == null)
					return null;

				return Zongsoft.Reflection.Reflector.GetValue(entity, path);
			}

			var elementType = typeof(TSource).GetProperty(path)?.PropertyType ?? typeof(TSource).GetField(path).FieldType;
			var collectionType = typeof(MappedCollection<,>).MakeGenericType(typeof(TSource), elementType);
			return (IEnumerable)Activator.CreateInstance(collectionType, new object[] { source, new Func<TSource, object>(Map) });
		}
		#endregion

		#region 嵌套子类
		private class MappedCollection<TSource, TResult> : IEnumerable<TResult>, IEnumerable, IPaginator
		{
			#region 事件声明
			public event EventHandler<PagingEventArgs> Paginated;
			#endregion

			#region 私有变量
			private IEnumerable<TSource> _source;
			private Func<TSource, object> _map;
			#endregion

			#region 构造函数
			public MappedCollection(IEnumerable<TSource> source, Func<TSource, object> map)
			{
				_map = map;
				_source = source;
				((IPaginator)_source).Paginated += this.Paginator_Paginated;
			}
			#endregion

			#region 枚举遍历
			public IEnumerator<TResult> GetEnumerator()
			{
				return new MappedIterator(_source.GetEnumerator(), _map, this.OnExit);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion

			#region 私有方法
			private void Paginator_Paginated(object sender, PagingEventArgs e)
			{
				this.Paginated?.Invoke(sender, e);
			}

			private void OnExit()
			{
				((IPaginator)_source).Paginated -= this.Paginator_Paginated;
			}
			#endregion

			#region 数据迭代
			private class MappedIterator : IEnumerator<TResult>, IEnumerator, IDisposable
			{
				private IEnumerator<TSource> _iterator;
				private Func<TSource, object> _map;
				private Action _exit;

				public MappedIterator(IEnumerator<TSource> iterator, Func<TSource, object> map, Action exit)
				{
					_iterator = iterator;
					_map = map;
					_exit = exit;
				}

				public TResult Current
				{
					get => (TResult)_map(_iterator.Current);
				}

				public bool MoveNext()
				{
					return _iterator.MoveNext();
				}

				public void Reset()
				{
					_iterator.Reset();
				}

				public void Dispose()
				{
					_iterator.Dispose();
					_exit();
				}

				object IEnumerator.Current
				{
					get => _map(_iterator.Current);
				}
			}
			#endregion
		}
		#endregion
	}
}
