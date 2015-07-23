/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Runtime.Serialization
{
	public class Serializer : ISerializer, ITextSerializer
	{
		#region 静态字段
		private static ITextSerializer _text = new Serializer(new TextSerializationWriter(), new TextSerializationSettings());
		private static ITextSerializer _json = new Serializer(new JsonSerializationWriter(), new TextSerializationSettings());
		#endregion

		#region 事件定义
		public event EventHandler<SerializationEventArgs> Serializing;
		public event EventHandler<SerializationEventArgs> Serialized;
		#endregion

		#region 成员字段
		private SerializationSettings _settings;
		private ISerializationWriter _writer;
		#endregion

		#region 构造函数
		public Serializer(ISerializationWriter writer) : this(writer, null)
		{
		}

		public Serializer(ISerializationWriter writer, SerializationSettings settings)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");

			_writer = writer;
			_settings = settings ?? new SerializationSettings();
		}
		#endregion

		#region 静态属性
		public static ITextSerializer Text
		{
			get
			{
				return _text;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_text = value;
			}
		}

		public static ITextSerializer Json
		{
			get
			{
				return _json;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_json = value;
			}
		}
		#endregion

		#region 公共属性
		public SerializationSettings Settings
		{
			get
			{
				return _settings;
			}
		}

		public ISerializationWriter Writer
		{
			get
			{
				return _writer;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_writer = value;
			}
		}
		#endregion

		#region 公共方法
		public T Deserialize<T>(Stream serializationStream)
		{
			return (T)this.Deserialize(serializationStream, typeof(Type));
		}

		public object Deserialize(Stream serializationStream)
		{
			throw new NotImplementedException();
		}

		public object Deserialize(Stream serializationStream, Type type)
		{
			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			if(type == null)
				throw new ArgumentNullException("type");

			throw new NotImplementedException();
		}

		public object Deserialize(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			throw new NotImplementedException();
		}

		public object Deserialize(TextReader reader, Type type)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			if(type == null)
				throw new ArgumentNullException("type");

			throw new NotImplementedException();
		}

		public T Deserialize<T>(TextReader reader)
		{
			return (T)this.Deserialize(reader, typeof(T));
		}

		public object Deserialize(string text)
		{
			using(var reader = new StringReader(text))
			{
				return this.Deserialize(reader);
			}
		}

		public object Deserialize(string text, Type type)
		{
			using(var reader = new StringReader(text))
			{
				return this.Deserialize(reader, type);
			}
		}

		public T Deserialize<T>(string text)
		{
			using(var reader = new StringReader(text))
			{
				return this.Deserialize<T>(reader);
			}
		}

		public string Serialize(object graph, TextSerializationSettings settings = null)
		{
			if(graph == null)
				return null;

			using(var writer = new StringWriter())
			{
				this.Serialize(writer, graph, settings);

				return writer.ToString();
			}
		}

		public void Serialize(TextWriter writer, object graph, TextSerializationSettings settings = null)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");

			if(graph == null)
				return;

			using(var stream = new MemoryStream())
			{
				this.Serialize(stream, graph);

				stream.Position = 0;

				using(var reader = new StreamReader(stream, writer.Encoding))
				{
					string line = null;

					while((line = reader.ReadLine()) != null)
					{
						writer.WriteLine(line);
					}
				}
			}
		}

		public void Serialize(Stream serializationStream, object graph, SerializationSettings settings = null)
		{
			if(_writer == null)
				throw new InvalidOperationException("The value of property 'Writer' is null.");

			if(graph == null)
				return;

			//激发“Serializing”事件
			this.OnSerializing(new SerializationEventArgs(SerializationDirection.Output, serializationStream, graph));

			//创建序列化上下文对象
			var context = new SerializationContext(this, serializationStream, graph, (settings ?? _settings));

			try
			{
				//通知序列化写入器的当前所处步骤(序列化开始)
				_writer.OnSerializing(context);

				//进行序列化写入操作
				this.Write(new SerializationWriterContext(_writer, context, null, graph, null, -1, 0, false, this.IsCollection(graph)), new HashSet<object>());
			}
			finally
			{
				//通知序列化写入器的当前所处步骤(序列化结束)
				_writer.OnSerialized(context);
			}

			//激发“Serialized”事件
			this.OnSerialized(new SerializationEventArgs(SerializationDirection.Output, serializationStream, graph));
		}
		#endregion

		#region 虚拟方法
		protected virtual void Write(SerializationWriterContext context, HashSet<object> stack)
		{
			try
			{
				//调用序列化写入器的写入方法，以进行实际的序列化写入
				_writer.Write(context);

				//如果当前序列化的成员是循环引用或者被强制终止则退出
				if(context.IsCircularReference || context.Terminated)
					return;

				//判断该是否已经达到允许的序列化的层次
				if(context.Settings.MaximumDepth > -1 && context.Depth >= context.Settings.MaximumDepth)
					return;

				//判断该是否需要继续序列化当前对象
				if(context.Value == null || this.IsTermination(context))
					return;

				//表示当前序列化对象是否已经序列化过
				bool exists = false;

				if(!context.Value.GetType().IsValueType)
					exists = stack.Add(context.Value);

				if(context.IsCollection)
				{
					int index = 0;

					foreach(var item in (System.Collections.IEnumerable)context.Value)
					{
						this.Write(this.CreateWriterContext(stack, context, item, null, index++), stack);
					}
				}
				else
				{
					this.WriteMembers(context, stack);
				}

				if(stack.Count > 0 && exists)
					stack.Remove(context.Value);
			}
			finally
			{
				//通知序列化写入器，本轮写入操作完成。
				_writer.OnWrote(context);
			}
		}

		protected virtual bool IsTermination(SerializationWriterContext context)
		{
			if(context.Terminated || context.Value == null)
				return true;

			var type = context.Value.GetType();

			return type.IsPrimitive || type.IsEnum ||
			       type == typeof(decimal) || type == typeof(DateTime) || type == typeof(DBNull) || type == typeof(Guid) ||
			       type == typeof(object) || type == typeof(string) || typeof(Type).IsAssignableFrom(type);
		}

		protected bool IsCollection(object value)
		{
			if(value == null)
				return false;

			return this.IsCollectionType(value.GetType());
		}

		protected virtual bool IsCollectionType(Type type)
		{
			if(type == null || type.IsPrimitive || type == typeof(string))
				return false;

			return typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
		}
		#endregion

		#region 私有方法
		private void WriteMembers(SerializationWriterContext context, HashSet<object> stack)
		{
			var target = context.Value;
			var members = new List<MemberInfo>();

			members.AddRange(target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public));
			members.AddRange(target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public));

			int index = 0;

			foreach(var member in members)
			{
				object value = null;

				try
				{
					switch(member.MemberType)
					{
						case MemberTypes.Field:
							value = ((FieldInfo)member).GetValue(target);
							break;
						case MemberTypes.Property:
							value = ((PropertyInfo)member).GetValue(target, null);
							break;
					}
				}
				catch
				{
					continue;
				}

				this.Write(this.CreateWriterContext(stack, context, value, member, index++), stack);
			}
		}

		private bool IsCircularReference(object value, HashSet<object> stack)
		{
			if(value == null || stack == null || stack.Count < 1)
				return false;

			return value.GetType().IsValueType ? false : stack.Contains(value);
		}

		private SerializationWriterContext CreateWriterContext(HashSet<object> stack, SerializationWriterContext context, object value, MemberInfo member, int index)
		{
			return new SerializationWriterContext(
						context.Writer,
						context.SerializationContext,
						context.Value,
						value,
						member,
						index,
						context.Depth + 1,
						this.IsCircularReference(value, stack),
						this.IsCollection(value));

		}
		#endregion

		#region 激发事件
		protected virtual void OnSerializing(SerializationEventArgs args)
		{
			if(this.Serializing != null)
				this.Serializing(this, args);
		}

		protected virtual void OnSerialized(SerializationEventArgs args)
		{
			if(this.Serialized != null)
				this.Serialized(this, args);
		}
		#endregion

		#region 静态方法
		public static bool CanSerialize(object graph)
		{
			if(graph == null)
				return false;

			if(typeof(ISerializable).IsAssignableFrom(graph.GetType()))
				return true;

			var attribute = (SerializerAttribute)Attribute.GetCustomAttribute(graph.GetType(), typeof(SerializerAttribute));

			return (attribute != null && attribute.Type != null);
		}

		public static bool TrySerialize(Stream serializationStream, object graph)
		{
			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			if(graph == null)
				return false;

			if(typeof(ISerializable).IsAssignableFrom(graph.GetType()))
			{
				((ISerializable)graph).Serialize(serializationStream);
				return true;
			}

			var attribute = (SerializerAttribute)Attribute.GetCustomAttribute(graph.GetType(), typeof(SerializerAttribute));

			if(attribute != null && attribute.Type != null)
			{
				var serializer = Activator.CreateInstance(attribute.Type) as ISerializer;

				if(serializer != null)
				{
					serializer.Serialize(serializationStream, graph);
					return true;
				}
			}

			return false;
		}
		#endregion

		#region 嵌套子类
		private class ObjectReferenceComparer : System.Collections.IEqualityComparer, IEqualityComparer<object>
		{
			public static readonly ObjectReferenceComparer Instance = new ObjectReferenceComparer();

			public new bool Equals(object x, object y)
			{
				return object.ReferenceEquals(x, y);
			}

			public int GetHashCode(object obj)
			{
				if(obj == null)
					return 0;

				return obj.GetHashCode();
			}
		}
		#endregion
	}
}
