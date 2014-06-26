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
	public class SerializationWriter : ISerializationWriter
	{
		#region 私有变量
		private IDictionary<SerializationContext, TextWriter> _writers;
		#endregion

		#region 成员字段
		private Encoding _encoding;
		#endregion

		#region 构造函数
		public SerializationWriter()
		{
			_encoding = Encoding.UTF8;
			_writers = new ConcurrentDictionary<SerializationContext, TextWriter>();
		}
		#endregion

		#region 公共属性
		public Encoding Encoding
		{
			get
			{
				return _encoding;
			}
			set
			{
				_encoding = value ?? Encoding.UTF8;
			}
		}
		#endregion

		#region 公共方法
		public bool Write(SerializationContext context)
		{
			var writer = this.GetWriter(context);

			if(writer == null)
				return false;

			var valueText = string.Empty;
			var indentText = this.GetIndentText(context.Depth);

			writer.Write(indentText);

			if(context.Member != null)
				writer.Write(context.MemberName + " : ");

			if(context.Value == null)
			{
				writer.WriteLine("<NULL>");
				return false;
			}

			if(context.IsCircularReference)
			{
				writer.WriteLine("<Circular Reference>");
				return false;
			}

			var isDirectedValue = this.GetDirectedValue(context.Value, out valueText);

			if(isDirectedValue)
			{
				writer.WriteLine(valueText);
			}
			else
			{
				writer.WriteLine(this.GetFriendlyTypeName(context.Value.GetType()));

				if(context.IsCollection)
					writer.WriteLine(indentText + "[");
				else
					writer.WriteLine(indentText + "{");
			}

			return !isDirectedValue;
		}
		#endregion

		#region 显式实现
		void ISerializationWriter.OnStep(SerializationContext context, SerializationWriterStep step)
		{
			var writer = this.GetWriter(context);

			if(writer == null)
				return;

			switch(step)
			{
				case SerializationWriterStep.Wrote:
					if(context.IsCollection)
						writer.WriteLine(this.GetIndentText(context.Depth) + "]");
					else
						writer.WriteLine(this.GetIndentText(context.Depth) + "}");
					break;
				case SerializationWriterStep.Serialized:
					writer.Flush();
					writer.Dispose();
					_writers.Remove(context);
					break;
			}
		}
		#endregion

		#region 虚拟方法
		protected virtual string GetIndentText(int indent)
		{
			return indent > 0 ? new string('\t', indent) : string.Empty;
		}

		protected virtual bool GetDirectedValue(object value, out string valueText)
		{
			valueText = null;

			if(value == null)
				return true;

			Type valueType = value.GetType();
			bool isDirectedValue = valueType.IsPrimitive || valueType.IsEnum || valueType.IsValueType ||
				                   valueType == typeof(object) || valueType == typeof(string) ||
								   typeof(Type).IsAssignableFrom(valueType);

			if(isDirectedValue)
			{
				valueText = value.ToString();
			}
			else
			{
				var converter = TypeDescriptor.GetConverter(value);
				//isDirectedValue = converter != null && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string));

				if(isDirectedValue)
					valueText = converter.ConvertToString(value);
			}

			return isDirectedValue;
		}
		#endregion

		#region 保护方法
		protected TextWriter GetWriter(SerializationContext context)
		{
			if(context == null)
				return null;

			TextWriter writer;

			if(!_writers.TryGetValue(context, out writer))
			{
				writer = new StreamWriter(context.SerializationStream, _encoding);
				_writers.Add(context, writer);
			}

			return writer;
		}

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

				for(int i=0; i< argumentTypes.Length; i++)
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
				return type.FullName + "@" + type.Assembly.GetName().Name;
		}
		#endregion
	}
}
