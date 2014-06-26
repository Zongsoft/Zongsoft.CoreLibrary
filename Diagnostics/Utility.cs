/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Diagnostics
{
	internal static class Utility
	{
		[Obsolete]
		public static void WriteObject(TextWriter output, object obj)
		{
			if(obj == null)
				return;

			output.WriteLine("{0}", obj.GetType().FullName);

			if(obj is string)
			{
				output.Write(obj);
				return;
			}

			if(obj is byte[])
			{
				output.Write(Zongsoft.Common.Convert.ToHexString((byte[])obj, ' '));
				return;
			}

			if(obj is IEnumerable)
			{
				var enumerator = ((IEnumerable)obj).GetEnumerator();
				int index = 0, count = 0;

				if(obj is ICollection)
					count = ((ICollection)obj).Count;

				while(enumerator.MoveNext())
				{
					output.Write("[{0}] ", index++);

					WriteObject(output, enumerator.Current);

					if(count < 1 || index < count)
						output.WriteLine();
				}

				return;
			}

			output.Write(obj.ToString());
		}
	}
}
