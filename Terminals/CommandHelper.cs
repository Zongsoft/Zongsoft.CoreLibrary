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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using Zongsoft.Services;
using Zongsoft.Resources;

namespace Zongsoft.Terminals
{
	internal static class CommandHelper
	{
		public static void DisplayCommandInfo(ITerminal terminal, ICommand command)
		{
			if(terminal == null || command == null)
				return;

			DisplayNameAttribute displayName = (DisplayNameAttribute)TypeDescriptor.GetAttributes(command)[typeof(DisplayNameAttribute)];
			DescriptionAttribute description = (DescriptionAttribute)TypeDescriptor.GetAttributes(command)[typeof(DescriptionAttribute)];

			terminal.Write(command.Name.ToLowerInvariant() + " ");

			if(!command.Enabled)
				terminal.Write(TerminalColor.DarkGray, "({0})", ResourceUtility.GetString("${Disabled}"));

			if(displayName == null || string.IsNullOrWhiteSpace(displayName.DisplayName))
				terminal.Write(ResourceUtility.GetString("${Command}"));
			else
				terminal.Write(TerminalColor.DarkYellow, ResourceUtility.GetString(displayName.DisplayName, command.GetType().Assembly));

			CommandOptionAttribute[] attributes = (CommandOptionAttribute[])command.GetType().GetCustomAttributes(typeof(CommandOptionAttribute), true);
			if(attributes != null && attributes.Length > 0)
			{
				terminal.WriteLine("," + ResourceUtility.GetString("${CommandUsages}"), attributes.Length);
				terminal.WriteLine();

				string commandName = command.Name;

				//if(command is Command)
				//    commandName = ((Command)command).FullName;

				terminal.Write(commandName.ToLowerInvariant() + " ");

				foreach(var attribute in attributes)
				{
					if(attribute.Required)
						terminal.Write("-{0} ", attribute.Name);
					else
						terminal.Write("[-{0}] ", attribute.Name);
				}

				terminal.WriteLine();

				int maxLength = GetMaxOptionLength(attributes) + 2;

				foreach(var attribute in attributes)
				{
					terminal.Write("\t-");
					terminal.Write(TerminalColor.DarkYellow, attribute.Name);
					int padding = maxLength - attribute.Name.Length;

					if(attribute.Type != null)
					{
						terminal.Write(":");
						terminal.Write(TerminalColor.DarkYellow, GetSimpleTypeName(attribute.Type));
						padding -= (GetSimpleTypeName(attribute.Type).Length + 1);
					}

					terminal.Write(" [".PadLeft(padding));
					if(attribute.Required)
						terminal.Write(TerminalColor.DarkMagenta, ResourceUtility.GetString("${Required}"));
					else
						terminal.Write(TerminalColor.DarkGreen, ResourceUtility.GetString("${Optional}"));
					terminal.Write("] ");

					if(!string.IsNullOrWhiteSpace(attribute.Description))
					{
						terminal.Write(TerminalColor.DarkYellow, ResourceUtility.GetString(attribute.Description, command.GetType().Assembly));
					}

					//if(string.IsNullOrWhiteSpace(attribute.Values))
					//    terminal.WriteLine();
					//else
					//{
					//    terminal.Write(ResourceUtility.GetString("${ValueRange}"));
					//    terminal.WriteLine(TerminalColor.DarkYellow, attribute.Values);
					//}
				}
			}

			if(description != null && !string.IsNullOrWhiteSpace(description.Description))
			{
				terminal.WriteLine();
				terminal.WriteLine(TerminalColor.DarkGray, ResourceUtility.GetString(description.Description, command.GetType().Assembly));
			}
		}

		public static void DisplayCommandInfo(ICommand command, TextWriter writer)
		{
			if(command == null || writer == null)
				return;

			var description = (DescriptionAttribute)command.GetType().GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();

			if(description != null)
			{
				writer.WriteLine(Resources.ResourceUtility.GetString("CommandDescription"));
				writer.WriteLine("\t" + Resources.ResourceUtility.GetString(description.Description));
			}

			writer.WriteLine(Resources.ResourceUtility.GetString("CommandUsages"));
			writer.Write("\t" + command.Name);

			var options = Zongsoft.Services.CommandHelper.GetOptions(command);

			foreach(var option in options)
			{
				if(option.Required)
					writer.Write(" -{0}", option.Name);
				else
					writer.Write(" [-{0}]", option.Name);
			}

			writer.WriteLine();

			if(options.Length > 0)
			{
				writer.WriteLine(Resources.ResourceUtility.GetString("CommandOptions"));

				foreach(var option in options)
				{
					writer.Write("\t-{0}", option.Name);

					if(option.Type != null)
					{
						writer.Write(":{0}", GetSimpleTypeName(option.Type));

						if(option.DefaultValue != null)
							writer.Write(" [{0}]", option.DefaultValue);
					}

					writer.WriteLine(" " + option.Description);

					if(option.Type != null && option.Type.IsEnum)
					{
						string indentText = new string(' ', option.Name.Length + 2);

						var entries = Common.EnumUtility.GetEnumEntries(option.Type, false);
						foreach(var entry in entries)
						{
							writer.WriteLine("\t{0}{1}\t{2}", indentText, entry.Name, entry.Description);
						}
					}
				}
			}
		}

		private static int GetMaxOptionLength(IEnumerable<CommandOptionAttribute> attributes)
		{
			if(attributes == null)
				return 0;

			int result = 0;

			foreach(var attribute in attributes)
			{
				if(attribute.Type == null)
					result = Math.Max(attribute.Name.Length, result);
				else
					result = Math.Max(attribute.Name.Length + GetSimpleTypeName(attribute.Type).Length + 1, result);
			}

			return result > 0 ? result + 1 : result;
		}

		private static string GetSimpleTypeName(Type type)
		{
			if(type.IsEnum)
				return "enum";

			switch(Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					return "boolean";
				case TypeCode.Byte:
					return "byte";
				case TypeCode.Char:
					return "char";
				case TypeCode.DateTime:
					return "datetime";
				case TypeCode.Decimal:
				case TypeCode.Double:
					return "numeric";
				case TypeCode.Int16:
				case TypeCode.UInt16:
					return "short";
				case TypeCode.Int32:
				case TypeCode.UInt32:
					return "int";
				case TypeCode.Int64:
				case TypeCode.UInt64:
					return "long";
				case TypeCode.String:
					return "string";
			}

			return type.Name;
		}
	}
}
