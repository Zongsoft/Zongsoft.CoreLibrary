/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
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
using System.Linq;
using System.Collections.Generic;

using Zongsoft.Common;
using Zongsoft.Runtime.Caching;

namespace Zongsoft.Communication.Net
{
	/// <summary>
	/// 默认实现的网络包的打包器。
	/// </summary>
	/// <remarks>
	///		<para>其定义的网络通讯包格式如下：</para>
	///		<list type="bullet">
	///			<item>
	///				<term>包的流水号(8 bytes)，该字段为递增的数值，可循环使用。</term>
	///				<term>数据总长度(8 bytes)，完整的数据可能被分拆成多个包发送，该字段为数据的完整大小。</term>
	///				<term>内容长度(4 bytes)。</term>
	///				<term>内容数据，该内容的实际长度由上面字段指定。</term>
	///			</item>
	///		</list>
	/// </remarks>
	public class TcpPacketizer : IPacketizer
	{
		#region 常量定义
		private const int HEAD_LENGTH = 20;
		#endregion

		#region 私有变量
		private long _sequenceId;
		private int _contentLength;
		private int _headOffset;
		private byte[] _headBuffer;
		private TcpPackageHeader _head;
		private Dictionary<long, BufferState> _bufferStates;
		#endregion

		#region 成员字段
		private BufferEvaluator _bufferEvaluator;
		private IBufferManager _bufferManager;
		private IBufferManagerSelector _bufferSelector;
		#endregion

		#region 构造函数
		public TcpPacketizer(IBufferManager bufferManager)
		{
			if(bufferManager == null)
				throw new ArgumentNullException("bufferManager");

			_headBuffer = new byte[HEAD_LENGTH];
			_bufferManager = bufferManager;
			_bufferEvaluator = BufferEvaluator.Default;
			_bufferStates = new Dictionary<long, BufferState>();
		}

		public TcpPacketizer(IBufferManagerSelector bufferSelector)
		{
			if(bufferSelector == null)
				throw new ArgumentNullException("bufferSelector");

			_headBuffer = new byte[HEAD_LENGTH];
			_bufferSelector = bufferSelector;
			_bufferEvaluator = BufferEvaluator.Default;
			_bufferStates = new Dictionary<long, BufferState>();
		}
		#endregion

		#region 公共属性
		public IBufferManager BufferManager
		{
			get
			{
				return _bufferManager;
			}
			set
			{
				_bufferManager = value;
			}
		}

		public IBufferManagerSelector BufferSelector
		{
			get
			{
				return _bufferSelector;
			}
			set
			{
				_bufferSelector = value;
			}
		}
		#endregion

		#region 打包方法
		public virtual IEnumerable<Zongsoft.Common.Buffer> Pack(Zongsoft.Common.Buffer buffer)
		{
			if(buffer == null || buffer.Count == 0)
			{
				//遍历返回字节组缓存信息
				yield return new Zongsoft.Common.Buffer(new byte[HEAD_LENGTH], 0);
			}

			var sequenceId = System.Threading.Interlocked.Increment(ref _sequenceId);
			var bytes = new byte[_bufferEvaluator.GetBufferSize(buffer.Count)];
			int contentLength = 0;

			while((contentLength = buffer.Read(bytes, HEAD_LENGTH, bytes.Length - HEAD_LENGTH)) > 0)
			{
				//设置包的头部字段
				this.SetPackageHead(bytes, sequenceId, buffer.Count, contentLength);

				//遍历返回字节组缓存信息
				yield return new Zongsoft.Common.Buffer(bytes, 0, HEAD_LENGTH + contentLength);

				//注意：一定要重新分配缓存数组，不然同一个缓存在高速发送过程中肯定会发生写入覆盖
				bytes = new byte[_bufferEvaluator.GetBufferSize(buffer.Count)];
			}
		}

		public virtual IEnumerable<Zongsoft.Common.Buffer> Pack(Stream stream)
		{
			if(stream == null || stream.Length - stream.Position == 0)
			{
				//遍历返回字节组缓存信息
				yield return new Zongsoft.Common.Buffer(new byte[HEAD_LENGTH], 0);
			}

			var sequenceId = System.Threading.Interlocked.Increment(ref _sequenceId);
			var totalLength = stream.Length - stream.Position;
			byte[] buffer = new byte[_bufferEvaluator.GetBufferSize(totalLength)];
			int contentLength = 0;

			while((contentLength = stream.Read(buffer, HEAD_LENGTH, buffer.Length - HEAD_LENGTH)) > 0)
			{
				//设置包的头部字段
				this.SetPackageHead(buffer, sequenceId, totalLength, contentLength);

				//遍历返回字节组缓存信息
				yield return new Zongsoft.Common.Buffer(buffer, 0, HEAD_LENGTH + contentLength);

				//注意：一定要重新分配缓存数组，不然同一个缓存在高速发送过程中肯定会发生写入覆盖
				buffer = new byte[_bufferEvaluator.GetBufferSize(totalLength)];
			}
		}

		private void SetPackageHead(byte[] buffer, long sequenceId, long totalLength, int contentLength)
		{
			//设置包的流水号
			System.Buffer.BlockCopy(BitConverter.GetBytes(sequenceId), 0, buffer, 0, 8);
			//设置数据的总长
			System.Buffer.BlockCopy(BitConverter.GetBytes(totalLength), 0, buffer, 8, 8);
			//设置内容长度
			System.Buffer.BlockCopy(BitConverter.GetBytes(contentLength), 0, buffer, 16, 4);
		}
		#endregion

