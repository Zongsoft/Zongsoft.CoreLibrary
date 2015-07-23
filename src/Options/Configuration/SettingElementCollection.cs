/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Options.Configuration
{
	public class SettingElementCollection : OptionConfigurationElementCollection, ISettingsProvider
	{
		#region 构造函数
		public SettingElementCollection() : base("setting")
		{
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取指定位置的<see cref="SettingElement"/>设置项元素。
		/// </summary>
		/// <param name="index">指定获取的设置项位置号。</param>
		/// <returns>返回指定位置的<see cref="SettingElement"/>设置项元素。</returns>
		public SettingElement this[int index]
		{
			get
			{
				return (SettingElement)this.Items[index];
			}
		}

		/// <summary>
		/// 获取或设置指定设置项的文本值。
		/// </summary>
		/// <param name="name">指定要获取或设置的项目名称。</param>
		/// <returns>返回指定名称对应的文本值，如果指定的名称不存在则返回空(null)，如果属性设置器中(setter)中<paramref name="value"/>参数为空(null)，则表示将其指定名称的设置项删除。</returns>
		public new string this[string name]
		{
			get
			{
				var element = base.Find(name) as SettingElement;

				if(element != null)
					return element.Value;
				else
					return null;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(name))
					throw new ArgumentNullException("name");

				if(value == null)
					this.Remove(name);
				else
				{
					var element = base.Find(name) as SettingElement;

					if(element != null)
						element.Value = value;
					else
						this.Add(new SettingElement(name, value));
				}
			}
		}
		#endregion

		#region 重写方法
		protected override OptionConfigurationElement CreateNewElement()
		{
			return new SettingElement();
		}

		protected override string GetElementKey(OptionConfigurationElement element)
		{
			return ((SettingElement)element).Name;
		}
		#endregion

		#region 显式实现
		object ISettingsProvider.GetValue(string name)
		{
			return this[name];
		}

		void ISettingsProvider.SetValue(string name, object value)
		{
			if(value == null)
				this[name] = null;
			else
				this[name] = value.ToString();
		}
		#endregion
	}
}
