/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2005-2008 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.ObjectModel;

namespace Zongsoft.Options
{
	public class OptionNodeCollection : Zongsoft.Collections.HierarchicalNodeCollection<OptionNode>
	{
		#region 构造函数
		internal OptionNodeCollection(OptionNode owner) : base(owner)
		{
		}
		#endregion

		#region 公共方法
		public OptionNode Add(string name, string title = null, string description = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			var node = new OptionNode(name, title, description);
			this.Add(node);
			return node;
		}

		public OptionNode Add(string name, IOptionProvider provider, string title = null, string description = null)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			OptionNode node = new OptionNode(name, title, description);

			if(provider != null)
				node.Option = new Option(node, provider);

			this.Add(node);
			return node;
		}
		#endregion
	}
}
