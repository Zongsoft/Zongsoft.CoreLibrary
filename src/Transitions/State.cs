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
	public abstract class State<T> where T : struct
	{
		#region 成员字段
		private T _value;
		private DateTime? _timestamp;
		private string _description;
		private StateDiagramBase<T> _diagram;
		#endregion

		#region 构造函数
		public State(T value)
		{
			this.Value = value;
			this.Timestamp = DateTime.Now;
			this.Description = null;
		}

		public State(T value, string description)
		{
			this.Value = value;
			this.Timestamp = DateTime.Now;
			this.Description = description;
		}

		public State(T value, DateTime? timestamp, string description = null)
		{
			this.Value = value;
			this.Timestamp = timestamp;
			this.Description = description;
		}
		#endregion

		#region 公共属性
		public T Value
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

		public StateDiagramBase<T> Diagram
		{
			get
			{
				return _diagram;
			}
			internal set
			{
				_diagram = value;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(string.IsNullOrWhiteSpace(_description))
			{
				if(_timestamp.HasValue)
					return string.Format("{0}@{1}", _value.ToString(), _timestamp.ToString());
				else
					return _value.ToString();
			}
			else
			{
				if(_timestamp.HasValue)
					return string.Format("{0}@{1} \"{2}\"", _value.ToString(), _timestamp.ToString(), _description);
				else
					return string.Format("{0} \"{1}\"", _value.ToString(), _description);
			}
		}

		public override bool Equals(object obj)
		{
			if(obj.GetType() != this.GetType())
				return false;

			return _value.Equals(((State<T>)obj).Value);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}
		#endregion

		#region 抽象方法
		internal protected abstract bool Match(State<T> state);
		#endregion
	}
}
