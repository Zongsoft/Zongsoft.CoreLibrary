#region File Header

// Authors:
//    钟峰(Popeye Zhong) <zongsoft@gmail.com>
//    邓祥云(X.Z. Deng) <627825056@qq.com>
//  
// Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
// 
// This file is part of Zongsoft.CoreLibrary.
// 
// Zongsoft.CoreLibrary is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with Zongsoft.CoreLibrary; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Zongsoft.Common;

namespace Zongsoft.Communication.Net.Ftp
{
    internal class FtpPortDataChannel : IFtpDataChannel, IDisposable
    {
        #region 私有变量

        private IPEndPoint _address;
        private Socket _client;
        private object _syncRoot = new object();
        private SocketAsyncEventArgs _receiveArgs;
        private SocketAsyncEventArgs _connectArgs;
        private SocketAsyncEventArgsPool _sendArgsPool;

        #endregion

        #region 构造函数

        public FtpPortDataChannel(IPEndPoint address)
            : this(address, 1024 * 32)
        {
        }

        public FtpPortDataChannel(IPEndPoint address, int bufferSize)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _address = address;

            var buffer = new byte[bufferSize];
            _receiveArgs = new SocketAsyncEventArgs();
            _receiveArgs.SetBuffer(buffer, 0, buffer.Length);
            _receiveArgs.Completed += OnReceiveCompleted;

            _connectArgs = new SocketAsyncEventArgs();
            _connectArgs.Completed += OnConnectCompleted;

            _sendArgsPool = new SocketAsyncEventArgsPool(OnSendCompleted);
        }

        #endregion

        #region 成员属性

        public event ErrorEventHandler Error;
        public event EventHandler Connected;
        public event EventHandler Closed;
        public event ReceiveDataEventHandler Received;

        public FtpServerChannel ServerChannel { get; set; }

        public Socket Socket
        {
            get { return _client; }
        }

        public bool IsConnected
        {
            get { return _client != null && _client.Connected; }
        }

        #endregion

        #region 成员方法

        public void Connect()
        {
            if (_client != null) return;

            lock (_syncRoot)
            {
                if (_client != null) return;

                try
                {
                    _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    _client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
                    _client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                    _connectArgs.RemoteEndPoint = _address;

                    if (!_client.ConnectAsync(_connectArgs))
                        OnConnectCompleted(_client, _connectArgs);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    Close();
                }
            }
        }

        public void Receive()
        {
            var client = _client;
            if (client == null)
                return; //throw new InvalidOperationException("Channel closed");

            try
            {
                if (!client.ReceiveAsync(_receiveArgs))
                    OnReceiveCompleted(client, _receiveArgs);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public void Close()
        {
            var client = Interlocked.Exchange(ref _client, null);
            if (client == null) return;

            try
            {

                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch { }

            OnClosed();
            _sendArgsPool.Clear();
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            var client = _client;
            if (client == null)
                throw new InvalidOperationException("Channel closed");

            try
            {
                var args = _sendArgsPool.GetObject();
                args.SetBuffer(buffer, offset, count);

                if (!client.SendAsync(args))
                    OnSendCompleted(client, args);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        public bool SendFile(FileInfo fileInfo, long offset)
        {
            var client = _client;
            if (client == null)
                throw new InvalidOperationException("Channel closed");

            try
            {
                using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (offset > 0)
                        stream.Seek(offset, SeekOrigin.Begin);

                    int count;
                    var buffer = new byte[1024 * 32];

                    while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        client.Send(buffer, 0, count, SocketFlags.None);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                OnError(ex);
            }

            return false;
        }

        public void Dispose()
        {
            Close();

            try
            {
                _receiveArgs.SetBuffer(null, 0, 0);
                _receiveArgs.Dispose();

                _connectArgs.Dispose();
            }
            catch (Exception) { }
        }

        #endregion

        #region 激发事件

        protected virtual void OnError(Exception exception)
        {
            ErrorEventHandler handler = Error;
            if (handler != null) handler(this, exception);
        }


        protected virtual void OnConnected()
        {
            EventHandler handler = Connected;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        protected virtual void OnClosed()
        {
            EventHandler handler = Closed;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        protected virtual void OnReceived(byte[] buffer, int offset, int count)
        {
            ReceiveDataEventHandler handler = Received;
            if (handler != null) handler(this, buffer, offset, count);
        }

        #endregion

        #region 私有方法

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                //Socket 正常关闭
                if (e.SocketError == SocketError.OperationAborted || e.SocketError == SocketError.Interrupted ||
                    e.SocketError == SocketError.NotSocket)
                    return;

                OnError(new SocketException((int)e.SocketError));
                Close();
            }
            else
            {
                OnConnected();
            }
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                OnReceived(e.Buffer, e.Offset, e.BytesTransferred);

                Receive();
            }
            else
            {
                if (e.SocketError != SocketError.Success)
                    OnError(new SocketException((int)e.SocketError));

                Close();
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                if (e.SocketError != SocketError.Success)
                    OnError(new SocketException((int)e.SocketError));

                Close();
            }

            _sendArgsPool.Release(e);
        }

        #endregion
    }
}