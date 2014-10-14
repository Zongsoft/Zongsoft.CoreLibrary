/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public static class DataAccessExtension
	{
		#region 扩展方法
		public static void Execute(this IDataAccess objectAccess, string name, object inParameters)
		{
			if(objectAccess == null)
				throw new ArgumentNullException("objectAccess");

			objectAccess.Execute(name, ResolveObjectToParameters(inParameters));
		}

		public static void Execute(this IDataAccess objectAccess, string name, object inParameters, out IDictionary<string, object> outParameters)
		{
			if(objectAccess == null)
				throw new ArgumentNullException("objectAccess");

			objectAccess.Execute(name, ResolveObjectToParameters(inParameters), out outParameters);
		}
		#endregion

		#region 私有方法
		private static IDictionary<string, object> ResolveObjectToParameters(object data)
		{
			if(data == null)
				return null;

			if(data.GetType().IsValueType)
				throw new ArgumentException();

			IDictionary<string, object> parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			var properties = System.ComponentModel.TypeDescriptor.GetProperties(data);

			foreach(PropertyDescriptor property in properties)
			{
				parameters.Add(property.Name, property.GetValue(data));
			}

			return parameters;
		}
		#endregion
	}
}
