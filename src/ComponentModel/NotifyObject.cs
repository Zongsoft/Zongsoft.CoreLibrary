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
		private ConcurrentDictionary<string, PropertyToken> _properties;
		#endregion

		#region 保护属性
		protected bool HasProperties
		{
			get
			{
				return _properties != null && _properties.Count > 0;
			}
		}

		protected ConcurrentDictionary<string, PropertyToken> Properties
		{
			get
			{
				if(_properties == null)
					System.Threading.Interlocked.CompareExchange(ref _properties, new ConcurrentDictionary<string, PropertyToken>(), null);

				return _properties;
			}
		}
		#endregion

		#region 保护方法
		protected T GetPropertyValue<T>(string propertyName, T defaultValue = default(T))
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException(nameof(propertyName));

			PropertyToken token;
			var properties = _properties;

			if(properties != null && properties.TryGetValue(propertyName.Trim(), out token))
				return (T)token.Value;

			var property = this.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if(property == null)
				throw new InvalidOperationException(string.Format("The '{0}' property is not exists.", propertyName));

			//返回属性的默认值
			return this.GetPropertyDefaultValue(property, defaultValue);
		}

		protected T GetPropertyValue<T>(string propertyName, Func<T> valueFactory)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException(nameof(propertyName));
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			var properties = _properties;
			PropertyToken token;

			if(properties != null && properties.TryGetValue(propertyName.Trim(), out token))
				return (T)token.Value;

			var value = valueFactory();
			this.SetPropertyValue(propertyName, value);
			return value;
		}

		protected T GetPropertyValue<T>(Expression<Func<T>> propertyExpression, T defaultValue = default(T))
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			var property = this.GetPropertyInfo<T>(propertyExpression);
			var properties = _properties;
			PropertyToken token;

			if(properties != null && properties.TryGetValue(property.Name, out token))
				return (T)token.Value;

			//返回属性的默认值
			return this.GetPropertyDefaultValue(property, defaultValue);
		}

		protected T GetPropertyValue<T>(Expression<Func<T>> propertyExpression, Func<T> valueFactory)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));
			if(valueFactory == null)
				throw new ArgumentNullException(nameof(valueFactory));

			var property = this.GetPropertyInfo<T>(propertyExpression);
			var properties = _properties;
			PropertyToken token;

			if(properties != null && properties.TryGetValue(property.Name, out token))
				return (T)token.Value;

			var value = valueFactory();
			this.SetPropertyValueCore(property, value);
			return value;
		}

		protected void SetPropertyValue(string propertyName, object value)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException(nameof(propertyName));

			var changed = true;
			var properties = this.Properties;
			PropertyToken token;

			//如果指定的属性没有被缓存
			if(!properties.TryGetValue(propertyName, out token))
			{
				var property = this.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if(property == null)
					throw new ArgumentException($"The '{propertyName}' property is not existed.");

				token = this.CreatePropertyToken(property.Name, property.PropertyType, null);
			}

			//调用属性设置通知
			token.Value = this.OnPropertySet(propertyName, token.Type, token.Value, value);

			//设置属性值
			properties.AddOrUpdate(propertyName, token, (_, original) => {
				changed = !object.Equals(token.Value, original.Value);
				return token;
			});

			if(changed)
				this.OnPropertyChanged(propertyName);
		}

		protected void SetPropertyValue<T>(Expression<Func<T>> propertyExpression, T value)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			//解析属性表达式
			var property = this.GetPropertyInfo<T>(propertyExpression);

			//设置属性值
			this.SetPropertyValueCore(property, value);
		}

		protected void SetPropertyValue<T>(string propertyName, ref T target, T value)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException(nameof(propertyName));

			if(object.Equals(target, value))
				return;

			//更新目标值
			target = value;

			//激发“PropertyChanged”事件
			this.OnPropertyChanged(propertyName);
		}

		protected void SetPropertyValue<T>(Expression<Func<T>> propertyExpression, ref T target, T value)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			if(object.Equals(target, value))
				return;

			//获取属性表达式指定的属性信息
			var property = this.GetPropertyInfo<T>(propertyExpression);

			//更新目标的值
			target = value;

			//激发“PropertyChanged”事件
			this.OnPropertyChanged(property.Name);
		}

		private void SetPropertyValueCore(PropertyInfo property, object value)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var changed = true;
			var properties = this.Properties;
			PropertyToken token;

			//如果指定的属性没有被缓存
			if(!properties.TryGetValue(property.Name, out token))
				token = this.CreatePropertyToken(property.Name, property.PropertyType, null);

			//调用属性设置通知
			token.Value = this.OnPropertySet(property.Name, token.Type, token.Value, value);

			//设置属性值
			properties.AddOrUpdate(property.Name, token, (_, original) => {
				changed = !object.Equals(token.Value, original.Value);
				return token;
			});

			if(changed)
				this.OnPropertyChanged(property.Name);
		}
		#endregion

		#region 虚拟方法
		protected virtual object OnPropertySet(string name, Type type, object oldValue, object newValue)
		{
			return newValue;
		}

		protected virtual PropertyToken CreatePropertyToken(string name, Type type, object value)
		{
			return new PropertyToken(name, type, value);
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
				throw new ArgumentNullException(nameof(propertyExpression));

			var memberExpression = propertyExpression.Body as MemberExpression;

			if(memberExpression == null)
				throw new ArgumentException("The expression is not a property expression.", nameof(propertyExpression));

			if(memberExpression.Member.MemberType != MemberTypes.Property)
				throw new InvalidOperationException($"The '{memberExpression.Member.Name}' member is not property.");

			return (PropertyInfo)memberExpression.Member;
		}

		private T GetPropertyDefaultValue<T>(PropertyInfo property, T defaultValue)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var attribute = property.GetCustomAttribute<DefaultValueAttribute>();

			if(attribute != null)
				return Zongsoft.Common.Convert.ConvertValue<T>(attribute.Value);

			return defaultValue;
		}
		#endregion

		#region 嵌套子类
		public class PropertyToken
		{
			public string Name;
			public object Value;
			public readonly Type Type;

			public PropertyToken(string name, Type type, object value)
			{
				if(string.IsNullOrWhiteSpace((name)))
					throw new ArgumentNullException(nameof(name));
				if(type == null)
					throw new ArgumentNullException(nameof(type));

				this.Name = name;
				this.Type = type;
				this.Value = value;
			}
		}
		#endregion
	}
}
