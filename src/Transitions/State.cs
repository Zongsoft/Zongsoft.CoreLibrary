/*
 * Authors:
 *   钟峰(Popeye Zhong) <9555843@qq.com>
 *
 * Copyright (C) 2017 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Transitions
{
	public abstract class State
	{
		#region 成员字段
		private DateTime? _timestamp;
		private string _description;
		private IStateDiagram _diagram;
		#endregion

		#region 构造函数
		protected State(IStateDiagram diagram)
		{
			if(diagram == null)
				throw new ArgumentNullException(nameof(diagram));

			_diagram = diagram;
			_timestamp = DateTime.Now;
			_description = null;
		}

		protected State(IStateDiagram diagram, string description)
		{
			if(diagram == null)
				throw new ArgumentNullException(nameof(diagram));

			_diagram = diagram;
			_timestamp = DateTime.Now;
			_description = description;
		}

		protected State(IStateDiagram diagram, DateTime? timestamp, string description = null)
		{
			if(diagram == null)
				throw new ArgumentNullException(nameof(diagram));

			_diagram = diagram;
			_timestamp = timestamp;
			_description = description;
		}
		#endregion

		#region 公共属性
		public IStateDiagram Diagram
		{
			get
			{
				return _diagram;
			}
			protected set
			{
				if(value == null)
					throw new ArgumentNullException();

				_diagram = value;
			}
		}

		public DateTime? Timestamp
		{
			get
			{
				return _timestamp;
			}
			set
			{
				_timestamp = value;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
			}
		}
		#endregion

		#region 抽象成员
		internal protected abstract bool Match(State state);
		#endregion
	}
}
