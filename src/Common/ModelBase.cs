/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2014-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Zongsoft.Common
{
	public abstract class ModelBase : System.ComponentModel.INotifyPropertyChanged
	{
		#region 静态常量
		private static readonly string[] EmptyArray = new string[0];
		#endregion

		#region 事件声明
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region 成员字段
		private ConcurrentDictionary<string, PropertyToken> _properties;
		private ConcurrentDictionary<string, object> _changedProperties;
		#endregion

		#region 构造函数
		protected ModelBase()
		{
		}

		protected ModelBase(ModelBase model)
		{
			if(model == null)
				return;

			var properties = model.GetChangedProperties();

			foreach(var property in properties)
			{
				this.SetPropertyValue(property.Key, property.Value);
			}
		}
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

			if(properties != null && properties.TryGetValue(propertyName, out token))
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

			if(properties != null && properties.TryGetValue(propertyName, out token))
				return (T)token.Value;

			var value = valueFactory();
			this.SetPropertyValue(propertyName, value);
			return value;
		}

		protected T GetPropertyValue<T>(Expression<Func<T>> propertyExpression, T defaultValue = default(T))
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			var property = this.GetPropertyInfo(propertyExpression);
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

			var property = this.GetPropertyInfo(propertyExpression);
			var properties = _properties;
			PropertyToken token;

			if(properties != null && properties.TryGetValue(property.Name, out token))
				return (T)token.Value;

			var value = valueFactory();
			this.SetPropertyValueCore(property, value);
			return value;
		}

		protected void SetPropertyValue<T>(string propertyName, ref T target, T value)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException(nameof(propertyName));

			//更新目标值
			target = value;

			//激发“PropertyChanged”事件
			this.RaisePropertyChanged(propertyName, value);
		}

		protected void SetPropertyValue<T>(Expression<Func<T>> propertyExpression, ref T target, T value)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			//获取属性表达式指定的属性信息
			var property = this.GetPropertyInfo(propertyExpression);

			//更新目标的值
			target = value;

			//激发“PropertyChanged”事件
			this.RaisePropertyChanged(property.Name, value);
		}

		protected void SetPropertyValue(string propertyName, object value, Action<object, object> onChanged = null)
		{
			if(string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentNullException(nameof(propertyName));

			var changed = true;
			var properties = this.Properties;
			object originalValue = null;

			properties.AddOrUpdate(propertyName,
				key =>
				{
					var property = this.GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if(property == null)
						throw new ArgumentException($"The '{key}' property is not existed.");

					//获取属性的默认值，注意：不能使用反射获取属性值，因为属性的获取器中可能会调用该方法而导致死循环
					originalValue = this.GetPropertyDefaultValue(property);

					var propertyValue = this.OnPropertySet(property.Name, property.PropertyType, originalValue, value);
					return this.CreatePropertyToken(property.Name, property.PropertyType, propertyValue);
				}, (_, original) =>
				{
					originalValue = original.Value;
					var propertyValue = this.OnPropertySet(original.Name, original.Type, original.Value, value);
					changed = !object.Equals(propertyValue, original.Value);

					if(changed)
						return original.Clone(propertyValue);
					else
						return original;
				});

			if(changed)
			{
				var newValue = properties[propertyName].Value;

				if(onChanged != null)
					onChanged(originalValue, newValue);

				//激发“PropertyChanged”事件，并将当前属性加入到更改集中
				this.RaisePropertyChanged(propertyName, newValue);
			}
		}

		protected void SetPropertyValue<T>(Expression<Func<T>> propertyExpression, T value, Action<T, T> onChanged = null)
		{
			if(propertyExpression == null)
				throw new ArgumentNullException(nameof(propertyExpression));

			//解析属性表达式
			var property = this.GetPropertyInfo(propertyExpression);

			//设置属性值
			if(onChanged == null)
				this.SetPropertyValueCore(property, value);
			else
				this.SetPropertyValueCore(property, value, (oldValue, newValue) => onChanged((T)oldValue, (T)newValue));
		}

		private void SetPropertyValueCore(PropertyInfo property, object value, Action<object, object> onChanged = null)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var changed = true;
			var properties = this.Properties;
			object originalValue = null;

			properties.AddOrUpdate(property.Name,
				key =>
				{
					//获取属性的默认值，注意：不能使用反射获取属性值，因为属性的获取器中可能会调用该方法而导致死循环
					originalValue = this.GetPropertyDefaultValue(property);

					var propertyValue = this.OnPropertySet(property.Name, property.PropertyType, originalValue, value);
					return this.CreatePropertyToken(property.Name, property.PropertyType, propertyValue);
				}, (_, original) =>
				{
					originalValue = original.Value;
					var propertyValue = this.OnPropertySet(original.Name, original.Type, original.Value, value);
					changed = !object.Equals(propertyValue, original.Value);

					if(changed)
						return original.Clone(propertyValue);
					else
						return original;
				});

			if(changed)
			{
				var newValue = properties[property.Name].Value;

				if(onChanged != null)
					onChanged(originalValue, newValue);

				//激发“PropertyChanged”事件，并将当前属性加入到更改集中
				this.RaisePropertyChanged(property.Name, newValue);
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 返回当前对象是否有属性值被改变过。
		/// </summary>
		public bool HasChanges()
		{
			return _changedProperties != null && _changedProperties.Count > 0;
		}

		/// <summary>
		/// 返回指定名称的属性是否被改变过。
		/// </summary>
		/// <param name="names">指定要判断的属性名数组。</param>
		/// <returns>返回一个值，指示指定的属性是否发生过改变。</returns>
		/// <remarks>
		///		<para>如果指定了多个属性名，则其中任意一个属性值发生过改变，返回值即为真(True)。</para>
		///		<para>如果没有指定属性名（即<paramref name="names"/>参数为空(null)或零个成员）则该实例中只要有任何属性发生过改变都返回真(True)。</para>
		/// </remarks>
		public bool HasChanges(params string[] names)
		{
			var isChanged = _changedProperties != null && _changedProperties.Count > 0;

			if(names == null || names.Length == 0)
				return isChanged;

			if(isChanged)
			{
				foreach(var name in names)
				{
					if(_changedProperties.ContainsKey(name))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 获取当前对象中被改变过的属性集。
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, object> GetChangedProperties()
		{
			if(_changedProperties == null)
				System.Threading.Interlocked.CompareExchange(ref _changedProperties, new ConcurrentDictionary<string, object>(), null);

			return _changedProperties;
		}

		/// <summary>
		/// 更新当前对象的属性值。
		/// </summary>
		/// <param name="properties">指定要更新的属性集。</param>
		public void Update(IDictionary<string, object> properties)
		{
			if(properties == null)
				return;

			foreach(var property in properties)
			{
				this.SetPropertyValue(property.Key, property.Value);
			}
		}

		/// <summary>
		/// 更新当前对象的属性值。
		/// </summary>
		/// <param name="model">指定要更新的模型对象。</param>
		public void Update(ModelBase model)
		{
			if(model != null)
				this.Update(model.GetChangedProperties());
		}
		#endregion

		#region 虚拟方法
		protected virtual object OnPropertySet(string name, Type type, object oldValue, object newValue)
		{
			return Zongsoft.Common.Convert.ConvertValue(newValue, type);
		}

		protected virtual PropertyToken CreatePropertyToken(string name, Type type, object value)
		{
			return new PropertyToken(name, type, value);
		}
		#endregion

		#region 激发事件
		protected void RaisePropertyChanged(string propertyName, object value)
		{
			//将发生改变的属性加入到变更集中
			this.SetChangedProperty(propertyName, value);

			//激发“PropertyChanged”事件
			this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			var handler = this.PropertyChanged;

			if(handler != null)
				handler(this, args);
		}
		#endregion

		#region 私有方法
		private void SetChangedProperty(string name, object value)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if(_changedProperties == null)
				System.Threading.Interlocked.CompareExchange(ref _changedProperties, new ConcurrentDictionary<string, object>(), null);

			_changedProperties[name] = value;
		}

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

		private object GetPropertyDefaultValue(PropertyInfo property)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var attribute = property.GetCustomAttribute<DefaultValueAttribute>();

			if(attribute != null)
				return Zongsoft.Common.Convert.ConvertValue(attribute.Value, property.PropertyType);

			return Zongsoft.Common.TypeExtension.GetDefaultValue(property.PropertyType);
		}

		private T GetPropertyDefaultValue<T>(PropertyInfo property, T defaultValue)
		{
			if(property == null)
				throw new ArgumentNullException(nameof(property));

			var attribute = property.GetCustomAttribute<DefaultValueAttribute>();

			if(attribute != null)
				return Zongsoft.Common.Convert.ConvertValue<T>(attribute.Value, defaultValue);

			return defaultValue;
		}
		#endregion

		#region 嵌套子类
		public struct PropertyToken
		{
			#region 成员字段
			public readonly string Name;
			public readonly Type Type;
			public object Value;
			#endregion

			#region 构造函数
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
			#endregion

			#region 公共方法
			public PropertyToken Clone(object value)
			{
				return new PropertyToken(this.Name, this.Type, value);
			}
			#endregion

			#region 重写方法
			public override string ToString()
			{
				return string.Format("{0}({1})={2}", this.Name, this.Type.Name, this.Value);
			}
			#endregion
		}
		#endregion
	}
}
