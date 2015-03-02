/*
 * Authors:
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
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
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
	/// <summary>
	/// 修改服务器上的文件的最后修改时间（GMT）
	/// </summary>
	internal class FtpMfmtCommand : FtpCommand
	{
		public FtpMfmtCommand() : base("MFMT")
		{
		}

		protected override void Run(FtpCommandContext context)
		{
			context.Channel.CheckLogin();

			try
			{
				if(string.IsNullOrWhiteSpace(context.Statement.Argument))
					throw new SyntaxException();

				var arguments = context.Statement.Argument.Split(new[] { ' ' }, 2);
				if(arguments.Length != 2)
					throw new SyntaxException();

				var dateTime = FtpDateUtils.ParseFtpDate(arguments[0]);
				var filePath = context.Channel.MapVirtualPathToLocalPath(arguments[1]);

				var fileInfo = new FileInfo(filePath);
				if(!fileInfo.Exists)
					throw new FileNotFoundException(arguments[1]);

				fileInfo.LastWriteTimeUtc = dateTime;

				context.Channel.Send("213 Date/time changed OK");
			}
			catch(FtpException)
			{
				throw;
			}
			catch(Exception)
			{
				throw new InternalException("");
			}
		}
	}
}
