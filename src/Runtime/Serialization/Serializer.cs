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
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Runtime.Serialization
{
	public class Serializer : ISerializer
	{
		#region 单例模式
		public static readonly Serializer Text = new Serializer(new TextSerializationWriter());
		public static readonly Serializer Json = new Serializer(new JsonSerializationWriter());
		#endregion

		#region 事件定义
		public event EventHandler<SerializationEventArgs> Serializing;
		public event EventHandler<SerializationEventArgs> Serialized;
		#endregion

		#region 成员字段
		private SerializerSettings _settings;
		private ISerializationWriter _writer;
		#endregion

		#region 构造函数
		public Serializer(ISerializationWriter writer) : this(writer, null)
		{
		}

		public Serializer(ISerializationWriter writer, SerializerSettings settings)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");

			_writer = writer;
			_settings = settings ?? new SerializerSettings();
		}
		#endregion

		#region 公共属性
		public SerializerSettings Settings
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
		public object Deserialize(Stream serializationStream)
		{
			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			throw new NotImplementedException();
		}

		public void Serialize(Stream serializationStream, object graph)
		{
			if(_writer == null)
				throw new InvalidOperationException("The value of property 'Writer' is null.");

			if(graph == null)
				return;

			//激发“Serializing”事件
			this.OnSerializing(new SerializationEventArgs(SerializationDirection.Output, serializationStream, graph));

			//创建序列化上下文对象
			var context = new SerializationContext(this, serializationStream, graph);

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
				if(_settings.MaximumDepth > -1 && context.Depth >= _settings.MaximumDepth)
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

		#region 嵌套子类
		private class ObjectReferenceComparer : System.Collections.IEqualityComparer, IEqualityComparer<object>
		{
			public static readonly ObjectReferenceComparer Instance = new ObjectReferenceComparer();

			public bool Equals(object x, object y)
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
