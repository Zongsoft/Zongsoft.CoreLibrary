/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2011-2016 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.ComponentModel;
using System.Linq;

using Zongsoft.Resources;

namespace Zongsoft.Services.Commands
{
	[DisplayName("${Text.HelpCommand.Title}")]
	[Description("${Text.HelpCommand.Description}")]
	public class HelpCommand : CommandBase<CommandContext>
	{
		#region 构造函数
		public HelpCommand() : base("Help")
		{
		}

		public HelpCommand(string name) : base(name)
		{
		}
		#endregion

		#region 重写方法
		protected override void OnExecute(CommandContext context)
		{
			if(context.Expression.Arguments.Count < 1)
			{
				foreach(var node in context.Executor.Root.Children)
				{
					PrintCommandNode(context.Output, node, 0);
				}

				return;
			}

			foreach(var argument in context.Expression.Arguments)
			{
				if(argument == "?" || argument == ".")
				{
					PrintHelpInfo(context.Output, this);
					continue;
				}

				CommandTreeNode node = context.Executor.Find(argument);

				if(node == null)
				{
					context.Output.WriteLine(CommandOutletColor.Red, ResourceUtility.GetString("CommandNotFound", argument));
					continue;
				}

				if(node != null && node.Command != null)
				{
					context.Output.WriteLine(node.FullPath);
					PrintHelpInfo(context.Output, node.Command);
				}
			}
		}
		#endregion

		#region 静态方法
		public static void PrintHelpInfo(ICommandOutlet output, ICommand command)
		{
			if(output == null || command == null)
				return;

			DisplayNameAttribute displayName = (DisplayNameAttribute)TypeDescriptor.GetAttributes(command)[typeof(DisplayNameAttribute)];
			DescriptionAttribute description = (DescriptionAttribute)TypeDescriptor.GetAttributes(command)[typeof(DescriptionAttribute)];

			output.Write(CommandOutletColor.Blue, command.Name + " ");

			if(!command.Enabled)
				output.Write(CommandOutletColor.DarkGray, "({0})", ResourceUtility.GetString("${Disabled}"));

			if(displayName == null || string.IsNullOrWhiteSpace(displayName.DisplayName))
				output.Write(ResourceUtility.GetString("${Command}"));
			else
				output.Write(ResourceUtility.GetString(displayName.DisplayName, command.GetType().Assembly));

			CommandOptionAttribute[] optionAttributes = (CommandOptionAttribute[])command.GetType().GetCustomAttributes(typeof(CommandOptionAttribute), true);

			if(optionAttributes != null && optionAttributes.Length > 0)
			{
				output.WriteLine("," + ResourceUtility.GetString("${CommandUsages}"), optionAttributes.Length);
				output.WriteLine();

				string commandName = command.Name;

				output.Write(CommandOutletColor.Blue, commandName + " ");

				foreach(var optionAttribute in optionAttributes)
				{
					if(optionAttribute.Required)
					{
						output.Write("<-");
						output.Write(CommandOutletColor.DarkYellow, optionAttribute.Name);
						output.Write("> ");
					}
					else
					{
						output.Write("[-");
						output.Write(CommandOutletColor.DarkYellow, optionAttribute.Name);
						output.Write("] ");
					}
				}

				output.WriteLine();

				int maxOptionLength = GetMaxOptionLength(optionAttributes) + 2;

				foreach(var optionAttribute in optionAttributes)
				{
					int optionPadding = maxOptionLength - optionAttribute.Name.Length;

					output.Write("\t-");
					output.Write(CommandOutletColor.DarkYellow, optionAttribute.Name);

					if(optionAttribute.Type != null)
					{
						output.Write(":");
						output.Write(CommandOutletColor.Magenta, GetSimpleTypeName(optionAttribute.Type));
						optionPadding -= (GetSimpleTypeName(optionAttribute.Type).Length + 1);
					}

					output.Write(" (".PadLeft(optionPadding));

					if(optionAttribute.Required)
						output.Write(CommandOutletColor.DarkRed, ResourceUtility.GetString("${Required}"));
					else
						output.Write(CommandOutletColor.DarkGreen, ResourceUtility.GetString("${Optional}"));

					output.Write(") ");

					if(!string.IsNullOrWhiteSpace(optionAttribute.Description))
						output.Write(ResourceUtility.GetString(optionAttribute.Description, command.GetType().Assembly));

					if(optionAttribute.Type != null && optionAttribute.Type.IsEnum)
					{
						var entries = Zongsoft.Common.EnumUtility.GetEnumEntries(optionAttribute.Type, false);
						var maxEnumLength = entries.Max(entry => string.IsNullOrWhiteSpace(entry.Alias) ? entry.Name.Length : entry.Name.Length + entry.Alias.Length + 2);

						foreach(var entry in entries)
						{
							var enumPadding = maxEnumLength - entry.Name.Length;

							output.WriteLine();
							output.Write("\t".PadRight(optionAttribute.Name.Length + 3));
							output.Write(CommandOutletColor.DarkMagenta, entry.Name.ToLowerInvariant());

							if(!string.IsNullOrWhiteSpace(entry.Alias))
							{
								output.Write(CommandOutletColor.DarkGray, "(");
								output.Write(CommandOutletColor.DarkMagenta, entry.Alias);
								output.Write(CommandOutletColor.DarkGray, ")");

								enumPadding -= entry.Alias.Length + 2;
							}

							if(!string.IsNullOrWhiteSpace(entry.Description))
								output.Write(new string(' ', enumPadding + 1) + entry.Description);
						}
					}

					output.WriteLine();
				}
			}

			if(description != null && !string.IsNullOrWhiteSpace(description.Description))
			{
				output.WriteLine();
				output.WriteLine(CommandOutletColor.DarkYellow, ResourceUtility.GetString(description.Description, command.GetType().Assembly));
			}

			output.WriteLine();
		}
		#endregion

		#region 私有方法
		private static void PrintCommandNode(ICommandOutlet output, CommandTreeNode node, int depth)
		{
			if(node == null)
				return;

			var indent = depth > 0 ? new string(' ', depth * 4) : string.Empty;
			var fulName = node.FullPath.Trim('/').Replace('/', '.');

			if(node.Command == null)
				output.WriteLine("{1}[{0}]", fulName, indent);
			else
			{
				var displayName = (DisplayNameAttribute)Attribute.GetCustomAttribute(node.Command.GetType(), typeof(DisplayNameAttribute), true);

				output.Write("{1}{0}", fulName, indent);

				if(displayName == null)
					output.WriteLine();
				else
					output.WriteLine(CommandOutletColor.DarkYellow, " " + ResourceUtility.GetString(displayName.DisplayName, node.Command.GetType().Assembly));
			}

			if(node.Children.Count > 0)
			{
				foreach(var child in node.Children)
					PrintCommandNode(output, child, depth + 1);
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
		#endregion
	}
}
