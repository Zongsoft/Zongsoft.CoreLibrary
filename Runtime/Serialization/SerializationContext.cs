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
using System.Collections;
using System.Collections.Generic;

namespace Zongsoft.Runtime.Serialization
{
	public class SerializationContext
	{
		#region 成员字段
		private ISerializer _serializer;
		private Stream _serializationStream;
		private object _serializationObject;
		private object _value;
		private object _container;
		private MemberInfo _member;
		private int _depth;
		private bool _isCollection;
		private bool _isCircularReference;
		#endregion

		#region 构造函数
		public SerializationContext(ISerializer serializer, Stream serializationStream, object serializationObject, object value, int depth, bool isCircularReference)
			: this(serializer, serializationStream, serializationObject, value, depth, isCircularReference, null, null)
		{
		}

		public SerializationContext(ISerializer serializer, Stream serializationStream, object serializationObject, object value, int depth, bool isCircularReference, object container, MemberInfo member)
		{
			if(serializer == null)
				throw new ArgumentNullException("serializer");

			if(serializationObject == null)
				throw new ArgumentNullException("serializationObject");

			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			_serializer = serializer;
			_serializationStream = serializationStream;
			_serializationObject = serializationObject;
			_value = value;
			_depth = depth;
			_member = member;
			_container = container;
			_isCircularReference = isCircularReference;
		}
		#endregion

		#region 公共属性
		public ISerializer Serializer
		{
			get
			{
				return _serializer;
			}
		}

		public Stream SerializationStream
		{
			get
			{
				return _serializationStream;
			}
		}

		public object SerializationObject
		{
			get
			{
				return _serializationObject;
			}
		}

		public int Depth
		{
			get
			{
				return _depth;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
			internal set
			{
				_value = value;
			}
		}

		public object Container
		{
			get
			{
				return _container;
			}
			internal protected set
			{
				_container = value;
			}
		}

		public MemberInfo Member
		{
			get
			{
				return _member;
			}
			internal set
			{
				_member = value;
			}
		}

		public string MemberName
		{
			get
			{
				return _member == null ? null : _member.Name;
			}
		}

		public Type MemberType
		{
			get
			{
				if(_member != null)
				{
					switch(_member.MemberType)
					{
						case MemberTypes.Field:
							return ((FieldInfo)_member).FieldType;
						case MemberTypes.Property:
							return ((PropertyInfo)_member).PropertyType;
					}
				}

				return null;
			}
		}

		public bool IsCollection
		{
			get
			{
				return _isCollection;
			}
			internal set
			{
				if(_value != null && value == true)
				{
					if(!typeof(System.Collections.IEnumerable).IsAssignableFrom(_value.GetType()))
						throw new InvalidOperationException();
				}

				_isCollection = value;
			}
		}

		public bool IsCircularReference
		{
			get
			{
				return _isCircularReference;
			}
			internal set
			{
				_isCircularReference = value;
			}
		}
		#endregion

		#region 内部方法
		internal void IncrementDepth()
		{
			System.Threading.Interlocked.Increment(ref _depth);
		}

		internal void DecrementDepth()
		{
			System.Threading.Interlocked.Decrement(ref _depth);
		}
		#endregion
	}
}
