/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net
{
	public class FtpClient
	{
		#region 成员变量
		private ICredentials _credentials;
		#endregion

        public bool KeepAlive { get; set; }

		#region 构造函数
		public FtpClient()
		{
		    KeepAlive = true;
			_credentials = null;
		}

		public FtpClient(ICredentials credentials)
		{
			_credentials = credentials;
		}

		public FtpClient(string userName, string password)
		{
			_credentials = new NetworkCredential(userName, password);
		}
		#endregion

		public void UploadFile(Uri url, string fileName)
		{
			using(var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, FileOptions.Asynchronous))
			{
				this.Upload(url, stream, false);
			}
		}

		public void Upload(Uri url, Stream stream, bool closeStream)
		{
			var request = WebRequest.Create(url) as FtpWebRequest;

			if(request == null)
				throw new InvalidOperationException();

            request.KeepAlive = KeepAlive;
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = _credentials;

			var requestStream = request.GetRequestStream();
			stream.CopyTo(requestStream);
            requestStream.Close();

            if(closeStream)
                stream.Close();
		}

		public Stream Download(Uri url)
		{
			var request = WebRequest.Create(url) as FtpWebRequest;

			if(request == null)
				throw new InvalidOperationException();

            request.KeepAlive = KeepAlive;
			request.Method = WebRequestMethods.Ftp.DownloadFile;
			request.Credentials = _credentials;

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			return response.GetResponseStream();
		}

		public void DownloadToFile(Uri url, string fileName)
		{
			var stream = this.Download(url);

			if(stream != null)
			{
				using(var fileStream = File.OpenWrite(fileName))
				{
					stream.CopyTo(fileStream, 1024);
				}
			}
		}
	}
}
