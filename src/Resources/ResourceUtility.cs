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
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

namespace Zongsoft.Resources
{
	/// <summary>
	/// 提供对资源文件的读取功能。
	/// </summary>
	/// <remarks>
	///		<para>
	///		该类的<see cref="GetString(string)"/>、<see cref="GetObject(string)"/>和<see cref="GetStream(string)"/>方法均支持如下表达式：
	///		${ResourceEntryName}或者${ResourceEntryName@BaseName, AssemblyName}， 其中BaseName和AssemblyName均可省略。
	///		</para>
	///		<para>GetString(string text, ...)方法的text参数如果不匹配上面的表达式语法则返回text参数值。</para>
	///		<para>如果GetXXXXX(string name, stirng baseName, ...)方法中baseName参数为空或空白字符串，或者表达式文本参数中没有包含ResourceBaseName部分，则资源根名(BaseName)默认为assembly程序集的名称。</para>
	/// </remarks>
	public static class ResourceUtility
	{
		#region 静态常量
		private static readonly string[] EmptyStringArray = new string[0];
		#endregion

		#region 私有变量
		private static Regex _regex = new Regex(@"(?<=\$\{)?\s*(?<name>[a-zA-Z0-9\._-]+)\s*([@]\s*(?<base>[a-zA-Z0-9\._-]+))?\s*(?=\})?", (RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));
		private static Dictionary<Assembly, ResourceManagerCollection> _resources = new Dictionary<Assembly, ResourceManagerCollection>();
		#endregion

		#region 解析格式
		public static bool TryParseResourceText(string text, out string name, out string baseName)
		{
			name = string.Empty;
			baseName = string.Empty;

			if(string.IsNullOrWhiteSpace(text))
				return false;

			var match = _regex.Match(text);

			if(match.Success)
			{
				name = match.Groups["name"].Value;
				baseName = match.Groups["base"].Value;
			}

			return match.Success;
		}
		#endregion

		#region 获取基名
		/// <summary>
		/// 获取指定资源文件的根名及其所有上级根名的数组。
		/// </summary>
		/// <param name="baseName">要查找的资源文件的根名，如果为空(null)或全空白字符串则返回空数组(长度为零)。</param>
		/// <returns>返回的资源文件的根名及其所有上级根名的字符串数组。</returns>
		/// <remarks>
		///		<para>
		///		譬如<paramref name="baseName"/>参数为：Zongsoft.EAS.Web，则返回的数组元素分别为：
		///		<list type="number">
		///			<item>
		///				<term>Zongsoft.EAS.Web</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft.EAS.Web.Resources</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft.EAS.Web.Properties.Resources</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft.EAS</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft.EAS.Resources</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft.EAS.Properties.Resources</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft.Resources</term>
		///			</item>
		///			<item>
		///				<term>Zongsoft.Properties.Resources</term>
		///			</item>
		///		</list>
		///		</para>
		/// </remarks>
		public static string[] GetBaseNames(string baseName)
		{
			return GetBaseNames(baseName, null);
		}

		/// <summary>
		/// 获取指定资源文件的根名及其所有上级根名的数组，并在指定的程序集进行有效性过滤。
		/// </summary>
		/// <param name="baseName">要查找的资源文件的根名。</param>
		/// <param name="assembly">对待返回的所有资源文件的根名进行有效性过滤的程序集，如果为空(null)则不进行有效性过滤。</param>
		/// <returns>
		///		<para>返回的在指定程序集中存在的资源文件的根名及其所有上级根名的字符串数组。</para>
		///		<para>如果<paramref name="baseName"/>参数为空(null)或全空白字符串但是<paramref name="assembly"/>参数为不为空(null)，则当<paramref name="baseName"/>参数值为<paramref name="assembly"/>参数指定的程序集名称。</para>
		///		<para>如果<paramref name="baseName"/>参数为空(null)或全空白字符串并且<paramref name="assembly"/>参数为空(null)，则返回空数组(长度为零)。</para>
		/// </returns>
		/// <remarks>
		///		<para>
		///		如果<paramref name="assembly"/>参数为空(null)，该方法执行效果与<see cref="ResourceUtility.GetBaseNames(string)"/>方法功能一样。
		///		</para>
		///		<para>
		///		如果<paramref name="assembly"/>参数不为空，则对方法返回的每个资源文件根名进行有效性过滤，即查看待返回的每个资源文件根名在<paramref name="assembly"/>参数指定的程序集中查看是否存在，如果不存在则不会包含在返回的数组中。
		///		</para>
		/// </remarks>
		public static string[] GetBaseNames(string baseName, Assembly assembly)
		{
			if(string.IsNullOrWhiteSpace(baseName))
			{
				if(assembly == null)
					return EmptyStringArray;

				baseName = assembly.GetName().Name;
			}
			else
			{
				baseName = baseName.Trim('.').Trim();
			}

			if(baseName.Length < 1)
				return EmptyStringArray;

			if(baseName.EndsWith(".Properties.Resources", StringComparison.OrdinalIgnoreCase))
				baseName = baseName.Substring(0, baseName.Length - ".Properties.Resources".Length);
			else if(baseName.EndsWith(".Resources", StringComparison.OrdinalIgnoreCase))
				baseName = baseName.Substring(0, baseName.Length - ".Resources".Length);

			List<string> baseNames = new List<string>();
			var parts = baseName.Split('.');
			var names = assembly == null ? null : assembly.GetManifestResourceNames().Select(name => name.EndsWith(".resources") ? name.Substring(0, name.Length - 10) : name);

			for(int i = parts.Length; i > 0; i--)
			{
				var name = string.Join(".", parts, 0, i);

				if(names == null || names.Contains(name, StringComparer.OrdinalIgnoreCase))
					baseNames.Add(string.Join(".", parts, 0, i));

				if(names == null || names.Contains(name + ".Resources", StringComparer.OrdinalIgnoreCase))
					baseNames.Add(name + ".Resources");

				if(names == null || names.Contains(name + ".Properties.Resources", StringComparer.OrdinalIgnoreCase))
					baseNames.Add(name + ".Properties.Resources");
			}

			return baseNames.ToArray();
		}
		#endregion

