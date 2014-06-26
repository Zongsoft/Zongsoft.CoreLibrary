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
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zongsoft.Communication
{
	public class IPEndPointConverter : TypeConverter
	{
		#region 静态变量
		private static readonly Regex _regex = new Regex(@"(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(\s*[:#]\s*(?<port>\d{1,8}))?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
		#endregion

		#region 重写方法
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return Parse(value as string);
		}
		#endregion

		#region 静态方法
		public static IPEndPoint Parse(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				return null;

			var match = _regex.Match(text);

			if(match.Success)
			{
				IPAddress address;

				if(IPAddress.TryParse(match.Groups["ip"].Value, out address))
				{
					int port;
					int.TryParse(match.Groups["port"].Value, out port);

					return new IPEndPoint(address, port);
				}
			}

			return null;
		}
		#endregion
	}
}
