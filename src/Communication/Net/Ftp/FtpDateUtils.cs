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
using System.Globalization;

namespace Zongsoft.Communication.Net.Ftp
{
	internal static class FtpDateUtils
	{
		private const string FtpDataFormats = "yyyyMMddHHmmss";

		/// <summary>
		/// 格式化成Unix样式的时间
		/// </summary>
		public static string FormatUnixDate(DateTime dateTime)
		{
			string mouth = dateTime.ToString("MMM", new CultureInfo("en-US", false).DateTimeFormat);

			if(dateTime.Year == DateTime.Now.Year)
				return string.Format("{0} {1:00} {2:00}:{3:00}", mouth, dateTime.Day, dateTime.Hour, dateTime.Minute);
			else
				return string.Format("{0} {1:00} {2}", mouth, dateTime.Day, dateTime.Year);
		}

		/// <summary>
		/// 解析GTM时间yyyyMMddHHmmss
		/// </summary>
		public static DateTime ParseFtpDate(string dateStr)
		{
			var dateTime = DateTime.ParseExact(dateStr, FtpDataFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal);
			return dateTime;
		}

		/// <summary>
		/// 格式化GTM时间yyyyMMddHHmmss
		/// </summary>
		public static string FormatFtpDate(DateTime dateTime)
		{
			return dateTime.ToString(FtpDataFormats, DateTimeFormatInfo.InvariantInfo);
		}
	}
}
