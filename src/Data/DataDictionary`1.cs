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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Zongsoft.Data
{
	public class DataDictionary<T> : DataDictionary
	{
		#region 构造函数
		private DataDictionary(object data) : base(data)
		{
		}
		#endregion

		#region 公共方法
		public bool Contains<TMember>(Expression<Func<T, TMember>> member)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			return this.Contains(Common.ExpressionUtility.GetMemberName(member));
		}

		public TMember Get<TMember>(Expression<Func<T, TMember>> member)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			var token = Common.ExpressionUtility.GetMember(member);
			return (TMember)Zongsoft.Common.Convert.ConvertValue(this.Get(token.Name), token.MemberType);
		}

		public TMember Get<TMember>(Expression<Func<T, TMember>> member, TMember defaultValue)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			object result;
			var name = Common.ExpressionUtility.GetMemberName(member);

			if(this.TryGet(name, out result))
			{
				if(Zongsoft.Common.Convert.TryConvertValue(result, typeof(TMember), out result))
					return (TMember)result;
			}

			return defaultValue;
		}

		public bool TryGet<TMember>(Expression<Func<T, TMember>> member, Action<string, TMember> onGot)
		{
			if(member == null)
				throw new ArgumentNullException("member");

			if(onGot == null)
				throw new ArgumentNullException("onGot");

			var name = Common.ExpressionUtility.GetMemberName(member);
			return this.TryGet(name, value => onGot(name, (TMember)this.ConvertValue(value, typeof(TMember))));
		}

		public bool TryGet<TMember>(Expression<Func<T, TMember>> member, out TMember result)
		{
			TMember memberValue = default(TMember);

			if(this.TryGet(member, (key, value) => memberValue = value))
			{
				result = memberValue;
				return true;
			}

			result = default(TMember);
			return false;
		}

		public void Set<TMember>(Expression<Func<T, TMember>> member, TMember value, Func<TMember, bool> predicate = null)
		{
			this.Set<TMember>(member, name => value, predicate);
		}

		public void Set<TMember>(Expression<Func<T, TMember>> member, Func<string, TMember> valueThunk, Func<TMember, bool> predicate = null)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			if(valueThunk == null)
				throw new ArgumentNullException("valueThunk");

			var requiredThorws = true;
			var predicateProxy = predicate;

			//如果条件断言不为空，则必须进行是否抛出异常的条件处理
			if(predicate != null)
			{
				//如果是因为设置条件没有通过，则不能抛出异常，因为这是设置方法的正常逻辑
				predicateProxy = p => requiredThorws = predicate(p);
			}

			if(!this.TrySet(member, valueThunk, predicateProxy) && requiredThorws)
			{
				//获取指定的成员信息
				var token = Common.ExpressionUtility.GetMember(member);

				throw new KeyNotFoundException(string.Format("The '{0}' property is not existed.", token.Name));
			}
		}

		public bool TrySet<TMember>(Expression<Func<T, TMember>> member, TMember value, Func<TMember, bool> predicate = null)
		{
			return this.TrySet<TMember>(member, (string _) => value, predicate);
		}

		public bool TrySet<TMember>(Expression<Func<T, TMember>> member, Func<string, TMember> valueThunk, Func<TMember, bool> predicate = null)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			if(valueThunk == null)
				throw new ArgumentNullException("valueThunk");

			//获取指定的成员信息
			var token = Common.ExpressionUtility.GetMember(member);

			if(predicate == null)
				return this.TrySet(token.Name, () => valueThunk(token.Name), null);
			else
				return this.TrySet(token.Name, () => valueThunk(token.Name), original => predicate((TMember)Zongsoft.Common.Convert.ConvertValue(original, token.MemberType)));
		}
		#endregion

		#region 静态方法
		public static DataDictionary<T> GetDataDictionary(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			if(data is DataDictionary<T>)
				return (DataDictionary<T>)data;

			if(data is DataDictionary)
				data = ((DataDictionary)data).Data;

			return new DataDictionary<T>(data);
		}

		public static IEnumerable<DataDictionary<T>> GetDataDictionaries(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			var items = data as IEnumerable;

			if(items == null)
				yield return GetDataDictionary(data);
			else
			{
				foreach(var item in items)
				{
					if(item != null)
						yield return GetDataDictionary(item);
				}
			}
		}
		#endregion

		#region 私有方法
		private object ConvertValue(object value, Type conversionType)
		{
			object result;

			//如果类型转换成功，则直接返回
			if(Common.Convert.TryConvertValue(value, conversionType, out result))
				return result;

			if(value == null)
				return Common.TypeExtension.GetDefaultValue(conversionType);

			if(conversionType.IsInterface)
			{
				if(Common.TypeExtension.IsEnumerable(conversionType))
				{
					if(conversionType.IsGenericType)
					{
						conversionType = conversionType.GetGenericArguments()[0];
						result = Activator.CreateInstance(typeof(List<>).MakeGenericType(conversionType));
					}
					else
					{
						conversionType = typeof(object);
						result = new List<object>();
					}
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			if(result is IList)
			{
				this.MapMany(value, (IList)result, conversionType);
				return result;
			}
			else
			{
				return this.MapSingle(value, conversionType);
			}
		}

		private void MapMany(object source, IList destination, Type conversionType)
		{
			var items = source as IEnumerable;

			if(items == null)
				destination.Add(MapSingle(source, conversionType));
			else
			{
				foreach(var item in items)
					destination.Add(MapSingle(item, conversionType));
			}
		}

		private object MapSingle(object value, Type conversionType)
		{
			object result;

			//如果类型转换成功，则直接返回
			if(Common.Convert.TryConvertValue(value, conversionType, out result))
				return result;

			if(value == null)
				return Common.TypeExtension.GetDefaultValue(conversionType);

			//创建目标类型的实例
			result = Activator.CreateInstance(conversionType);

			if(Common.TypeExtension.IsDictionary(conversionType))
			{
				if(Common.TypeExtension.IsDictionary(value.GetType()))
				{
					foreach(var entry in (IEnumerable)value)
						;
				}
			}
			else
			{
			}

			//获取源对象的属性集合
			var sources = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach(var source in sources)
			{
				var destination = conversionType.GetProperty(source.Name);

				if(destination != null)
					destination.SetValue(result, Common.Convert.ConvertValue(source.GetValue(value), destination.PropertyType));
			}

			return result;
		}
		#endregion
	}
}
