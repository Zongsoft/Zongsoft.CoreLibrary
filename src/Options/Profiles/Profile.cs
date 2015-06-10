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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Zongsoft.Options.Profiles
{
	/// <summary>
	/// 提供了对 Microsoft 的INI文件格式的各项操作。
	/// </summary>
	/// <remarks>
	///		<para>INI文件就是简单的文本文件，只不过这种文本文件要遵循一定的INI文件格式，其扩展名通常为“.ini”、“.cfg”、“.conf”。</para>
	///		<para>INI文件中的每一行文本为一个元素单位，其类型分别为 Section(节)、Entry/Parameter(条目/参数)、Comment(注释)。</para>
	///		<para>Entry: INI所包含的最基本的“元素”就是 Entry/Parameter，每一个“条目”都由一个名称和一个值组成(值可选)，名称与值由等号“=”分隔，名称在等号的左边；可以省略值的部分。譬如：name=value 或者只有名称的“条目”：name。注意：在同一个设置节中，各个条目的名称必须唯一。</para>
	///		<para>Section: 所有的“条目”都是以“节”为单位结合在一起的。“节”名字都被方括号包围着。在“节”声明后的所有“条目”都是属于该“节”。对于一个“节”没有明显的结束标志符，一个“节”的开始就是上一个“节”的结束。</para>
	///		<para>注意：节是支持嵌套的，如果在中括号里面以空格或制表符(Tab)分隔来表示节的嵌套关系。</para>
	///		<para>Comment: 在INI文件中注释语句是以分号“;”或者“#”开始的。所有的注释语句不管多长都是独占一行直到结束的，在注释符和行结束符之间的所有内容都是被忽略的。</para>
	/// </remarks>
	public class Profile : MarshalByRefObject, IOptionProvider, Zongsoft.Runtime.Serialization.ISerializable
	{
		#region 枚举定义
		private enum LineType
		{
			Empty,
			Entry,
			Section,
			Comment,
		}
		#endregion

		#region 成员字段
		private string _filePath;
		private ProfileItemCollection _items;
		private ProfileCommentCollection _comments;
		private ProfileSectionCollection _sections;
		#endregion

		#region 构造函数
		public Profile(string filePath = null)
		{
			if(filePath != null)
				_filePath = filePath.Trim();

			_items = new ProfileItemCollection(this);
		}
		#endregion

		#region 公共属性
		public string FilePath
		{
			get
			{
				return _filePath;
			}
		}

		public ProfileItemCollection Items
		{
			get
			{
				return _items;
			}
		}

		public ProfileCommentCollection Comments
		{
			get
			{
				if(_comments == null)
					System.Threading.Interlocked.CompareExchange(ref _comments, new ProfileCommentCollection(_items), null);

				return _comments;
			}
		}

		public ProfileSectionCollection Sections
		{
			get
			{
				if(_sections == null)
					System.Threading.Interlocked.CompareExchange(ref _sections, new ProfileSectionCollection(_items), null);

				return _sections;
			}
		}
		#endregion

		#region 加载方法
		public static Profile Load(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentNullException("filePath");

			using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Load(stream);
			}
		}

		public static Profile Load(Stream stream, Encoding encoding = null)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			Profile profile = new Profile(stream is FileStream ? ((FileStream)stream).Name : string.Empty);
			string text, content;
			ProfileSection section = null;

			using(var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
			{
				int lineNumber = 0;

				while((text = reader.ReadLine()) != null)
				{
					//解析读取到的行文本
					switch(ParseLine(text, out content))
					{
						case LineType.Section:
							var parts = content.Split(' ', '\t');
							var sections = profile.Sections;

							foreach(string part in parts)
							{
								section = sections[part];

								if(section == null)
									section = sections.Add(part, lineNumber);

								sections = section.Sections;
							}

							break;
						case LineType.Entry:
							//if(section == null)
							//	throw new ProfileException("Invalid format of the profile.");

							var index = content.IndexOf('=');

							if(section == null)
							{
								if(index < 0)
									profile.Items.Add(new ProfileEntry(lineNumber, content));
								else
									profile.Items.Add(new ProfileEntry(lineNumber, content.Substring(0, index), content.Substring(index + 1)));
							}
							else
							{
								if(index < 0)
									section.Entries.Add(lineNumber, content);
								else
									section.Entries.Add(lineNumber, content.Substring(0, index), content.Substring(index + 1));
							}

							break;
						case LineType.Comment:
							if(section == null)
								profile._items.Add(new ProfileComment(content, lineNumber));
							else
								section.Items.Add(new ProfileComment(content, lineNumber));
							break;
					}

					//递增行号
					lineNumber++;
				}
			}

			return profile;
		}
		#endregion

		#region 保存方法
		public void Save()
		{
			if(string.IsNullOrWhiteSpace(_filePath))
				throw new InvalidOperationException();

			this.Save(_filePath);
		}

		public void Save(string filePath, Encoding encoding = null)
		{
			filePath = filePath ?? _filePath;

			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException();

			using(var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				this.Save(stream, encoding);
			}
		}

		public void Save(Stream stream, Encoding encoding = null)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			using(var writer = new StreamWriter(stream, encoding ?? Encoding.UTF8))
			{
				this.Save(writer);
			}
		}

		public void Save(TextWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");

			foreach(var item in _items)
			{
				switch(item.ItemType)
				{
					case ProfileItemType.Comment:
						foreach(var line in ((ProfileComment)item).Lines)
						{
							writer.WriteLine("#" + line);
						}
						break;
					case ProfileItemType.Section:
						WriteSection(writer, (ProfileSection)item);
						break;
				}
			}
		}
		#endregion

		#region 接口实现
		/// <summary>
		/// 获取指定路径的配置数据。
		/// </summary>
		/// <param name="path">指定的配置项路径，路径是以“/”斜杠分隔的文本。</param>
		/// <returns>如果找到则返回配置结果，否则返回空(null)。</returns>
		/// <remarks>
		///		<para>如果<paramref name="path"/>参数指定的配置路径以“/”斜杠结尾则将返回指定配置段的所有条目集；否则返回指定的配置条目的值。</para>
		/// </remarks>
		public object GetOptionObject(string path)
		{
			string name;
			bool isSectionPath;
			ProfileSection section;

			if(!this.ParsePath(path, false, out section, out name, out isSectionPath))
				return null;

			if(section == null)
			{
				section = this.Sections[name];

				if(section == null)
					return null;
				else
					return section.Entries;
			}

			if(isSectionPath)
			{
				section = section.Sections[name];

				if(section == null)
					return null;
				else
					return section.Entries;
			}

			var entry = section.Entries[name];

			if(entry == null)
				return null;
			else
				return entry.Value;
		}

		public void SetOptionObject(string path, object optionObject)
		{
			string name;
			bool isSectionPath;
			ProfileSection section;

			if(!this.ParsePath(path, true, out section, out name, out isSectionPath))
				return;

			if(section == null)
			{
				section = this.Sections[name] ?? this.Sections.Add(name);
				this.UpdateEntries(section, optionObject);
			}
			else
			{
				if(isSectionPath)
				{
					section = section.Sections[name] ?? section.Sections.Add(name);
					this.UpdateEntries(section, optionObject);
				}
				else
				{
					section.SetEntryValue(name, Zongsoft.Common.Convert.ConvertValue<string>(optionObject));
				}
			}
		}

		void Zongsoft.Runtime.Serialization.ISerializable.Serialize(Stream serializationStream)
		{
			if(serializationStream == null)
				throw new ArgumentNullException("serializationStream");

			this.Save(serializationStream);
		}
		#endregion

		#region 私有方法
		private bool ParsePath(string path, bool createSectionOnNotExists, out ProfileSection section, out string name, out bool isSectionPath)
		{
			section = null;
			name = null;
			isSectionPath = false;

			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentNullException("path");

			var parts = path.Split('/');
			int index = -1;

			for(int i = parts.Length - 1; i >= 0; i--)
			{
				if(!string.IsNullOrWhiteSpace(parts[i]))
				{
					index = i;
					break;
				}
			}

			if(index < 0)
				return false;

			name = parts[index];
			isSectionPath = string.IsNullOrWhiteSpace(parts[parts.Length - 1]);

			var sections = this.Sections;

			for(int i = 0; i < index; i++)
			{
				if(string.IsNullOrWhiteSpace(parts[i]))
					continue;

				section = sections[parts[i]];

				if(section == null)
				{
					if(!createSectionOnNotExists)
						return false;

					section = sections.Add(parts[i]);
				}

				sections = section.Sections;
			}

			return true;
		}

		private void UpdateEntries(ProfileSection section, object value)
		{
			if(section == null || value == null)
				return;

			var entries = value as IEnumerable<ProfileEntry>;

			if(entries != null)
			{
				foreach(var entry in entries)
				{
					section.SetEntryValue(entry.Name, entry.Value);
				}

				return;
			}

			var dictionary = value as IDictionary;

			if(dictionary != null)
			{
				foreach(DictionaryEntry entry in dictionary)
				{
					section.SetEntryValue(
						Zongsoft.Common.Convert.ConvertValue<string>(entry.Key),
						Zongsoft.Common.Convert.ConvertValue<string>(entry.Value));
				}

				return;
			}

			if(Zongsoft.Common.TypeExtension.IsAssignableFrom(typeof(IDictionary<,>), value.GetType()))
			{
				foreach(var entry in (IEnumerable)value)
				{
					var entryKey = entry.GetType().GetProperty("Key", BindingFlags.Public | BindingFlags.Instance).GetValue(entry);
					var entryValue = entry.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).GetValue(entry);

					section.SetEntryValue(
						Zongsoft.Common.Convert.ConvertValue<string>(entryKey),
						Zongsoft.Common.Convert.ConvertValue<string>(entryValue));
				}
			}
		}

		private static void WriteSection(TextWriter writer, ProfileSection section)
		{
			if(section == null)
				return;

			var sections = new List<ProfileSection>();

			if(section.Entries.Count > 0 || section.Comments.Count > 0)
			{
				writer.WriteLine();
				writer.WriteLine("[{0}]", section.FullName);
			}

			foreach(var item in section.Items)
			{
				switch(item.ItemType)
				{
					case ProfileItemType.Section:
						sections.Add((ProfileSection)item);
						break;
					case ProfileItemType.Entry:
						var entry = (ProfileEntry)item;

						if(string.IsNullOrWhiteSpace(entry.Value))
							writer.WriteLine(entry.Name);
						else
							writer.WriteLine(entry.Name + "=" + entry.Value);

						break;
					case ProfileItemType.Comment:
						foreach(var line in ((ProfileComment)item).Lines)
						{
							writer.WriteLine("#" + line);
						}
						break;
				}
			}

			if(sections.Count > 0)
			{
				foreach(var child in sections)
					WriteSection(writer, child);
			}
		}

		private static LineType ParseLine(string text, out string result)
		{
			result = null;

			if(string.IsNullOrWhiteSpace(text))
				return LineType.Empty;

			text = text.Trim();

			if(text[0] == ';' || text[0] == '#')
			{
				result = text.Substring(1);
				return LineType.Comment;
			}

			if(text[0] == '[' && text[text.Length - 1] == ']')
			{
				if(text.Length < 3)
					throw new ProfileException("Invalid format.");

				result = text.Substring(1, text.Length - 2);
				return LineType.Section;
			}

			if(text[0] == '=')
				throw new ProfileException("Invalid format.");

			result = text;
			return LineType.Entry;
		}
		#endregion
	}
}
