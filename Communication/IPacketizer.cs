/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *   邓祥云(X.Z. Deng) <627825056@qq.com>
 *
 * Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
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

namespace Zongsoft.Communication
{
	/// <summary>
	/// 提供通讯协议解析功能的接口。
	/// </summary>
	public interface IPacketizer
	{
		/// <summary>
		/// 将指定的源数据字节组信息解析(打包)成待发送的字节组信息集。
		/// </summary>
		/// <param name="buffer">要发送的待解析(打包)的源数据字节组信息。</param>
		/// <returns>返回解析(打包)成功后的字节组信息集。</returns>
		/// <remarks>
		///		<para>注意：实现者应确保该方法的返回结果始终不能为空(null)。</para>
		/// </remarks>
		IEnumerable<Zongsoft.Common.Buffer> Pack(Zongsoft.Common.Buffer buffer);

		/// <summary>
		/// 将指定的源数据流解析(打包)成待发送的字节组信息集。
		/// </summary>
		/// <param name="stream">要发送的待解析(打包)的源数据流。</param>
		/// <returns>返回解析(打包)成功后的字节组信息集。</returns>
		/// <remarks>
		///		<para>注意：实现者应确保该方法的返回结果始终不能为空(null)。</para>
		/// </remarks>
		IEnumerable<Zongsoft.Common.Buffer> Pack(Stream stream);

		/// <summary>
		/// 将接收到的字节组信息解析(解包)成目标数据集。
		/// </summary>
		/// <param name="buffer">接收到的待解析(解包)的字节组信息。</param>
		/// <returns>返回解析(解包)成功后的目标数据集。</returns>
		/// <remarks>
		///		<para>注意：实现者应确保该方法的返回结果始终不能为空(null)。</para>
		/// </remarks>
		IEnumerable<object> Unpack(Zongsoft.Common.Buffer buffer);
	}
}
