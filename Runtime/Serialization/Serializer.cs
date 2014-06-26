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
		public static readonly Serializer Default = new Serializer(new SerializationWriter());
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
			var context = this.CreateContext(serializationStream, graph);

			try
			{
				//通知序列化写入器的当前所处步骤(序列化开始)
				_writer.OnStep(context, SerializationWriterStep.Serializing);

				//进行序列化写入操作
				this.Write(context, new HashSet<object>(ObjectReferenceComparer.Instance));
			}
			finally
			{
				//通知序列化写入器的当前所处步骤(序列化结束)
				_writer.OnStep(context, SerializationWriterStep.Serialized);
			}

			//激发“Serialized”事件
			this.OnSerialized(new SerializationEventArgs(SerializationDirection.Output, serializationStream, graph));
		}
		#endregion

		#region 虚拟方法
		protected virtual SerializationContext CreateContext(Stream serializationStream, object graph)
		{
			return new SerializationContext(this, serializationStream, graph, graph, 0, false);
		}

		protected virtual void Write(SerializationContext context, HashSet<object> stack)
		{
			//设置当前待序列化对象是否为集合类型
			context.IsCollection = context.Value == null ? false : this.IsCollectionType(context.Value.GetType());
			//设置当前待序列化对象是否为循环引用对象
			context.IsCircularReference = context.Value != null && context.Value.GetType().IsValueType ? false : stack.Contains(context.Value);

			//调用序列化写入器的写入方法，以进行实际的序列化写入
			bool continued = _writer.Write(context);

			//进行最大序列化层次的检测判断
			if(_settings.MaximumDepth > -1 && context.Depth >= _settings.MaximumDepth)
				return;

			//进行是否需要继续序列化当前对象的检测判断
			if((!continued) || context.Value == null || this.IsSimpleType(context.Value.GetType()))
				return;

			var originalContainer = context.Container;
			var originalValue = context.Value;
			var originalMember = context.Member;

			try
			{
				context.IncrementDepth();
				context.Container = context.Value;

				if(!context.IsCircularReference && !context.Value.GetType().IsValueType)
					stack.Add(context.Value);

				if(context.IsCollection)
				{
					foreach(var item in (System.Collections.IEnumerable)context.Value)
					{
						context.Value = item;

						this.Write(context, stack);
					}

					return;
				}

				var target = context.Value;
				var members = new List<MemberInfo>();

				members.AddRange(target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public));
				members.AddRange(target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public));

				foreach(var member in members)
				{
					try
					{
						switch(member.MemberType)
						{
							case MemberTypes.Field:
								context.Value = ((FieldInfo)member).GetValue(target);
								break;
							case MemberTypes.Property:
								context.Value = ((PropertyInfo)member).GetValue(target, null);
								break;
						}
					}
					catch
					{
						continue;
					}

					context.Member = member;
					this.Write(context, stack);
				}

				stack.Remove(context.Value);
			}
			finally
			{
				context.Value = originalValue;
				context.Member = originalMember;
				context.Container = originalContainer;
				context.IsCollection = originalValue == null ? false : this.IsCollectionType(originalValue.GetType());
				context.DecrementDepth();

				//通知序列化写入器，本轮写入操作完成。
				_writer.OnStep(context, SerializationWriterStep.Wrote);
			}
		}
		#endregion

		#region 私有方法
		private bool IsSimpleType(Type type)
		{
			if(type.IsPrimitive || type.IsEnum || type.IsValueType || type == typeof(object) || type == typeof(string) || typeof(Type).IsAssignableFrom(type))
				return true;

			return false;
		}

		private bool IsCollectionType(Type type)
		{
			if(type.IsPrimitive || type == typeof(string))
				return false;

			return typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
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
