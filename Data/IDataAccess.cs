/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2003-2010 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zongsoft.Data
{
	/// <summary>
	/// 表示数据访问的公共接口。
	/// </summary>
	[Obsolete]
	public interface IDataAccess
	{
		/// <summary>
		/// 在映射中根据指定的适配器名称，构建带参数查询并返回查询的数据集。
		/// </summary>
		/// <param name="adapterName">查询适配器的名称。</param>
		/// <param name="parameterNames">包含查询参数名称的数组。</param>
		/// <param name="parameterValues">包含查询参数值的数组</param>
		/// <returns>返回适配器的带参数查询的数据集。</returns>
		//DataSet Select(string adapterName, string[] parameterNames, object[] parameterValues);

		/// <summary>
		/// 在映射中根据指定的适配器名称，构建带参数查询并返回查询的数据集。
		/// </summary>
		/// <param name="adapterName">查询适配器的名称。</param>
		/// <param name="inningParameters">包含查询参数的列表。</param>
		/// <returns>返回适配器的带参数查询的数据集。</returns>
		DataSet Select(string adapterName, IDictionary<string, object> inningParameters);

		/// <summary>
		/// 在映射中根据指定的适配器名称，构建带参数查询并返回查询的数据集。
		/// </summary>
		/// <param name="adapterName">查询适配器的名称。</param>
		/// <param name="inningParameters">包含查询参数的列表。</param>
		/// <param name="outingParameters">输出参数，指示查询命令返回的所有参数列表。</param>
		/// <returns>返回适配器的带参数查询的数据集。</returns>
		DataSet Select(string adapterName, IDictionary<string, object> inningParameters, out IDictionary<string, object> outingParameters);

		/// <summary>
		/// 在映射中根据指定的适配器名称，构建查询并返回查询的数据集。
		/// <para>
		///		注意：该查询方法不带参数，返回查询结果中的所有记录。
		/// </para>
		/// </summary>
		/// <param name="adapterName">查询适配器的名称。</param>
		/// <returns>返回适配的查询的数据集。</returns>
		DataSet SelectAll(string adapterName);

		/// <summary>
		/// 在映射中根据指定的适配器名称，构建查询并返回查询的数据集。
		/// <para>
		///		注意：该查询方法不带参数，返回查询结果中的所有记录。
		/// </para>
		/// </summary>
		/// <param name="adapterName">查询适配器的名称。</param>
		/// <param name="outingParameters">输出参数，指示查询命令返回的所有参数列表。</param>
		/// <returns>返回适配的查询的数据集。</returns>
		DataSet SelectAll(string adapterName, out IDictionary<string, object> outingParameters);

		/// <summary>
		/// 在映射中根据指定的适配器名称，构建查询并返回查询的空数据集(返回结果不带数据，只含有数据集的结构)。
		/// </summary>
		/// <param name="adapterName">查询适配器的名称。</param>
		/// <returns>返回适配器查询的空数据集。</returns>
		DataSet SelectSchema(string adapterName);

		/// <summary>
		/// 在映射中根据指定的适配器名称，构建查询并返回查询的空数据集(返回结果不带数据，只含有数据集的结构)。
		/// </summary>
		/// <param name="adapterName">查询适配器的名称。</param>
		/// <param name="outingParameters">输出参数，指示查询命令返回的所有参数列表。</param>
		/// <returns>返回适配器查询的空数据集。</returns>
		DataSet SelectSchema(string adapterName, out IDictionary<string, object> outingParameters);

		/// <summary>
		/// 在映射中根据制定的访问器名称，构建相应的命令将传入的数据集参数保存至数据源中。
		/// <para>
		///		注意：如果指定的访问器在映射文件中的访问器定义区不存在，则在适配器定义区中进行查找。
		/// </para>
		/// </summary>
		/// <param name="accessorName">更新数据集的访问器或适配器名称。</param>
		/// <param name="dataSet">要进行保存的数据集对象。</param>
		/// <returns>返回保存成功的受影响的记录数，该返回值为指定数据集的所有表的受影响的记录数。</returns>
		int Update(string accessorName, DataSet dataSet);
	}
}
