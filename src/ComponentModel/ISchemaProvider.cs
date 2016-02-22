/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2008-2012 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.ComponentModel
{
	/// <summary>
	/// 提供<seealso cref="Schema"/>相关功能的接口。
	/// </summary>
	[Obsolete]
	public interface ISchemaProvider
	{
		/// <summary>
		/// 获取当前应用中所有<seealso cref="Schema"/>的平面集合。
		/// </summary>
		/// <returns>当前应用中的所有<see cref="Schema"/>集合，该集合不含层次结构。</returns>
		/// <remarks>
		///		<para>如果需要获取应用中<see cref="Schema"/>的树型结构模型，请参考：<seealso cref="GetHierarchicalSchemas"/>方法。</para>
		/// </remarks>
		SchemaCollection GetSchemas();

		/// <summary>
		/// 获取当前应用中所有<seealso cref="Schema"/>的树型结构。
		/// </summary>
		/// <returns>包含当前应用中所有<see cref="Schema"/>的分类根节点。</returns>
		/// <remarks>
		///		<para>如果只需要获取应用中所有<see cref="Schema"/>的平面集，请参考：<seealso cref="GetSchemas"/>方法。</para>
		/// </remarks>
		SchemaCategory GetHierarchicalSchemas();
	}
}
