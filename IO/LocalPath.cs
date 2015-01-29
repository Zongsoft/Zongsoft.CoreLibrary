/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text.RegularExpressions;

namespace Zongsoft.IO
{
	internal static class LocalPath
	{
		private static readonly Regex _winPathRegex = new Regex(@"^(\/?(?<drive>[^\/\\\*\?:]+))(?<path>(/[^\/\\\*\?:]+)*\/?)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

		public static string ToLocalPath(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException("path");

			path = path.Trim();

			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return path;
			}

			var match = _winPathRegex.Match(path);

			if(!match.Success)
				throw new PathException(string.Format("Invalid format of the '{0}' path.", path));

			string driveName = match.Groups["drive"].Value.Trim();

			if(string.IsNullOrWhiteSpace(driveName))
				throw new PathException(string.Format("The '{0}' path not cantians drive.", path));

			if(driveName.Length > 1)
			{
				var drives = System.IO.DriveInfo.GetDrives();

				foreach(var drive in drives)
				{
					if(string.Equals(drive.VolumeLabel, driveName, StringComparison.OrdinalIgnoreCase))
					{
						driveName = drive.Name;
						break;
					}
				}
			}

			return driveName[0] + ":" + match.Groups["path"].Value.Replace('/', '\\');
		}
	}
}