		#region 公共方法
		public static string GetString(string text)
		{
			return GetString(text, Assembly.GetCallingAssembly(), null);
		}

		public static string GetString(string text, params object[] args)
		{
			return GetString(text, Assembly.GetCallingAssembly(), args);
		}

		public static string GetString(string text, Assembly assembly)
		{
			return GetString(text, assembly, null);
		}

		public static string GetString(string text, Assembly assembly, params object[] args)
		{
			if(string.IsNullOrWhiteSpace(text))
				return text;

			string name, baseName, result;

			if(TryParseResourceText(text, out name, out baseName))
				result = (string)GetResourceValue(name, baseName, assembly, resourceManager => resourceManager.GetString(name));
			else
				result = (string)GetResourceValue(text, baseName, assembly, resourceManager => resourceManager.GetString(text));

			if(result == null)
				return text;

			if(args != null && args.Length > 0 && (!string.IsNullOrWhiteSpace(result)))
				return string.Format(result, args);

			return result;
		}

		public static bool TryGetString(string key, out string result)
		{
			return TryGetString(key, Assembly.GetCallingAssembly(), out result);
		}

		public static bool TryGetString(string key, Assembly assembly, out string result)
		{
			result = null;

			if(string.IsNullOrWhiteSpace(key))
				return false;

			string name, baseName;

			if(TryParseResourceText(key, out name, out baseName))
				result = (string)GetResourceValue(name, baseName, assembly, resourceManager => resourceManager.GetString(name));
			else
				result = (string)GetResourceValue(key, baseName, assembly, resourceManager => resourceManager.GetString(key));

			return result != null;
		}

		public static object GetObject(string text)
		{
			return GetObject(text, Assembly.GetCallingAssembly());
		}

		public static object GetObject(string text, Assembly assembly)
		{
			string name, baseName;

			if(TryParseResourceText(text, out name, out baseName))
				return GetResourceValue(name, baseName, assembly, resourceManager => resourceManager.GetObject(name));
			else
				return GetResourceValue(text, baseName, assembly, resourceManager => resourceManager.GetObject(text));
		}

		public static Stream GetStream(string text)
		{
			return GetStream(text, Assembly.GetCallingAssembly());
		}

		public static Stream GetStream(string text, Assembly assembly)
		{
			string name, baseName;

			if(TryParseResourceText(text, out name, out baseName))
				return (Stream)GetResourceValue(name, baseName, assembly, resourceManager => resourceManager.GetStream(name));
			else
				return (Stream)GetResourceValue(text, baseName, assembly, resourceManager => resourceManager.GetStream(text));
		}
		#endregion

		#region 私有方法
		private static object GetResourceValue(string name, string baseName, Assembly assembly, Func<ResourceManager, object> callback)
		{
			if(assembly == null)
				throw new ArgumentNullException("assembly");

			if(callback == null)
				throw new ArgumentNullException("callback");

			if(string.IsNullOrWhiteSpace(name))
				return null;

			var managerBaseNames = GetBaseNames(baseName, assembly);

			if(managerBaseNames == null || managerBaseNames.Length < 1)
				return null;

			//此处必须使用同步锁，因为下面的EnsureResourceManager方法对集合的操作不是异步安全的
			lock(((ICollection)_resources).SyncRoot)
			{
				foreach(var managerBaseName in managerBaseNames)
				{
					var resourceManager = EnsureResourceManager(managerBaseName, assembly);

					if(resourceManager != null)
					{
						var value = callback(resourceManager);

						if(value != null)
							return value;
					}
				}
			}

			return null;
		}

		//注意：该方法内部没有进行线程同步处理，调用者必须确保线程安全。
		private static ResourceManager EnsureResourceManager(string baseName, Assembly assembly)
		{
			if(assembly == null || string.IsNullOrWhiteSpace(baseName))
				return null;

			ResourceManager resourceManager;
			ResourceManagerCollection resourceManagers;

			if(!_resources.TryGetValue(assembly, out resourceManagers))
			{
				resourceManagers = new ResourceManagerCollection();
				_resources.Add(assembly, resourceManagers);
			}

			if(!resourceManagers.TryGetValue(baseName, out resourceManager))
			{
				resourceManager = new ResourceManager(baseName, assembly);
				resourceManagers.Add(resourceManager);
			}

			return resourceManager;
		}
		#endregion

		#region 嵌套子类
		private class ResourceManagerCollection : KeyedCollection<string, ResourceManager>
		{
			public ResourceManagerCollection() : base(StringComparer.OrdinalIgnoreCase)
			{
			}

			protected override string GetKeyForItem(ResourceManager item)
			{
				return item.BaseName;
			}

			public bool TryGetValue(string name, out ResourceManager value)
			{
				value = null;

				if(this.Dictionary != null && this.Dictionary.Count > 0)
					return this.Dictionary.TryGetValue(name, out value);

				if(this.Items != null && this.Items.Count > 0)
				{
					foreach(ResourceManager item in this.Items)
					{
						if(string.Equals(name, item.BaseName, StringComparison.OrdinalIgnoreCase))
						{
							value = item;
							return true;
						}
					}
				}

				return false;
			}
		}
		#endregion
	}
}
