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
using System.Text;

namespace Zongsoft.Communication.Net.Ftp
{
    internal class FtpListFileFormater
    {
        private const string NEWLINE = "\r\n";
        private const char DELIM = ' ';

        public static string Format(FtpCommandContext context, FileSystemInfo fileInfo)
        {
            var output = new StringBuilder();
            Format(context, fileInfo, output);
            return output.ToString();
        }

        public static void Format(FtpCommandContext context, FileSystemInfo fileInfo, StringBuilder output)
        {
            output.Append(GetPermission(context, fileInfo));
            output.Append(DELIM);
            output.Append(DELIM);
            output.Append(DELIM);
            output.Append(1); //LinkCount
            output.Append(DELIM);
            output.Append("ftp"); //OwnerName
            output.Append(DELIM);
            output.Append("ftp"); //GroupName
            output.Append(DELIM);
            output.Append(GetLength(fileInfo));
            output.Append(DELIM);
            output.Append(GetLastModified(fileInfo));
            output.Append(DELIM);
            output.Append(fileInfo.Name);
            output.Append(NEWLINE);
        }

        private static char[] GetPermission(FtpCommandContext context, FileSystemInfo fileInfo)
        {
            var perm = ("----------").ToCharArray();
            perm[0] = fileInfo is DirectoryInfo ? 'd' : '-';
            perm[1] = 'r';
            perm[2] = !context.Channel.User.ReadOnly ? 'w' : '-';
            perm[0] = fileInfo is DirectoryInfo ? 'x' : '-';
            return perm;
        }

        private static string GetLength(FileSystemInfo fileInfo)
        {
            var charLength = 12;
            long sz = 0;
            if (fileInfo is FileInfo)
                sz = ((FileInfo) fileInfo).Length;

            return sz.ToString().PadLeft(charLength);
        }

        private static string GetLastModified(FileSystemInfo file)
        {
            return FtpDateUtils.FormatUnixDate(file.LastWriteTimeUtc);
        }
    }
}
