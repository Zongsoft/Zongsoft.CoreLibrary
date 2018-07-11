/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2018 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Collections
{
	public static class Enumerable
	{
		#region 公共方法
		public static IEnumerable Empty(Type elementType)
		{
			if(elementType == null)
				throw new ArgumentNullException(nameof(elementType));

			return (IEnumerable)System.Activator.CreateInstance(typeof(EmptyEnumerable<>).MakeGenericType(elementType));
		}

		public static IEnumerable<T> Enumerate<T>(object source)
		{
			if(source == null)
				return System.Linq.Enumerable.Empty<T>();

			if(source is IEnumerable<T> items)
				return items;

			return (IEnumerable<T>)System.Activator.CreateInstance(typeof(TypedEnumerable<>).MakeGenericType(typeof(T)), new object[] { source });
		}

		public static IEnumerable Enumerate(object source, Type elementType)
		{
			if(elementType == null)
				throw new ArgumentNullException(nameof(elementType));

			if(source == null)
				return (IEnumerable)System.Activator.CreateInstance(typeof(EmptyEnumerable<>).MakeGenericType(elementType));
			else
				return (IEnumerable)System.Activator.CreateInstance(typeof(TypedEnumerable<>).MakeGenericType(elementType), new object[] { source });
		}
		#endregion

		#region 嵌套子类
		private class EmptyEnumerable<T> : IEnumerable<T>
		{
			public IEnumerator<T> GetEnumerator()
			{
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				yield break;
			}
		}

		private class TypedEnumerable<T> : IEnumerable<T>
		{
			#region 私有变量
			private Func<IEnumerator<T>> _iterator;
			#endregion

			#region 构造函数
			public TypedEnumerable(object source)
			{
				var items = source as IEnumerable;

				if(items != null && (source.GetType() != typeof(string) || typeof(T) == typeof(char)))
					_iterator = () => new MultitapEnumerator(items.GetEnumerator());
				else
				{
					if(Zongsoft.Common.Convert.TryConvertValue<T>(source, out var element))
						_iterator = () => new SimulateEnumerator(element);
					else
						throw new InvalidOperationException($"The '{source.GetType()}' type cannot be convert to '{typeof(T)}' type.");
				}
			}
			#endregion

			#region 迭代遍历
			public IEnumerator<T> GetEnumerator()
			{
				return _iterator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _iterator();
			}
			#endregion

			#region 迭代实现
			private class SimulateEnumerator : IEnumerator<T>
			{
				private int _flag;
				private T _element;

				public SimulateEnumerator(T element)
				{
					_element = element;
				}

				public T Current
				{
					get
					{
						if(_flag == 1)
							return _element;

						if(_flag == 0)
							throw new InvalidOperationException("The iterator has not yet started, please call the MoveNext() method first.");
						else
							throw new InvalidOperationException("The iterator has terminated.");
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				public bool MoveNext()
				{
					var original = System.Threading.Interlocked.CompareExchange(ref _flag, 1, 0);

					if(original == 0)
						return true;

					System.Threading.Interlocked.CompareExchange(ref _flag, 2, 1);
					return false;
				}

				public void Reset()
				{
					_flag = 0;
				}

				public void Dispose()
				{
					if(_element is IDisposable disposable)
						disposable.Dispose();
				}
			}

			private class MultitapEnumerator : IEnumerator<T>
			{
				private IEnumerator _enumerator;

				public MultitapEnumerator(IEnumerator enumerator)
				{
					_enumerator = enumerator;
				}

				public T Current
				{
					get
					{
						return (T)_enumerator.Current;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return _enumerator.Current;
					}
				}

				public bool MoveNext()
				{
					return _enumerator.MoveNext();
				}

				public void Reset()
				{
					_enumerator.Reset();
				}

				public void Dispose()
				{
					if(_enumerator is IDisposable disposable)
						disposable.Dispose();
				}
			}
			#endregion
		}
		#endregion
	}
}
