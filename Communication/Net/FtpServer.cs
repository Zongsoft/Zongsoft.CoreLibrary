/*
 * Authors:
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Zongsoft.Collections;
using Zongsoft.Communication.Net.Ftp;
using Zongsoft.Services;
using Zongsoft.Services.Composition;
using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Communication.Net
{
	public class FtpServer : TcpServer
	{
		#region 常量定义
		private const string OPTION_PATH = "/Communication/Net/FtpServer";
		#endregion

		#region 成员字段
		private Configuration.FtpServerOptionElement _configuration;
		private ObjectPool<FtpPasvDataChannel> _pasvDataChannelPool;
		private FtpCommandExecutor _commandExecutor;
		#endregion

		#region 构造函数
		public FtpServer()
			: this(null)
		{
		}

		public FtpServer(Configuration.FtpServerOptionElement configuration)
			: base(21)
		{
			//不要在构造函数检测参数是否空，应该在OnStart方法中检测
			_configuration = configuration;

			_pasvDataChannelPool = new ObjectPool<FtpPasvDataChannel>(
				() =>
				{
					var channel = new FtpPasvDataChannel();
					channel.StartListener();
					return channel;
				}, channel => channel.StopListener());

			_commandExecutor = new FtpCommandExecutor();
		}
		#endregion

		#region 公共属性
		public Configuration.FtpServerOptionElement Configuration
		{
			get
			{
				return _configuration;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				_configuration = value;
			}
		}

		internal ObjectPool<FtpPasvDataChannel> PasvDataChannelPool
		{
			get
			{
				return _pasvDataChannelPool;
			}
		}
		#endregion

		#region 重写方法
		protected override TcpServerChannelManager CreateChannelManager()
		{
			return new FtpServerChannelManager(this, new PacketizerFactory<FtpPacketizer>());
		}

		protected override void OnStart(string[] args)
		{
			if(_configuration == null)
				throw new InvalidOperationException("The value of the 'Configuration' property is null.");

			this.Address = new IPEndPoint(IPAddress.Any, _configuration.Port);

			//调用基类同名方法
			base.OnStart(args);
		}

		protected override void OnStop(string[] args)
		{
			_pasvDataChannelPool.Clear();

			//调用基类同名方法
			base.OnStop(args);
		}

		protected override void OnReceived(ReceivedEventArgs args)
		{
			var statement = args.ReceivedObject as FtpStatement;
			object result = null;

			if(statement != null)
			{
				try
				{
					result = _commandExecutor.Execute(statement.Name, args);
				}
				catch(CommandNotFoundException)
				{
					args.Channel.Send("502 Command not implemented.", null);
				}
			}

			if(result != null)
			{
				//调用基类同名方法
				base.OnReceived(new ReceivedEventArgs(args.Channel, result));
			}
		}
		#endregion

		#region 内部方法
		internal void NotifiyReceived(ReceivedEventArgs args)
		{
			var statement = args.ReceivedObject as FtpStatement;
			if(statement != null)
			{
				//Console.WriteLine("{0}:{1}:{2}", statement.Name, statement.Argument, statement.Result);
			}

			base.OnReceived(args);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_pasvDataChannelPool.Dispose();
				_pasvDataChannelPool = null;
			}

			base.Dispose(disposing);
		}
		#endregion

		#region 嵌套子类
		private class FtpCommandExecutor : CommandExecutorBase
		{
			public FtpCommandExecutor()
			{
				this.Root.Loader = new FtpCommandLoader();
			}

			public object Execute(string name, ReceivedEventArgs args)
			{
				return base.Execute(name, args);
			}

			protected override object OnExecute(CommandExecutorContext context)
			{
				var args = (ReceivedEventArgs)context.Parameter;

				return context.Command.Execute(
					new FtpCommandContext(
						context.Command,
						this,
						(FtpServerChannel)args.Channel,
						(FtpStatement)args.ReceivedObject));
			}
		}
		#endregion
	}
}