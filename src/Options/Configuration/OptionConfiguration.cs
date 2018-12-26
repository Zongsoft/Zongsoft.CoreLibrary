﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2018 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Xml;

namespace Zongsoft.Options.Configuration
{
	[OptionLoader(LoaderType = typeof(OptionConfigurationLoader))]
	public class OptionConfiguration : IOptionProvider
	{
		#region 常量定义
		private const string XML_ROOT_ELEMENT = "options";
		private const string XML_OPTION_ELEMENT = "option";
		private const string XML_DECLARATION_COLLECTION = "declarations";
		private const string XML_DECLARATION_ELEMENT = "declaration";
		private const string XML_PATH_ATTRIBUTE = "path";
		private const string XML_NAME_ATTRIBUTE = "name";
		private const string XML_TYPE_ATTRIBUTE = "type";
		#endregion

		#region 静态字段
		private static OptionConfigurationDeclarationCollection _declarations;
		#endregion

		#region 成员字段
		private string _filePath;
		private OptionConfigurationSectionCollection _sections;
		#endregion

		#region 构造函数
		public OptionConfiguration(string filePath = null)
		{
			if(filePath != null)
				_filePath = filePath.Trim();

			_sections = new OptionConfigurationSectionCollection();
		}
		#endregion

		#region 静态属性
		public static OptionConfigurationDeclarationCollection Declarations
		{
			get
			{
				if(_declarations == null)
				{
					var original = System.Threading.Interlocked.CompareExchange(ref _declarations, new OptionConfigurationDeclarationCollection(), null);

					if(original == null)
					{
						_declarations.Add("filter", typeof(FilterElement));
						_declarations.Add("filters", typeof(FilterElementCollection));

						_declarations.Add("setting", typeof(SettingElement));
						_declarations.Add("settings", typeof(SettingElementCollection));

						_declarations.Add("connectionString", typeof(ConnectionStringElement));
						_declarations.Add("connectionStrings", typeof(ConnectionStringElementCollection));
					}
				}

				return _declarations;
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前选项配置的完整文件路径。
		/// </summary>
		public string FilePath
		{
			get
			{
				return _filePath;
			}
		}

		public OptionConfigurationSectionCollection Sections
		{
			get
			{
				return _sections;
			}
		}
		#endregion

		#region 公共方法
		public OptionConfigurationSection GetSection(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException(nameof(path));

			path = path.Trim().Trim('/');

			if(!_sections.Contains(path))
				throw new OptionConfigurationException($"No found specified by '{path}' path of option section in the '{this.FilePath}' configuration file.");

			return _sections[path];
		}

		public object GetOptionValue(string text)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(nameof(text));

			//解析层次结构表达式
			var expression = Collections.HierarchicalExpression.Parse(text);

			string sectionPath, elementName;

			//拆解路径文本中的Section和Element部分
			OptionUtility.ResolveOptionPath(expression.Path, out sectionPath, out elementName);

			//获取指定路径的配置节
			var section = this.GetSection(sectionPath);

			if(section == null)
				throw new ArgumentException($"Not found '{sectionPath}' configuration option section in the '{this.FilePath}' file.");

			//获取指定配置节下面的配置元素
			var element = section.Children[elementName];

			if(element == null)
				throw new ArgumentException($"Not found '{elementName}' configuration element in the '{this.FilePath}' file.");

			if(expression.Members.Length > 0)
				return Reflection.MemberAccess.GetMemberValue<object>(element, expression.Members);

			return element;
		}

		public void SetOptionValue(string text, object value)
		{
			if(string.IsNullOrWhiteSpace(text))
				throw new ArgumentNullException(nameof(text));

			//解析层次结构表达式
			var expression = Collections.HierarchicalExpression.Parse(text);

			string sectionPath, elementName;

			//拆解路径文本中的Section和Element部分
			OptionUtility.ResolveOptionPath(expression.Path, out sectionPath, out elementName);

			//获取指定路径的配置节
			var section = this.GetSection(sectionPath);

			if(section == null)
				throw new ArgumentException($"Not found '{sectionPath}' configuration option section in the '{this.FilePath}' file.");

			//获取指定配置节下面的配置元素
			var element = section.Children[elementName];

			if(element == null)
				throw new ArgumentException($"Not found '{elementName}' configuration element in the '{this.FilePath}' file.");

			if(expression.Members.Length > 0)
				Reflection.MemberAccess.SetMemberValue(element, expression.Members, value);
		}
		#endregion

		#region 加载方法
		public static OptionConfiguration Load(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentNullException("filePath");

			using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Load(stream);
			}
		}

		public static OptionConfiguration Load(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			using(XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings()
			{
				CloseInput = true,
				IgnoreComments = true,
				IgnoreProcessingInstructions = true,
				IgnoreWhitespace = true,
			}))
			{
				reader.MoveToContent();

				if(reader.NodeType == XmlNodeType.Element && reader.Name != XML_ROOT_ELEMENT)
					throw new OptionConfigurationException();

				var configuration = new OptionConfiguration(stream is FileStream ? ((FileStream)stream).Name : string.Empty);

				while(reader.Read())
				{
					if(reader.NodeType != XmlNodeType.Element)
						continue;

					switch(reader.Name)
					{
						case XML_OPTION_ELEMENT:
							ResolveOptionElement(reader, configuration);
							break;
						case XML_DECLARATION_COLLECTION:
							ResolveDeclarationsElement(reader, configuration);
							break;
						default:
							throw new OptionConfigurationException(string.Format("Illegal '{0}' element in '{1}' option file.", reader.Name, configuration.FilePath));
					}
				}

				return configuration;
			}
		}

