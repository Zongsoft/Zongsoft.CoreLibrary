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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
	internal class FtpException : Exception
	{
		public FtpException(string message) : base(message)
		{
		}

		public FtpException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	internal class CommandNotImplementedException : FtpException
	{
		/// <summary>
		/// 502 Command not implemented.
		/// </summary>
		public CommandNotImplementedException()
			: base("502 Command not implemented.")
		{
		}
	}

	internal class NeedLoginException : FtpException
	{
		/// <summary>
		/// 530 Please login with USER and PASS.
		/// </summary>
		public NeedLoginException()
			: base("530 Please login with USER and PASS.")
		{
		}
	}

	internal class NeedUserInfoException : FtpException
	{
		/// <summary>
		/// 503 Login with USER first.
		/// </summary>
		public NeedUserInfoException()
			: base("503 Login with USER first.")
		{
		}
	}

	internal class LoginFailException : FtpException
	{
		/// <summary>
		/// 530 Not logged in, user or password incorrect!
		/// </summary>
		public LoginFailException()
			: base("530 Not logged in, user or password incorrect!")
		{
		}
	}

	internal class AccessDeniedException : FtpException
	{
		/// <summary>
		/// 550 "}path}": Permission denied.
		/// </summary>
		public AccessDeniedException(string path)
			: base("550 \"" + path + "\": Permission denied.")
		{
		}
	}

	internal class DirectoryNotFoundException : FtpException
	{
		/// <summary>
		/// 550 "{path}": Directory not found.
		/// </summary>
		public DirectoryNotFoundException(string path)
			: base("550 \"" + path + "\": Directory not found.")
		{
		}
	}

	internal class FileNotFoundException : FtpException
	{
		/// <summary>
		/// 550 "{file}": File not found.
		/// </summary>
		public FileNotFoundException(string file)
			: base("550 \"" + file + "\": File not found.")
		{
		}
	}

	internal class DirectoryExistsException : FtpException
	{
		/// <summary>
		/// 550  "{dir}": Directory already exists.
		/// </summary>
		public DirectoryExistsException(string dir)
			: base("550 \"{dir}\": Directory already exists.")
		{
		}
	}

	internal class InternalException : FtpException
	{
		/// <summary>
		/// 450 Internal error for {desp}
		/// </summary>
		public InternalException(string desp)
			: base("450 Internal error for " + desp)
		{
		}
	}

	internal class BadSeqCommandsException : FtpException
	{
		/// <summary>
		/// 503 Commands bad sequence .
		/// </summary>
		public BadSeqCommandsException()
			: base("503 Commands bad sequence .")
		{
		}
	}

	internal class SyntaxException : FtpException
	{
		/// <summary>
		/// 501 Syntax error in arguments.
		/// </summary>
		public SyntaxException()
			: base("501 Syntax error in arguments.")
		{
		}
	}

	internal class DataConnNotReadyException : FtpException
	{
		/// <summary>
		/// "425 Data connection is not ready."
		/// </summary>
		public DataConnNotReadyException()
			: base("425 Data connection is not ready.")
		{
		}
	}

	/// <summary>
	/// 未登录的异常
	/// </summary>
	internal class NotLoginException : FtpException
	{
		/// <summary>
		/// "530 Please authenticate firtst."
		/// </summary>
		public NotLoginException()
			: base("530 Please authenticate firtst.")
		{
		}
	}

	internal class UnknownTransferModeException : FtpException
	{
		/// <summary>
		/// 550 Error - unknown transfer mode.
		/// </summary>
		public UnknownTransferModeException()
			: base("550 Error - unknown transfer mode.")
		{
		}
	}
}