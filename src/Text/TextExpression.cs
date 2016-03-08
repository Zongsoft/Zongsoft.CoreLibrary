/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013-2014 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace Zongsoft.Text
{
	/// <summary>
	/// 描述表达式的类。
	/// </summary>
	/// <remarks>
	///		<para>关于表达式的几种常用写法及其格式组成大致如下：</para>
	///		<list type="table">
	///			<listheader>
	///				<term>表达式</term>
	///				<term>方案(Scheme)</term>
	///				<term>文本(Text)</term>
	///				<term>格式字符串(Format)</term>
	///				<term>参数数组(Arguments)</term>
	///			</listheader>
	///			<item>
	///				<term>${text}</term>
	///				<term></term>
	///				<term>text</term>
	///				<term></term>
	///				<term></term>
	///			</item>
	///			<item>
	///				<term>${text#format}</term>
	///				<term></term>
	///				<term>text</term>
	///				<term>format</term>
	///				<term></term>
	///			</item>
	///			<item>
	///				<term>${binding:ViewData[Model].Title}</term>
	///				<term>binding</term>
	///				<term>ViewData[Model].Title</term>
	///				<term></term>
	///				<term></term>
	///			</item>
	///			<item>
	///				<term>${binding:ViewData[Model].Birthdate#D}</term>
	///				<term>binding</term>
	///				<term>ViewData[Model].Birthdate</term>
	///				<term>D</term>
	///				<term></term>
	///			</item>
	///			<item>
	///				<term>${binding:${ViewData[MemberPath]}#D}</term>
	///				<term>binding</term>
	///				<term>${ViewData[MemberPath]}</term>
	///				<term>D</term>
	///				<term></term>
	///			</item>
	///			<item>
	///				<term>${method:IsAuthorized /forum/thread/create post}</term>
	///				<term>method</term>
	///				<term>IsAuthorized</term>
	///				<term></term>
	///				<term>
	///					<list type="bullet">
	///						<term>/forum/thread/create</term>
	///						<term>post</term>
	///					</list>
	///				</term>
	///			</item>
	///			<item>
	///				<term>${method:IsAuthorized ${Request.Path} post}</term>
	///				<term>method</term>
	///				<term>IsAuthorized</term>
	///				<term></term>
	///				<term>
	///					<list type="bullet">
	///						<term>${Request.Path}</term>
	///						<term>post</term>
	///					</list>
	///				</term>
	///			</item>
	///		</list>
	/// </remarks>
	[Obsolete()]
	public class TextExpression
	{
		#region 私有变量
		private int _textEvaluated;
		#endregion

		#region 成员字段
		private string _scheme;
		private string _text;
		private string _format;
		private TextExpressionArgument[] _arguments;
		private TextExpressionNodeCollection _nodes;
		#endregion

		#region 构造函数
		public TextExpression(string scheme, string text, string format, TextExpressionArgument[] args)
		{
			_scheme = string.IsNullOrWhiteSpace(scheme) ? string.Empty : scheme.Trim();
			_text = string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim();
			_format = string.IsNullOrWhiteSpace(format) ? string.Empty : format.Trim();
			_arguments = args ?? new TextExpressionArgument[0];
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取表达式的模式，该模式对应为某种解析器。
		/// </summary>
		public string Scheme
		{
			get
			{
				return _scheme;
			}
		}

		/// <summary>
		/// 获取表达式的文本，该属性值由<see cref="Scheme"/>指定的解析器来解析。
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
		}

		/// <summary>
		/// 获取表达式的文本值(即<see cref="Text"/>属性)对应的表达式节点集合(<seealso cref="TextExpressionNodeCollection"/>)，如果<see cref="Text"/>属性不是一个有效的表达式则返回空集。
		/// </summary>
		/// <remarks>
		///		<para>该属性采用延迟计算机制，如果不获取本属性则不会解析表达式文本值。</para>
		/// </remarks>
		public TextExpressionNodeCollection Nodes
		{
			get
			{
				if(_textEvaluated == 0)
				{
					var textEvaluated = Interlocked.CompareExchange(ref _textEvaluated, 1, 0);

					if(textEvaluated == 0)
						_nodes = TextExpressionParser.Parse(_text);
				}

				return _nodes;
			}
		}

		/// <summary>
		/// 获取表达式文本的格式字符串。
		/// </summary>
		public string Format
		{
			get
			{
				return _format;
			}
		}

		/// <summary>
		/// 获取表达式的参数数组，如果该表达式没有参数则返回长度为零的空数组。
		/// </summary>
		public TextExpressionArgument[] Arguments
		{
			get
			{
				return _arguments;
			}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			var result = "${" +
						 (string.IsNullOrEmpty(_scheme) ? string.Empty : _scheme + ":") +
						 _text +
						 (string.IsNullOrEmpty(_format) ? string.Empty : "#" + _format);

			if(_arguments != null && _arguments.Length > 0)
			{
				foreach(var argument in _arguments)
					result += " " + argument.ToString();
			}

			return result + "}";
		}
		#endregion
	}
}
