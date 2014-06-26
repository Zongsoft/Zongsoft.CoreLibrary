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
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

using Zongsoft.Diagnostics;

namespace Zongsoft.Communication.Net
{
	public class TcpServerChannelManager : MarshalByRefObject, IDisposable
	{
		#region 私有变量
		private int _channelId;
		private ConcurrentDictionary<int, TcpServerChannel> _activedChannels;
		private Zongsoft.Common.ObjectPool<TcpServerChannel> _channelPool;
		#endregion

		#region 成员变量
		private TcpServer _server;
		private IPacketizerFactory _packetizerFactory;
		#endregion

		#region 构造函数
		public TcpServerChannelManager(TcpServer server) : this(server, null)
		{
		}

		public TcpServerChannelManager(TcpServer server, IPacketizerFactory packetizerFactory)
		{
			if(server == null)
				throw new ArgumentNullException("server");

			//保存当前通道管理器所属的服务器对象
			_server = server;
			//保存当前通道管理器的协议解析器工厂
			_packetizerFactory = packetizerFactory;

			//创建活动通道池
			_activedChannels = new ConcurrentDictionary<int, TcpServerChannel>();

			//挂载服务器的接受事件
			_server.Accepted += new EventHandler<ChannelEventArgs>(Server_Accepted);

			//创建通道对象池
			_channelPool = new Common.ObjectPool<TcpServerChannel>(() =>
			{
				var channel = this.CreateChannel(System.Threading.Interlocked.Increment(ref _channelId));

				if(channel != null && channel.Packetizer == null && _packetizerFactory != null)
				{
					channel.Packetizer = _packetizerFactory.GetPacketizer(channel);
				}

				return channel;
			}, null);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前活动通道总数。
		/// </summary>
		public int ActivedChannelCount
		{
			get
			{
				return _activedChannels.Count;
			}
		}

		/// <summary>
		/// 获取当前通道管理器所属的服务器对象。
		/// </summary>
		public TcpServer Server
		{
			get
			{
				return _server;
			}
		}

		public IPacketizerFactory PacketizerFactory
		{
			get
			{
				return _packetizerFactory;
			}
			set
			{
				_packetizerFactory = value;
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 获取指定编号的活动通道。
		/// </summary>
		/// <param name="channelId">指定的要获取的通道编号。</param>
		/// <returns>返回获取到的<see cref="TcpServerChannel"/>通道对象，如果指定的通道编号是不存在的或该通道已经被关闭则返回空(null)。</returns>
		public TcpServerChannel GetActivedChannel(int channelId)
		{
			TcpServerChannel channel;

			if(_activedChannels.TryGetValue(channelId, out channel))
				return channel;

			return null;
		}

		/// <summary>
		/// 获取当前所有活动的通道集合。
		/// </summary>
		/// <returns>返回的当前所有活动的通道集合，该返回的结果集只表示本方法调用之际的活动通道集的快照。</returns>
		public IEnumerable<TcpServerChannel> GetActivedChannels()
		{
			return _activedChannels.Values;
		}
		#endregion

		#region 获取通道
		internal TcpServerChannel GetChannel()
		{
			var channel = _channelPool.GetObject();
			channel.Closed += Channel_Closed;
			return channel;
		}
		#endregion

		#region 创建通道
		protected virtual TcpServerChannel CreateChannel(int channelId)
		{
			return new TcpServerChannel(this, _channelId);
		}
		#endregion

		#region 事件处理
		private void Server_Accepted(object sender, ChannelEventArgs e)
		{
			_activedChannels.TryAdd(e.Channel.ChannelId, (TcpServerChannel)e.Channel);
		}

		private void Channel_Closed(object sender, ChannelEventArgs e)
		{
			var channel = (TcpServerChannel)e.Channel;

			channel.Closed -= Channel_Closed;

			if(_activedChannels.TryRemove(channel.ChannelId, out channel))
				_channelPool.Release(channel);
		}
		#endregion

		#region 关闭方法
		/// <summary>
		/// 关闭所有活动的通道。
		/// </summary>
		internal protected void Close()
		{
			TcpServerChannel channel;

			foreach(var id in _activedChannels.Keys)
			{
				if(_activedChannels.TryRemove(id, out channel))
					channel.Close();
			}
		}
		#endregion

		#region 释放资源
		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			//关闭所有活动通道
			this.Close();

			if(disposing)
			{
				IDisposable packetizerFacotry = _packetizerFactory as IDisposable;

				if(packetizerFacotry != null)
					packetizerFacotry.Dispose();
			}
		}
		#endregion
	}
}