		private static void ResolveDeclarationsElement(XmlReader reader, OptionConfiguration configuration)
		{
			if(reader == null || reader.NodeType != XmlNodeType.Element || reader.Name != XML_DECLARATION_COLLECTION)
				return;

			var depth = reader.Depth;

			while(reader.Read() && reader.Depth > depth)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				if(reader.Name != XML_DECLARATION_ELEMENT)
					throw new OptionConfigurationException(string.Format("Invalid '{0}' declaration element in '{1}' option file.", reader.Name, configuration.FilePath));

				if(string.IsNullOrWhiteSpace(reader.GetAttribute(XML_NAME_ATTRIBUTE)))
					throw new OptionConfigurationException("The 'name' attribute of declaration element is empty.");

				if(string.IsNullOrWhiteSpace(reader.GetAttribute(XML_TYPE_ATTRIBUTE)))
					throw new OptionConfigurationException("The 'type' attribute of declaration element is empty.");

				var type = Type.GetType(reader.GetAttribute(XML_TYPE_ATTRIBUTE), false);

				if(type == null)
					throw new OptionConfigurationException(string.Format("Invalid '{0}' type of declaration.", reader.GetAttribute(XML_TYPE_ATTRIBUTE)));

				OptionConfiguration.Declarations.Add(reader.GetAttribute(XML_NAME_ATTRIBUTE), type);
			}
		}

		private static void ResolveOptionElement(XmlReader reader, OptionConfiguration configuration)
		{
			if(reader == null || reader.NodeType != XmlNodeType.Element || reader.Name != XML_OPTION_ELEMENT)
				return;

			if(string.IsNullOrWhiteSpace(reader.GetAttribute("path")))
				throw new OptionConfigurationException("The 'path' attribute of option element is empty or unspecified.");

			if(!configuration._sections.TryGet(reader.GetAttribute(XML_PATH_ATTRIBUTE), out var section))
				section = configuration._sections.Add(reader.GetAttribute(XML_PATH_ATTRIBUTE));

			while(reader.Read() && reader.Depth > 1)
			{
				if(reader.NodeType != XmlNodeType.Element)
					continue;

				var typeName = reader.GetAttribute(reader.Name + ".type");
				OptionConfigurationElement element = null;

				if(string.IsNullOrWhiteSpace(typeName))
				{
					element = OptionConfigurationUtility.GetGlobalElement(reader.Name);

					if(element == null)
						throw new OptionConfigurationException(string.Format("The '{0}' is a undeclared option element in the '{1}' file.", reader.Name, configuration._filePath));
				}
				else
				{
					Type type = Type.GetType(typeName, false, true);

					if(!typeof(OptionConfigurationElement).IsAssignableFrom(type))
						throw new OptionConfigurationException(string.Format("The '{0}' is not a OptionConfigurationElement type, in the '{0}' file.", typeName, configuration._filePath));

					element = (OptionConfigurationElement)Activator.CreateInstance(type);
				}

				if(element != null)
				{
					//保存当前元素的名称
					string elementName = reader.Name;
					//判断获取的配置项元素是否为配置项集合
					var collection = element as OptionConfigurationElementCollection;

					if(collection == null)
					{
						element.DeserializeElement(reader);
					}
					else
					{
						int depth = reader.Depth;

						//由于在OptionConfigurationSection中不允许存在默认集合，则此处始终应将读取器移动到其下的子元素的XML节点上
						while(reader.Read() && reader.Depth > depth)
						{
							if(reader.NodeType == XmlNodeType.Element)
							{
								collection.DeserializeElement(reader.ReadSubtree());
							}
						}
					}

					//将处理完毕的元素对象加入到当前选项节中
					section.Children.Add(elementName, element);
				}
			}
		}
		#endregion

		#region 保存方法
		public void Save()
		{
			if(string.IsNullOrWhiteSpace(_filePath))
				throw new InvalidOperationException();

			this.Save(_filePath);
		}

		public void Save(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentNullException("filePath");

			using(var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				this.Save(stream);
			}
		}

		public void Save(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			using(XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings()
			{
				Encoding = Encoding.UTF8,
				Indent = true,
				IndentChars = "\t",
				OmitXmlDeclaration = false,
				NewLineOnAttributes = false,
			}))
			{
				writer.WriteStartElement("options");

				foreach(var section in _sections)
				{
					//写入option开始元素
					writer.WriteStartElement("option");

					writer.WriteStartAttribute("path");
					writer.WriteString(string.IsNullOrEmpty(section.Path) ? "/" : section.Path);
					writer.WriteEndAttribute();

					//依次写入选项申明节点下的各级子元素
					foreach(var child in section.Children)
					{
						writer.WriteStartElement(child.Key);

						if(typeof(OptionConfigurationElement).IsAssignableFrom(child.Value.GetType()))
						{
							writer.WriteStartAttribute(child.Key + ".type");
							writer.WriteString(child.Value.GetType().AssemblyQualifiedName);
							writer.WriteEndAttribute();
						}

						child.Value.SerializeElement(writer);
						writer.WriteEndElement();
					}

					//关闭option元素，即写入</option>元素
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}
		}
		#endregion
	}
}
