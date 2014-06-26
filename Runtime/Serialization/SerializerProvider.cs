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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Zongsoft.Runtime.Serialization
{
	[Obsolete]
	public class SerializerProvider
	{
		#region 成员字段
		private HashAlgorithm _hashEvaluator;
		private IDictionary<byte[], ISerializer> _items;
		#endregion

		#region 单例模式
		public static readonly SerializerProvider Default = new SerializerProvider();
		#endregion

		#region 构造函数
		public SerializerProvider()
		{
			_hashEvaluator = new MD5CryptoServiceProvider();
			_items = new Dictionary<byte[], ISerializer>(Zongsoft.Collections.BinaryComparer.Default);
		}
		#endregion

		#region 公共属性
		public HashAlgorithm HashEvaluator
		{
			get
			{
				return _hashEvaluator;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				if(_items.Count > 0)
					throw new InvalidOperationException();

				_hashEvaluator = value;
			}
		}
		#endregion

		#region 注册方法
		public void Register(string name, ISerializer value)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			var key = this.GetHashedKey(Encoding.UTF8.GetBytes(name));
			_items.Add(key, value);
		}
		#endregion

		#region 注销方法
		public void Unregister(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			var key = this.GetHashedKey(Encoding.UTF8.GetBytes(name));
			_items.Remove(key);
		}
		#endregion

		#region 解析方法
		public ISerializer GetSerializer(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			var key = this.GetHashedKey(Encoding.UTF8.GetBytes(name));
			return this.GetSerializer(key);
		}

		public ISerializer GetSerializer(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			if(stream.CanRead && stream.Length >= _hashEvaluator.HashSize)
			{
				byte[] key = new byte[_hashEvaluator.HashSize];
				long position = stream.Position;

				if(stream.Read(key, 0, key.Length) == key.Length)
					return this.GetSerializer(key);

				if(stream.CanSeek)
					stream.Position = position;
			}

			return null;
		}

		public ISerializer GetSerializer(byte[] hashedKey)
		{
			ISerializer result;

			if(_items.TryGetValue(hashedKey, out result))
				return result;
			else
				return null;
		}
		#endregion

		#region 保护方法
		protected virtual byte[] GetHashedKey(byte[] source)
		{
			if(source == null || source.Length < 1)
				return null;

			return _hashEvaluator.ComputeHash(source);
		}
		#endregion
	}
}
