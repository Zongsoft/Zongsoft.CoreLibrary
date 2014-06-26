/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2011 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Text;
using System.Text.RegularExpressions;

namespace Zongsoft.IO
{
	/// <summary>
	/// 提供有关路径和文件操作功能的静态类。
	/// </summary>
	public static class PathUtility
	{
		public static string GetCurrentFilePathWithSerialNo(string filePath, Predicate<string> predicate)
		{
			string filePathOfCurrent = filePath;
			string filePathOfMaxSerialNo = GetFilePathOfMaxSerialNo(filePath);

			if(!string.IsNullOrEmpty(filePathOfMaxSerialNo))
				filePathOfCurrent = filePathOfMaxSerialNo;

			if(predicate.Invoke(filePathOfCurrent))
				filePath = GetFilePathOfNextSerialNo(filePath);
			else
				filePath = filePathOfCurrent;

			return filePath;
		}

		public static string GetFilePathOfMaxSerialNo(string filePath)
		{
			int? temp;
			return GetFilePathOfMaxSerialNo(filePath, out temp);
		}

		public static string GetFilePathOfMaxSerialNo(string filePath, out int? maxSerialNo)
		{
			if(string.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("filePath");

			string directoryPath = Path.GetDirectoryName(filePath);
			string fileName = GetFileNameOfMaxSerialNo(directoryPath, Path.GetFileName(filePath), out maxSerialNo);

			if(string.IsNullOrEmpty(fileName))
				return null;

			return Path.Combine(directoryPath, fileName);
		}

		public static string GetFileNameOfMaxSerialNo(string directoryPath, string fileName)
		{
			int? temp;
			return GetFileNameOfMaxSerialNo(directoryPath, fileName, out temp);
		}

		public static string GetFileNameOfMaxSerialNo(string directoryPath, string fileName, out int? maxSerialNo)
		{
			if(string.IsNullOrEmpty(directoryPath))
				throw new ArgumentNullException("directoryPath");

			if(string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");

			maxSerialNo = null;
			string fileNameOfMaxSerialNo = null;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			string extensionName = Path.GetExtension(fileName);

			string[] sameFileNames = Directory.GetFiles(directoryPath, fileNameWithoutExtension + "-*" + extensionName);

			foreach(string sameFileName in sameFileNames)
			{
				Match match = Regex.Match(sameFileName, string.Format(@"{0}-(?'no'\d+)\{1}\b", fileNameWithoutExtension, extensionName));

				if(match.Success && match.Groups.Count > 1 && match.Groups["no"].Success)
				{
					int fileNo = int.Parse(match.Groups["no"].Value);

					if(fileNameOfMaxSerialNo != null)
					{
						if(fileNo > maxSerialNo)
						{
							maxSerialNo = fileNo;
							fileNameOfMaxSerialNo = sameFileName;
						}
					}
					else
					{
						maxSerialNo = int.Parse(match.Groups["no"].Value);
						fileNameOfMaxSerialNo = sameFileName;
					}
				}
			}

			return fileNameOfMaxSerialNo;
		}

		public static string GetFilePathOfNextSerialNo(string filePath)
		{
			return GetFilePathOfNextSerialNo(filePath, 1, 1);
		}

		public static string GetFilePathOfNextSerialNo(string filePath, int seed, int step)
		{
			string directoryPath = Path.GetDirectoryName(filePath);
			string fileName = GetFileNameOfNextSerialNo(directoryPath, Path.GetFileName(filePath));

			return Path.Combine(directoryPath, fileName);
		}

		public static string GetFileNameOfNextSerialNo(string directoryPath, string fileName)
		{
			return GetFileNameOfNextSerialNo(directoryPath, fileName, 1, 1);
		}

		public static string GetFileNameOfNextSerialNo(string directoryPath, string fileName, int seed, int step)
		{
			int? maxSerialNo;
			GetFileNameOfMaxSerialNo(directoryPath, fileName, out maxSerialNo);

			if(maxSerialNo.HasValue)
				return string.Format("{0}-{2}{1}", Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName), maxSerialNo + step);
			else
				return string.Format("{0}-{2}{1}", Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName), seed);
		}
	}
}
