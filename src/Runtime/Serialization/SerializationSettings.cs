/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Runtime.Serialization
{
	public class SerializationSettings : Zongsoft.ComponentModel.NotifyObject
	{
		#region 成员变量
		private int _maximumDepth;
		private SerializationMembers _serializationMembers;
		#endregion

		#region 构造函数
		public SerializationSettings()
		{
			_maximumDepth = -1;
			_serializationMembers = Serialization.SerializationMembers.All;
		}

		public SerializationSettings(int maximumDepth, Serialization.SerializationMembers serializationMembers)
		{
			this.MaximumDepth = maximumDepth;
			this.SerializationMembers = serializationMembers;
		}
		#endregion

		#region 公共属性
		public int MaximumDepth
		{
			get
			{
				return _maximumDepth;
			}
			set
			{
				var newValue = Math.Max(-1, value);
				var changed = _maximumDepth != newValue;

				_maximumDepth = newValue;

				if(changed)
					this.OnPropertyChanged(() => this.MaximumDepth);
			}
		}

		public SerializationMembers SerializationMembers
		{
			get
			{
				return _serializationMembers;
			}
			set
			{
				if(_serializationMembers != value)
				{
					_serializationMembers = value;
					this.OnPropertyChanged(() => this.SerializationMembers);
				}
			}
		}
		#endregion
	}
}
