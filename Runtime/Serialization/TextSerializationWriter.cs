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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace Zongsoft.Runtime.Serialization
{
	public class TextSerializationWriter : TextSerializationWriterBase
	{
		#region 构造函数
		public TextSerializationWriter()
		{
		}
		#endregion

		#region 写入方法
		protected override void OnWrite(SerializationWriterContext context)
		{
			var writer = this.GetWriter(context);

			if(writer == null)
				throw new System.Runtime.Serialization.SerializationException("Can not obtain a text writer.");

			if(context.Index >= 0)
				writer.WriteLine();

			var indentText = this.GetIndentText(context.Depth);
			writer.Write(indentText);

			if(context.Member != null)
				writer.Write(context.MemberName + " : ");

			if(context.Value == null)
			{
				writer.Write("<NULL>");
				return;
			}

			if(context.IsCircularReference)
			{
				writer.Write("<Circular Reference>");
				return;
			}

			var directedValue = string.Empty;
			var isDirectedValue = this.GetDirectedValue(context.Value, out directedValue);

			if(isDirectedValue)
			{
				writer.Write(directedValue);
				writer.Write(" (" + this.GetFriendlyTypeName(context.Value.GetType()) + ")");
			}
			else
			{
				writer.Write(this.GetFriendlyTypeName(context.Value.GetType()));

				if(context.IsCollection)
					writer.Write(writer.NewLine + indentText + "[");
				else
					writer.Write(writer.NewLine + indentText + "{");
			}

			context.Terminated = isDirectedValue;
		}
		#endregion

		#region 重写方法
		protected override void OnWrote(SerializationWriterContext context)
		{
			if(context.Terminated || context.IsCircularReference)
				return;

			var writer = this.GetWriter(context);

			if(writer == null)
				return;

			if(context.IsCollection)
				writer.Write(writer.NewLine + this.GetIndentText(context.Depth) + "]");
			else
				writer.Write(writer.NewLine + this.GetIndentText(context.Depth) + "}");
		}
		#endregion

		#region 保护方法
		protected string GetFriendlyTypeName(Type type)
		{
			if(type == null)
				return string.Empty;

			if(type.IsPrimitive || type == typeof(object) || type == typeof(string))
				return type.Name;

			if(type.IsGenericType)
			{
				string typeName = type.GetGenericTypeDefinition().FullName.Substring(0, type.GetGenericTypeDefinition().FullName.IndexOf('`')) + "<";
				Type[] argumentTypes = type.GetGenericArguments();

				for(int i = 0; i < argumentTypes.Length; i++)
				{
					typeName += this.GetFriendlyTypeName(argumentTypes[i]);

					if(i < argumentTypes.Length - 1)
						typeName += ", ";
				}

				return typeName + ">";
			}

			if(type.FullName.StartsWith("System.", StringComparison.Ordinal) || type.FullName.StartsWith("Zongsoft.", StringComparison.Ordinal))
				return type.FullName;
			else
				return type.FullName + ", " + type.Assembly.GetName().Name;
		}
		#endregion
	}
}
