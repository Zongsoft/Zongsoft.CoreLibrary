/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Options.Configuration
{
	public class ConnectionStringElementCollection : OptionConfigurationElementCollection<ConnectionStringElement>, ISettingsProvider
	{
		#region 构造函数
		public ConnectionStringElementCollection() : base("connectionString")
		{
		}
		#endregion

		#region 重写方法
		protected override OptionConfigurationElement CreateNewElement()
		{
			return new ConnectionStringElement();
		}

		protected override string GetElementKey(OptionConfigurationElement element)
		{
			return ((ConnectionStringElement)element).Name;
		}
		#endregion

		#region 接口实现
		object ISettingsProvider.GetValue(string name)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			return this[name];
		}

		void ISettingsProvider.SetValue(string name, object value)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			string text, provider = null;

			if(value is string)
			{
				text = (string)value;
			}
			else if(value is ConnectionStringElement)
			{
				text = ((ConnectionStringElement)value).Value;
				provider = ((ConnectionStringElement)value).Provider;
			}
			else if(value is SettingElement)
			{
				text = ((SettingElement)value).Value;
			}
			else
				throw new InvalidOperationException("Unsupported value type.");

			if(this.ContainsKey(name))
			{
				this[name].Value = text;

				if(provider != null)
					this[name].Provider = provider;
			}
			else
			{
				this.Add(new ConnectionStringElement(name, text, provider));
			}
		}
		#endregion
	}
}
