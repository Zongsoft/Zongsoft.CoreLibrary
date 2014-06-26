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

		#region 保护方法
		protected void SetValue<T>(ref T target, T value, string propertyName)
		{
			if(object.ReferenceEquals(target, value))
				return;

			target = value;
			this.OnPropertyChanged(propertyName);
		}

		protected void SetValue<T>(ref T target, T value, Expression<Func<T>> propertyExpression)
		{
			if(object.ReferenceEquals(target, value))
				return;

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
		#endregion
	}
}
