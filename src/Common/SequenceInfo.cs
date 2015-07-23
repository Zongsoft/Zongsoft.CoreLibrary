/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Common
{
	[Serializable]
	public class SequenceInfo : MarshalByRefObject
	{
		#region 成员字段
		private string _name;
		private int _interval;
		private long _value;
		private string _formatString;
		#endregion

		#region 构造函数
		public SequenceInfo(string name) : this(name, 0, 1, null)
		{
		}

		public SequenceInfo(string name, int value, int interval, string formatString)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			_name = name.Trim();
			_value = value;
			_interval = interval;
			_formatString = formatString;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置序列号名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// 获取或设置序列号的递增步长值。
		/// </summary>
		public int Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				_interval = value;
			}
		}

		/// <summary>
		/// 获取或设置序列号的当前数值。
		/// </summary>
		public long Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// 获取或设置序列号的格式化字符串。
		/// </summary>
		/// <remarks>
		///		格式化字符串的变量为花括号框起来的序列号名，譬如下列这些格式化字符串：
		///		<list type="bullet">
		///			<item>
		///				<term>No:{#}</term>
		///				<description>表示由“No:”这个常量接上当前序列号的数值，当前序列号数值的变量即为“{#}”。</description>
		///			</item>
		///			<item>
		///				<term>No:{#:00000}</term>
		///				<description>表示由“No:”这个常量接上当前序列号的数值，并且格式化后的数字文本至少为5位，不足5位的在前面补零。里面的“00000”表示格式修饰，详细说明请参考<seealso cref="System.String.Format(System.IFormatProvider, string, object[])"/>方法。</description>
		///			</item>
		///			<item>
		///				<term>No:{#}-{0:00}-{1:G}-{Zongsoft.CRM.Customer.Id}</term>
		///				<description>表示由“No:”这个常量接上当前序列号的数值，再接上“-{0}-{1}-”常量(有效的变量名必须以字母和下划线打头，否则视为常量)，再接上名为“Zongsoft.CRM.Customer.Id”的这个序列号数值。</description>
		///			</item>
		///		</list>
		/// </remarks>
		public string FormatString
		{
			get
			{
				return _formatString;
			}
			set
			{
				_formatString = value;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			return string.Format("{0} ({1}, {2}, {3})", _name, _value, _interval, _formatString);
		}
		#endregion
	}
}