		#region 拆包方法
		public virtual IEnumerable<object> Unpack(Zongsoft.Common.Buffer buffer)
		{
			int availableLength;

			while(buffer.CanRead())
			{
				//如果当前头部缓存指针位置小于包头的长度，则必须先读取头部数据
				if(_contentLength == 0 && _headOffset < HEAD_LENGTH)
				{
					//从当前接受缓存中读取剩下的包头数据
					availableLength = buffer.Read(_headBuffer, _headOffset, HEAD_LENGTH - _headOffset);

					//将当前头部缓存指针加上当前实际读取的长度
					_headOffset += availableLength;

					//如果当前头部缓存指针位置仍然小于包头长度，则说明当前接数据缓存区数据量不够，
					//即本次接收到的缓存数据已被读完，故直接退出当前方法。
					if(_headOffset < HEAD_LENGTH)
						yield break;

					//至此头部数据接收完毕，将头部缓存解析成包头实体
					_head = this.ResolveHeader(_headBuffer);

					//如果包头指示当前数据包的实际内容长度为零，则说明此包为空包
					if(_head.TotalLength == 0 || _head.ContentLength == 0)
					{
						//重置缓存指针位置，以指示下次需要进行包头解析
						_headOffset = 0;
						_contentLength = 0;

						//接着处理下一个小包
						continue;
					}

					//映射当前数据包对应的缓存区。
					if(!_bufferStates.ContainsKey(_head.SequenceId))
					{
						var bufferManager = this.GetBufferManager();
						var id = bufferManager.Allocate(_head.TotalLength);
						_bufferStates[_head.SequenceId] = new BufferState(bufferManager.GetStream(id));
					}
				}

				//计算出本次要接受的内容长度
				int contentLength = _contentLength == 0 ? _head.ContentLength : _contentLength;

				//定义当前接收的数据包对应的缓存状态对象
				BufferState bufferState;

				//从缓存容器中获取当前大包的缓存状态对象
				if(!_bufferStates.TryGetValue(_head.SequenceId, out bufferState))
					throw new InvalidOperationException("Can not obtain the BufferCache with sequence-id.");

				//将接收到的数据写入缓存区
				availableLength = buffer.Read(bufferState.BufferStream, contentLength);
				//更新当前缓存状态中的缓存数
				bufferState.BufferedSize += availableLength;
				//设置下次要接收的内容长度
				_contentLength = contentLength - availableLength;

				//如果下次要接收的内容长度为零，则指示下次接收要先进行包头的处理，即将头缓存偏移量置零
				if(_contentLength == 0)
					_headOffset = 0;

				//如果整个大包全部接受完毕
				if(bufferState.BufferedSize == _head.TotalLength)
				{
					//重置缓存指针位置，以指示下次需要进行包头解析
					_headOffset = 0;
					_contentLength = 0;

					//重置当前缓存区流的指针
					bufferState.BufferStream.Position = 0;

					//将当前接收的数据包从缓存映射中删除
					_bufferStates.Remove(_head.SequenceId);

					yield return bufferState.BufferStream;
				}
			}
		}
		#endregion

		#region 私有方法
		private TcpPackageHeader ResolveHeader(byte[] head)
		{
			long sequenceId = BitConverter.ToInt64(head, 0);
			long totalLength = BitConverter.ToInt64(head, 8);
			int contentLength = BitConverter.ToInt32(head, 16);

			return new TcpPackageHeader(sequenceId, totalLength, contentLength);
		}

		private IBufferManager GetBufferManager()
		{
			if(_bufferManager != null)
				return _bufferManager;

			//如果缓存管理选择器不为空，则通过缓存管理选择器根据当前要接收收据的总长度来选择一个恰当的缓存管理器
			if(_bufferSelector != null)
				return _bufferSelector.GetBufferManager(_head.TotalLength) ?? _bufferManager;

			throw new InvalidOperationException("Cann't obtain a BufferManager.");
		}
		#endregion

		#region 包头结构
		private struct TcpPackageHeader
		{
			#region 私有字段
			private IDictionary<string, object> _properties;
			#endregion

			#region 公共字段
			public readonly long SequenceId;
			public readonly long TotalLength;
			public readonly int ContentLength;
			#endregion

			#region 构造函数
			public TcpPackageHeader(long sequenceId, long totalLength, int contentLength)
			{
				this.SequenceId = sequenceId;
				this.TotalLength = totalLength;
				this.ContentLength = contentLength;

				_properties = null;
			}
			#endregion

			#region 公共属性
			public bool HasProperties
			{
				get
				{
					return _properties != null && _properties.Count > 0;
				}
			}

			public IDictionary<string, object> Properties
			{
				get
				{
					if(_properties == null)
						System.Threading.Interlocked.CompareExchange(ref _properties, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), null);

					return _properties;
				}
			}
			#endregion
		}
		#endregion

		#region 缓存状态
		private class BufferState
		{
			public Stream BufferStream;
			public long BufferedSize;

			public BufferState(Stream bufferStream)
			{
				this.BufferStream = bufferStream;
			}
		}
		#endregion
	}
}
