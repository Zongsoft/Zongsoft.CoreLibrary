/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Options.Configuration
{
	public class OptionConfigurationLoader : IOptionLoader
	{
		#region 成员字段
		private OptionNode _root;
		#endregion

		#region 构造函数
		public OptionConfigurationLoader(OptionNode rootNode)
		{
			if(rootNode == null)
				throw new ArgumentNullException("rootNode");

			_root = rootNode;
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
		#endregion

		#region 公共方法
		public virtual void Load(IOptionProvider provider)
		{
			this.LoadConfiguration(provider as OptionConfiguration);
		}

		public virtual void Unload(IOptionProvider provider)
		{
			this.UnloadConfiguration(provider as OptionConfiguration);
		}
		#endregion

		#region 保护方法
		public void LoadConfiguration(OptionConfiguration configuration)
		{
			if(configuration == null)
				return;

			foreach(var section in configuration.Sections)
			{
				//必须先确保选项节对应的空节点被添加
				var sectionNode = (OptionNode)_root.FindNode(section.Path, token =>
				{
					if(token.Current == null)
					{
						var parent = token.Parent as OptionNode;

						if(parent != null)
							return parent.Children.Add(token.Name);
					}

					return token.Current;
				});

				//在添加了选项上级空节点添加完成之后再添加选项元素的节点
				foreach(var elementName in section.Children.Keys)
				{
					var elementNode = sectionNode.Children[elementName];

					if(elementNode == null)
					{
						sectionNode.Children.Add(elementName, configuration);
					}
					else
					{
						if(elementNode.Option == null)
							elementNode.Option = new Option(elementNode, configuration);
					}
				}
			}
		}

		public void UnloadConfiguration(OptionConfiguration configuration)
		{
			if(configuration == null)
				return;

			foreach(var section in configuration.Sections)
			{
				foreach(var elementName in section.Children.Keys)
				{
					var node = _root.Find(section.Path, elementName);

					if(node != null)
					{
						var option = node.Option;

						if(option != null && option.Provider == configuration)
						{
							node.Option = null;

							var parent = node.Parent;
							if(parent != null)
								parent.Children.Remove(node);
						}
					}
				}
			}
		}
		#endregion
	}
}
