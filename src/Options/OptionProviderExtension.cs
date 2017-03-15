/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2005-2008 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Options
{
	public static class OptionProviderExtension
	{
		#region 公共扩展
		public static T GetOptionObject<T>(this IOptionProvider optionProvider) where T : class
		{
			if(optionProvider == null)
				throw new ArgumentNullException("optionProvider");

			string path = GetOptionPath(typeof(T));

			if(string.IsNullOrWhiteSpace(path))
				return default(T);
			else
				return optionProvider.GetOptionValue(path) as T;
		}

		public static void SetOptionObject<T>(this IOptionProvider optionProvider, object optionObject) where T : class
		{
			if(optionProvider == null)
				throw new ArgumentNullException("optionProvider");

			string path = GetOptionPath(typeof(T));

			if(string.IsNullOrWhiteSpace(path))
				throw new InvalidOperationException(string.Format("Invalid generic type '{0}'.", typeof(T).AssemblyQualifiedName));

			optionProvider.SetOptionValue(path, optionObject);
		}
		#endregion

		#region 内部方法
		internal static string GetOptionPath(Type type)
		{
			if(type == null)
				return null;

			var field = type.GetField("Path", (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
			if(field != null && field.FieldType == typeof(string))
				return (string)field.GetValue(null);

			var property = type.GetProperty("Path", (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
			if(property != null && property.CanRead && property.PropertyType == typeof(string))
				return (string)property.GetValue(null, null);

			return null;
		}
		#endregion
	}
}
