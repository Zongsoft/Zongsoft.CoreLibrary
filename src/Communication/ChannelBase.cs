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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using Zongsoft.ComponentModel;

namespace Zongsoft.Communication
{
	/// <summary>
	/// 定义通道基本功能的抽象基类。
	/// </summary>
	public abstract class ChannelBase : MarshalByRefObject, IChannel, IDisposable
	{
		#region 事件定义
		public event EventHandler<ChannelFailureEventArgs> Failed;
		public event EventHandler<ReceivedEventArgs> Received;
		public event EventHandler<SentEventArgs> Sent;
		public event EventHandler<ChannelEventArgs> Closed;
		public event EventHandler<ChannelEventArgs> Closing;
		#endregion

		#region 成员字段
		private int _channelId;
		private DateTime _lastSendTime;
		private DateTime _lastReceivedTime;
		#endregion

		#region 构造函数
		protected ChannelBase(int channelId)
		{
			_channelId = channelId;
			_lastSendTime = new DateTime(1900, 1, 1);
			_lastReceivedTime = new DateTime(1900, 1, 1);
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前通道的唯一编号。
		/// </summary>
		public int ChannelId
		{
			get
			{
				return _channelId;
			}
		}

		/// <summary>
		/// 获取当前通道是否为空闲状态。
		/// </summary>
		/// <remarks>
		///		<para>对子类实现者：应该确保该属性能即时反应当前通道的真实状态。</para>
		/// </remarks>
		public abstract bool IsIdled
		{
			get;
		}

		/// <summary>
		/// 获取最后一次发送成功的时间。
		/// </summary>
		public DateTime LastSendTime
		{
			get
			{
				return _lastSendTime;
			}
			protected set
			{
				_lastSendTime = value;
			}
		}

		/// <summary>
		/// 获取最后一次成功接收数据的时间。
		/// </summary>
		public DateTime LastReceivedTime
		{
			get
			{
				return _lastReceivedTime;
			}
			protected set
			{
				_lastReceivedTime = value;
			}
		}
		#endregion

		#region 发送方法
		public void Send(string text, object asyncState = null)
		{
			this.Send(text, null, asyncState);
		}

		public virtual void Send(string text, Encoding encoding, object asyncState = null)
		{
			if(text == null)
				throw new ArgumentNullException("text");

			this.Send((encoding ?? Encoding.UTF8).GetBytes(text), asyncState);
		}

		public void Send(byte[] buffer, object asyncState = null)
		{
			if(buffer == null)
				throw new ArgumentNullException("buffer");

			this.Send(buffer, 0, buffer.Length, asyncState);
		}

		public void Send(byte[] buffer, int offset, object asyncState = null)
		{
			if(buffer == null)
				throw new ArgumentNullException("buffer");

			this.Send(buffer, offset, buffer.Length - offset, asyncState);
		}

		public abstract void Send(Stream stream, object asyncState = null);
		public abstract void Send(byte[] buffer, int offset, int count, object asyncState = null);
		#endregion

		#region 激发事件
		protected virtual void OnClosed(ChannelEventArgs args)
		{
			var handler = this.Closed;

			if(handler != null)
				handler(this, args);
		}

		protected virtual void OnClosing(ChannelEventArgs args)
		{
			var handler = this.Closing;

			if(handler != null)
				handler(this, args);
		}

		protected virtual void OnFailed(ChannelFailureEventArgs args)
		{
			var handler = this.Failed;

			if(handler != null)
				handler(this, args);
		}

		protected virtual void OnSent(SentEventArgs args)
		{
			//更新最后发送时间
			_lastSendTime = DateTime.Now;

			var handler = this.Sent;

			if(handler != null)
				handler(this, args);
		}

		protected virtual void OnReceived(ReceivedEventArgs args)
		{
			var handler = this.Received;

			if(handler != null)
				handler(this, args);
		}
		#endregion

		#region 关闭方法
		/// <summary>
		/// 当前通道被关闭时候由子类实现。
		/// </summary>
		protected virtual void OnClose()
		{
		}

		/// <summary>
		/// 关闭当前通道。
		/// </summary>
		/// <remarks>
		///		<para>注意：该方法不允许线程重入，即在多线程调用中，本方法内部会以同步机制运行。</para>
		///		<para>如果当前通道是空闲的(即<seealso cref="IsIdled"/>属性为真)，则该方法不执行任何操作。</para>
		/// </remarks>
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public void Close()
		{
			//如果当前通道是空闲的，则无需关闭。
			//注意：该判断可避免关闭方法被多线程重入。
			if(this.IsIdled)
				return;

			var args = new ChannelEventArgs(this);

			//激发“Closing”关闭前事件
			this.OnClosing(args);

			//如果关闭前事件处理函数取消后续的关闭操作则退出
			//if(args.Cancel)
			//    return;

			//执行子类实现的真正关闭动作
			this.OnClose();

			//激发“Closed”关闭后事件
			this.OnClosed(new ChannelEventArgs(this));
		}
		#endregion

		#region 处置方法
		protected virtual void Dispose(bool disposing)
		{
			this.Close();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
