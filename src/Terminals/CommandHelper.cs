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
		internal static void DisplayCommandInfo(ITerminal terminal, ICommand command)
		{
			if(terminal == null || command == null)
				return;

			DisplayNameAttribute displayName = (DisplayNameAttribute)TypeDescriptor.GetAttributes(command)[typeof(DisplayNameAttribute)];
			DescriptionAttribute description = (DescriptionAttribute)TypeDescriptor.GetAttributes(command)[typeof(DescriptionAttribute)];

			terminal.Write(TerminalColor.Blue, command.Name + " ");

			if(!command.Enabled)
				terminal.Write(TerminalColor.DarkGray, "({0})", ResourceUtility.GetString("${Disabled}"));

			if(displayName == null || string.IsNullOrWhiteSpace(displayName.DisplayName))
				terminal.Write(ResourceUtility.GetString("${Command}"));
			else
				terminal.Write(ResourceUtility.GetString(displayName.DisplayName, command.GetType().Assembly));

			CommandOptionAttribute[] optionAttributes = (CommandOptionAttribute[])command.GetType().GetCustomAttributes(typeof(CommandOptionAttribute), true);

			if(optionAttributes != null && optionAttributes.Length > 0)
			{
				terminal.WriteLine("," + ResourceUtility.GetString("${CommandUsages}"), optionAttributes.Length);
				terminal.WriteLine();

				string commandName = command.Name;

				terminal.Write(TerminalColor.Blue, commandName + " ");

				foreach(var optionAttribute in optionAttributes)
				{
					if(optionAttribute.Required)
					{
						terminal.Write("<-");
						terminal.Write(TerminalColor.DarkYellow, optionAttribute.Name);
						terminal.Write("> ");
					}
					else
					{
						terminal.Write("[-");
						terminal.Write(TerminalColor.DarkYellow, optionAttribute.Name);
						terminal.Write("] ");
					}
				}

				terminal.WriteLine();

				int maxOptionLength = GetMaxOptionLength(optionAttributes) + 2;

				foreach(var optionAttribute in optionAttributes)
				{
					int optionPadding = maxOptionLength - optionAttribute.Name.Length;

					terminal.Write("\t-");
					terminal.Write(TerminalColor.DarkYellow, optionAttribute.Name);

					if(optionAttribute.Type != null)
					{
						terminal.Write(":");
						terminal.Write(TerminalColor.Magenta, GetSimpleTypeName(optionAttribute.Type));
						optionPadding -= (GetSimpleTypeName(optionAttribute.Type).Length + 1);
					}

					terminal.Write(" (".PadLeft(optionPadding));

					if(optionAttribute.Required)
						terminal.Write(TerminalColor.DarkRed, ResourceUtility.GetString("${Required}"));
					else
						terminal.Write(TerminalColor.DarkGreen, ResourceUtility.GetString("${Optional}"));

					terminal.Write(") ");

					if(!string.IsNullOrWhiteSpace(optionAttribute.Description))
						terminal.Write(ResourceUtility.GetString(optionAttribute.Description, command.GetType().Assembly));

					if(optionAttribute.Type != null && optionAttribute.Type.IsEnum)
					{
						var entries = Zongsoft.Common.EnumUtility.GetEnumEntries(optionAttribute.Type, false);
						var maxEnumLength = entries.Max(entry => string.IsNullOrWhiteSpace(entry.Alias) ? entry.Name.Length : entry.Name.Length + entry.Alias.Length + 2);

						foreach(var entry in entries)
						{
							var enumPadding = maxEnumLength - entry.Name.Length;

							terminal.WriteLine();
							terminal.Write("\t".PadRight(optionAttribute.Name.Length + 3));
							terminal.Write(TerminalColor.DarkMagenta, entry.Name.ToLowerInvariant());

							if(!string.IsNullOrWhiteSpace(entry.Alias))
							{
								terminal.Write(TerminalColor.DarkGray, "(");
								terminal.Write(TerminalColor.DarkMagenta, entry.Alias);
								terminal.Write(TerminalColor.DarkGray, ")");

								enumPadding -= entry.Alias.Length + 2;
							}

							if(!string.IsNullOrWhiteSpace(entry.Description))
								terminal.Write(new string(' ', enumPadding + 1) + entry.Description);
						}
					}

					terminal.WriteLine();
				}
			}

			if(description != null && !string.IsNullOrWhiteSpace(description.Description))
			{
				terminal.WriteLine();
				terminal.WriteLine(TerminalColor.DarkYellow, ResourceUtility.GetString(description.Description, command.GetType().Assembly));
			}

			terminal.WriteLine();
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
