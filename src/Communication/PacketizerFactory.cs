/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Concurrent;

namespace Zongsoft.Communication
{
	public class PacketizerFactory<TPacketizer> : MarshalByRefObject, IPacketizerFactory, IDisposable where TPacketizer : class, IPacketizer
	{
		#region 私有变量
		private Func<IChannel, TPacketizer> _creator;
		private readonly ConcurrentDictionary<int, TPacketizer> _items;
		#endregion

		#region 构造函数
		public PacketizerFactory() : this(null)
		{
		}

		public PacketizerFactory(Func<IChannel, TPacketizer> creator)
		{
			if(creator == null)
				_creator = (channel) =>
				{
					return Activator.CreateInstance<TPacketizer>();
				};
			else
				_creator = creator;

			_items = new ConcurrentDictionary<int, TPacketizer>();
		}
		#endregion

		#region 获取方法
		public TPacketizer GetPacketizer(IChannel channel)
		{
			if(channel == null)
				return null;

			TPacketizer result;

			if(_items.TryGetValue(channel.ChannelId, out result))
				return result;

			result = this.CreatePacketizer(channel);

			if(_items.TryAdd(channel.ChannelId, result))
				return result;
			else
				return _items[channel.ChannelId];
		}

		IPacketizer IPacketizerFactory.GetPacketizer(IChannel channel)
		{
			return this.GetPacketizer(channel);
		}
		#endregion

		#region 创建元素
		protected virtual TPacketizer CreatePacketizer(IChannel channel)
		{
			if(_creator != null)
				return _creator(channel);

			return null;
		}
		#endregion

		#region 处置方法
		public void Dispose()
		{
			if(_items.IsEmpty)
				return;

			TPacketizer packetizer;

			while(!_items.IsEmpty)
			{
				var keys = _items.Keys;

				foreach(var key in keys)
				{
					if(_items.TryRemove(key, out packetizer))
					{
						if(packetizer is IDisposable)
							((IDisposable)packetizer).Dispose();
					}
				}
			}
		}
		#endregion
	}
}
