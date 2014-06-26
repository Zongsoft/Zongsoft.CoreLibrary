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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Options
{
	public class OptionManager : IOptionProvider
	{
		#region 私有枚举
		private enum InvokeMethod
		{
			Apply,
			Reset,
		}
		#endregion

		#region 成员变量
		private OptionNode _root;
		#endregion

		#region 构造函数
		public OptionManager()
		{
			_root = new OptionNode();
		}
		#endregion

		#region 公共属性
		public OptionNode RootNode
		{
			get
			{
				return _root;
			}
		}

		public ISettingProvider Settings
		{
			get
			{
				var node = this.Find("/settings");

				if(node == null || node.Option == null)
					return null;

				return node.Option.OptionObject as ISettingProvider;
			}
		}
		#endregion

		#region 公共方法
		public void Apply()
		{
			this.Invoke(_root, InvokeMethod.Apply);
		}

		public void Reset()
		{
			this.Invoke(_root, InvokeMethod.Reset);
		}

		public object GetOptionObject(string path)
		{
			var node = this.Find(path);

			if(node != null && node.Option != null)
				return node.Option.OptionObject;

			return null;
		}

		public void SetOptionObject(string path, object optionObject)
		{
			var node = this.Find(path);

			if(node != null && node.Option != null && node.Option.Provider != null)
				node.Option.Provider.SetOptionObject(path, optionObject);
		}

		public OptionNode Find(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return null;

			var parts = path.Split('/');

			if(parts == null || parts.Length < 1)
				return null;

			OptionNode current = _root;

			foreach(var part in parts)
			{
				if(string.IsNullOrWhiteSpace(part))
					continue;

				current = current.Children[part];

				if(current == null)
					return null;
			}

			return current;
		}
		#endregion

		#region 私有方法
		private void Invoke(OptionNode node, InvokeMethod method)
		{
			if(node == null)
				return;

			if(node.Option != null)
			{
				switch(method)
				{
					case InvokeMethod.Apply:
						node.Option.Apply();
						break;
					case InvokeMethod.Reset:
						node.Option.Reset();
						break;
				}
			}

			foreach(var child in node.Children)
				this.Invoke(child, method);
		}
		#endregion
	}
}
