/*
 * Authors:
 *   陈德宝(Debao Chen) <chendebao1985@163.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Zongsoft.ComponentModel
{
	public class NotifyObject : System.ComponentModel.INotifyPropertyChanged
	{
		#region 事件声明
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region 成员字段
		private ConcurrentDictionary<string, object> _properties;
		#endregion

		#region 保护属性
		protected bool HasProperties
		{
			get
			{
				return _properties != null && _properties.Count > 0;
			}
		}

		protected IDictionary<string, object> Properties
		{
			get
			{
				if(_properties == null)
					System.Threading.Interlocked.CompareExchange(ref _properties, new ConcurrentDictionary<string, object>(), null);

				return _properties;
			}
		}
		#endregion

		#region 保护方法
		[Obsolete("Please use the GetPropertyValue<T>(...) method.")]
		protected T GetValue<T>(string propertyName, T defaultValue = default(T))
		{
			return this.GetPropertyValue<T>(propertyName, defaultValue);
		}

		[Obsolete("Please use the GetPropertyValue<T>(...) method.")]
		protected T GetValue<T>(Expression<Func<T>> propertyExpression, T defaultValue = default(T))
		{
			return this.GetPropertyValue<T>(propertyExpression, defaultValue);
		}

		[Obsolete("Please use the SetPropertyValue<T>(...) method.")]
		protected void SetValue(object value, string propertyName)
		{
			this.SetPropertyValue(propertyName, value);
		}

		[Obsolete("Please use the SetPropertyValue<T>(...) method.")]
		protected void SetValue<T>(T value, Expression<Func<T>> propertyExpression)
		{
			this.SetPropertyValue<T>(propertyExpression, value);
		}

		[Obsolete("Please use the SetPropertyValue<T>(...) method.")]
		protected void SetValue<T>(ref T target, T value, string propertyName)
		{
			this.SetPropertyValue<T>(propertyName, ref target, value);
		}

		[Obsolete("Please use the SetPropertyValue<T>(...) method.")]
		protected void SetValue<T>(ref T target, T value, Expression<Func<T>> propertyExpression)
		{
			this.SetPropertyValue<T>(propertyExpression, ref target, value);
		}

		protected T GetPropertyValue<T>(string propertyName, T defaultValue = default(T))
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName");

			var properties = this.Properties;
			object value;

			if(properties.TryGetValue(propertyName.Trim(), out value))
				return (T)value;

			var property = this.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if(property == null)
				throw new InvalidOperationException(string.Format("The '{0}' property is not exists.", propertyName));

			//返回属性的默认值
			return this.GetPropertyDefaultValue(property, defaultValue);
		}

		protected T GetPropertyValue<T>(Expression<Func<T>> propertyExpression, T defaultValue = default(T))
		{
			if(propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");

			var property = this.GetPropertyInfo<T>(propertyExpression);

			if(property == null)
				throw new ArgumentException("Invalid expression of the argument", "propertyExpression");

			var properties = this.Properties;
			object value;

			if(properties.TryGetValue(property.Name, out value))
				return (T)value;

			//返回属性的默认值
			return this.GetPropertyDefaultValue(property, defaultValue);
		}

		protected void SetPropertyValue(string propertyName, object value)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException("propertyName");

			var properties = this.Properties;

			properties[propertyName.Trim()] = value;

			this.OnPropertyChanged(propertyName);
		}

		protected void SetPropertyValue<T>(Expression<Func<T>> propertyExpression, T value)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");

			var property = this.GetPropertyInfo<T>(propertyExpression);

			if(property == null)
				throw new ArgumentException("Invalid expression of the argument", "propertyExpression");

			var properties = this.Properties;

			properties[property.Name] = value;

			this.OnPropertyChanged(property.Name);
		}

		protected void SetPropertyValue<T>(string propertyName, ref T target, T value)
		{
			if(object.ReferenceEquals(target, value))
				return;

			target = value;
			this.OnPropertyChanged(propertyName);
		}

		protected void SetPropertyValue<T>(Expression<Func<T>> propertyExpression, ref T target, T value)
		{
			if(object.ReferenceEquals(target, value))
				return;

			if(propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");

			var property = this.GetPropertyInfo<T>(propertyExpression);

			if(property == null)
				throw new ArgumentException("Invalid expression of the argument", "propertyExpression");

			target = value;
			this.OnPropertyChanged(property.Name);
		}
		#endregion

		#region 激发事件
		protected void OnPropertyChanged(string propertyName)
		{
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");

			var property = this.GetPropertyInfo<T>(propertyExpression);

			if(property == null)
				throw new ArgumentException("Invalid expression of the argument", "propertyExpression");

			this.OnPropertyChanged(property.Name);
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			var handler = this.PropertyChanged;

			if(handler != null)
				handler(this, args);
		}
		#endregion

		#region 私有方法
		private PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> propertyExpression)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");

			var memberExpression = propertyExpression.Body as MemberExpression;

			if(memberExpression == null)
				throw new ArgumentException("Invalid expression of the argument.", "propertyExpression");

			return memberExpression.Member as PropertyInfo;
		}

		private T GetPropertyDefaultValue<T>(PropertyInfo property, T defaultValue)
		{
			if(property == null)
				throw new ArgumentNullException("property");

			var attribute = property.GetCustomAttribute<DefaultValueAttribute>();

			if(attribute != null)
				return Zongsoft.Common.Convert.ConvertValue<T>(attribute.Value);

			return defaultValue;
		}
		#endregion
	}
}
