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
using System.IO;

using Zongsoft.Services.Composition;

namespace Zongsoft.Runtime.Serialization
{
	[Obsolete]
	public class SerializerProviderFilter : IExecutionFilter
	{
		#region 成员字段
		private SerializerProvider _serializerProvider;
		#endregion

		#region 公共属性
		public string Name
		{
			get
			{
				return this.GetType().Name;
			}
		}

		public SerializerProvider SerializerProvider
		{
			get
			{
				return _serializerProvider;
			}
			set
			{
				_serializerProvider = value;
			}
		}
		#endregion

		#region 过滤方法
		public void OnExecuting(ExecutionContext context)
		{
			var stream = context.Parameter as Stream;
			var serializerProvider = _serializerProvider;

			if(serializerProvider != null && stream != null && stream.CanRead && stream.Length > 0)
			{
				var serializer = serializerProvider.GetSerializer(stream);

				if(serializer != null)
				{
					var result = serializer.Deserialize(stream);

					if(result != null)
						context.Parameter = result;
				}
			}
		}

		public void OnExecuted(ExecutionContext context)
		{
		}
		#endregion
	}
}
