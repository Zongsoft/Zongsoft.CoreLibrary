/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2013 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.IO.MemoryMappedFiles;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;

namespace Zongsoft.Runtime.Caching
{
	public class BufferManager : BufferManagerBase, IDisposable
	{
		#region 常量定义
		private const uint StorageSize = (uint)(2L * 1024 * 1024 * 1024);
		private const uint ViewPageTotalSize = BufferUtility.GB;
		private const int ViewPageCount = 16;
		#endregion

		#region 私有变量
		private MemoryMappedFile _memoryMappedFile;
		private MetadataInfo _metadata;
		private MemoryMappedPageManager _pageManager;
		#endregion

		#region 成员变量
		private long _capacity;
		private int _count;
		private int _blockCount;
		#endregion

		#region 构造函数
		public BufferManager(string filePath) : this(filePath, 32 * BufferUtility.KB)
		{
		}

		public BufferManager(string filePath, int blockSize) : base(blockSize)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentNullException("filePath");

			this.Initialize(filePath);
		}

		public BufferManager(FileStream fileStream) : this(fileStream, 32 * BufferUtility.KB)
		{
		}

		public BufferManager(FileStream fileStream, int blockSize) : base(blockSize)
		{
			if(fileStream == null)
				throw new ArgumentNullException("fileStream");

			this.Initialize(fileStream);
		}

		private void Initialize(object file)
		{
			if((ViewPageTotalSize / ViewPageCount) % this.BlockSize != 0)
				throw new ArgumentException(string.Format("This '{0}' value of blockSize parameter is invalid.", this.BlockSize));

			//计算缓存区总共有多少个存储块
			_blockCount = (int)(StorageSize / this.BlockSize);

			//计算缓存区的总大(为存储区长度+索引区长度+分配表区长度+文件头长度)
			_capacity = StorageSize + (_blockCount * 4) + (36 * BufferUtility.KB) + MetadataInfo.HeadSize;

			//判断传入参数是否为文件流
			var fileStream = file as FileStream;

			if(fileStream == null)
			{
				var fileInfo = new FileInfo((string)file);

				if(fileInfo.Exists && fileInfo.Length > 0 && fileInfo.Length != _capacity)
					throw new ArgumentException("file");

				fileStream = new FileStream((string)file,
											FileMode.OpenOrCreate,
											FileAccess.ReadWrite,
											FileShare.ReadWrite,
											this.BlockSize,
											FileOptions.RandomAccess);

				if(fileStream.Length == 0)
					fileStream.SetLength(_capacity);
			}
			else
			{
				if(fileStream.Length > 0 && fileStream.Length != _capacity)
					throw new ArgumentException("file");
			}

			//创建内存映射文件对象
			_memoryMappedFile = MemoryMappedFile.CreateFromFile(fileStream,
																Path.GetFileName(fileStream.Name),
																_capacity,
																MemoryMappedFileAccess.ReadWrite,
																null,
																HandleInheritability.Inheritable,
																false);

			//构建默认的元数据描述对象
			_metadata = MetadataInfo.Create(this.BlockSize, _blockCount);

			//创建缓存页面管理器对象
			_pageManager = new MemoryMappedPageManager(this, (uint)_metadata.StorageOffset);
		}
		#endregion

		#region 单例模式
		private static BufferManager _default;
		public static BufferManager Default
		{
			get
			{
				if(_default == null)
					Interlocked.CompareExchange(ref _default, GetBufferManager(typeof(BufferManager).FullName + ".Default"), null);

				return _default;
			}
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前缓存管理器对应的内存映射文件的大小，单位：字节(byte)。
		/// </summary>
		/// <remarks>
		///		<para>内存映射文件的大小包括：存储区长度+索引区长度+分配表区长度+文件头长度。</para>
		///		<para>具体的内存映射文件的详细数据结构请参考相关设计文档。</para>
		/// </remarks>
		public long Capacity
		{
			get
			{
				return _capacity;
			}
		}

		/// <summary>
		/// 获取已经分配的缓存项数量。
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
		}

		/// <summary>
		/// 获取缓存区的总块数。
		/// </summary>
		public int BlockCount
		{
			get
			{
				return _blockCount;
			}
		}

		/// <summary>
		/// 获取当前缓存管理对应的<seealso cref="System.IO.MemoryMappedFiles.MemoryMappedFile"/>内存映射文件。
		/// </summary>
		public MemoryMappedFile Storage
		{
			get
			{
				return _memoryMappedFile;
			}
		}
		#endregion

		#region 重写方法
		public override int Allocate(long size)
		{
			//如果申请分配的大小超过2GB则抛出异常
			if(size > 0x80000000)
				throw new NotSupportedException("The size of allocation is too large.");

			//从直接寻址的分配表记录区分配一条记录
			var id = this.AllocateRecord(size, _metadata.AllocationTableBuffer);

			//如果分配成功则返回
			if(id >= 0)
				return id;

			var blockBuffer = new byte[this.BlockSize];
			var recordCount = _metadata.AllocationTableBuffer.Length / AllocationRecord.Length;

			unsafe
			{
				fixed(byte* pAddressBuffer = _metadata.AllocationTableAddressBuffer)
				{
					int* pAddressIndex = (int*)pAddressBuffer;

					for(int i = 0; i < _metadata.AllocationTableAddressBuffer.Length / 4; i++)
					{
						//如果当前的索引项的值为零，则将其改为-1；如果不为零，则不作改动。而无论该条件是否成立均返回其原始值。
						int blockId = Interlocked.CompareExchange(ref *pAddressIndex, -1, 0);

						//如果指向的存储块序号为零则说明该分配表记录区尚未分配对应的存储块
						if(blockId == 0)
						{
							//分配一块存储块，以用作分配表记录的存储空间
							blockId = this.AllocateBlocks(1);

							//如果分配存储块失败则直接返回失败
							if(blockId < 1)
							{
								//回滚上面修改的当前索引项的值
								*pAddressIndex = 0;

								//返回失败
								return -1;
							}

							//设置当前间接分配表地址值为分配到的存储块序号
							*pAddressIndex = blockId;
						}

						if(blockId > 0)
						{
							//从间接寻址的分配表记录区分配一条记录
							id = this.AllocateRecord(size, blockId);

							//如果分配成功则返回
							if(id >= 0)
								return (_metadata.AllocationTableBuffer.Length / AllocationRecord.Length) + (i * this.BlockSize / AllocationRecord.Length) + id;
						}

						//递增分配地址区的当前索引值
						pAddressIndex++;
					}
				}
			}

			//最终返回失败
			return -1;
		}

		public override void Release(int id)
		{
			if(id < 0)
				return;

			int head = this.ReleaseRecord(id);

			if(head > 0)
				this.ReleaseBlocks(head);
		}

		public override Stream GetStream(int id)
		{
			if(id < 0)
				throw new ArgumentOutOfRangeException("id");

			var record = this.GetRecord(id);

			if(record.HasValue)
				return new BufferViewStream(id, record.Value, this);
			else
				throw new InvalidOperationException(string.Format("Can't found the buffer record by id({0}).", id));
		}
		#endregion

		#region 处置方法
		public void Dispose()
		{
			_capacity = 0;
			_metadata = null;
			_memoryMappedFile.Dispose();
		}
		#endregion

		#region 私有方法
		unsafe private AllocationRecord? GetRecord(int id)
		{
			var recordCount = _metadata.AllocationTableBuffer.Length / AllocationRecord.Length;

			if(id < recordCount)
				return this.GetRecord(_metadata.AllocationTableBuffer, id);

			int recordsPerBlock = this.BlockSize / AllocationRecord.Length;
			int recordAddressIndex = (id - recordCount) / recordsPerBlock;
			int recordIndex = (id - recordCount) % recordsPerBlock;

			fixed(byte* pAddressBuffer = _metadata.AllocationTableAddressBuffer)
			{
				int* pAddressIndex = (int*)(pAddressBuffer + (recordAddressIndex * 4));

				if(*pAddressIndex > 0)
				{
					var recordBuffer = new byte[this.BlockSize];

					if(this.ReadBlock(*pAddressIndex, 0, recordBuffer, 0, recordBuffer.Length) == recordBuffer.Length)
						return this.GetRecord(recordBuffer, recordIndex);
				}
			}

			//返回失败
			return null;
		}

		unsafe private AllocationRecord? GetRecord(byte[] buffer, int recordIndex)
		{
			int offset = recordIndex * AllocationRecord.Length;

			if(offset < 0 || offset > buffer.Length - AllocationRecord.Length)
				return null;

			AllocationRecord record;

			fixed(byte* pBuffer = buffer)
			{
				record.Head = *((int*)(pBuffer + offset));
				record.Size = *((uint*)(pBuffer + offset + 4));
				record.Timestamp = *((uint*)(pBuffer + offset + 8));
				record.Flags = *((uint*)(pBuffer + offset + 12));
			}

			return record;
		}

		unsafe private int AllocateRecord(long size, int blockId)
		{
			if(size < 1 || blockId < 0)
				return -1;

			//通过页面管理器获取指定块所在的页面，该方法内部确保始终能返回其所在的存储页面对象，注意：该方法始终进行线程同步。
			using(var page = _pageManager.GetPage(blockId))
			{
				int temp = 0;
				var blocks = (int)Math.Ceiling((double)size / this.BlockSize);
				var recordCount = this.BlockSize / AllocationRecord.Length;
				var recordBuffer = new byte[AllocationRecord.Length];

				//获取指定存储块所在缓存页的流，该方法内部确保将返回的流指针定位到对应的块起始位置并加上指定的偏移量。
				var stream = page.GetPageStream(blockId, 0, ref temp);

				for(int i = 0; i < recordCount; i++)
				{
					if(stream.Read(recordBuffer, 0, recordBuffer.Length) != recordBuffer.Length)
						continue;

					var flags = BitConverter.ToInt32(recordBuffer, 12);

					if(flags == AllocationRecord.Unallocated)
					{
						var head = this.AllocateBlocks(blocks);

						if(head > 0)
						{
							fixed(byte* pRecord = recordBuffer)
							{
								*(int*)pRecord = head;
								*(uint*)(pRecord + 4) = (uint)size;
								*(uint*)(pRecord + 8) = BufferUtility.GetTimestamp();
								*(int*)(pRecord + 12) = AllocationRecord.Allocated;
							}

							//在写入之前要先调整回读入前的位置
							stream.Position -= recordBuffer.Length;
							//将记录缓存数据写入流中
							stream.Write(recordBuffer, 0, recordBuffer.Length);

							//累加已经分配的缓存项数量
							Interlocked.Increment(ref _count);
						}

						return i;
					}
				}
			}

			return -1;
		}

		unsafe private int AllocateRecord(long size, byte[] recordBuffer)
		{
			if(recordBuffer == null || recordBuffer.Length == 0)
				throw new ArgumentNullException("recordBuffer");

			if(size < 1 || recordBuffer.Length < AllocationRecord.Length)
				return -1;

			var blocks = (int)Math.Ceiling((double)size / this.BlockSize);
			var recordCount = recordBuffer.Length / AllocationRecord.Length;

			fixed(byte* pBuffer = recordBuffer)
			{
				byte* pRecord;

				for(int i = 0; i < recordCount; i++)
				{
					pRecord = pBuffer + (i * AllocationRecord.Length);

					int flags = Interlocked.CompareExchange(ref *(int*)(pRecord + AllocationRecord.Length - 4), AllocationRecord.Allocated, AllocationRecord.Unallocated);

					if(flags == AllocationRecord.Unallocated)
					{
						var head = this.AllocateBlocks(blocks);

						if(head > 0)
						{
							*(int*)pRecord = head;
							*(uint*)(pRecord + 4) = (uint)size;
							*(uint*)(pRecord + 8) = BufferUtility.GetTimestamp();

							//累加已经分配的缓存项数量
							Interlocked.Increment(ref _count);
						}
						else
						{
							*(int*)(pRecord + 12) = AllocationRecord.Unallocated;
						}

						return i;
					}
				}
			}

			return -1;
		}

		unsafe private int ReleaseRecord(int id)
		{
			var recordCount = _metadata.AllocationTableBuffer.Length / AllocationRecord.Length;

			if(id < recordCount)
				return this.ReleaseRecord(id, _metadata.AllocationTableBuffer);

			int recordsPerBlock = this.BlockSize / AllocationRecord.Length;
			int recordAddressIndex = (id - recordCount) / recordsPerBlock;
			int recordIndex = (id - recordCount) % recordsPerBlock;

			fixed(byte* pAddressBuffer = _metadata.AllocationTableAddressBuffer)
			{
				int* pAddressIndex = (int*)(pAddressBuffer + (recordAddressIndex * 4));

				if(*pAddressIndex > 0)
				{
					var recordBuffer = new byte[this.BlockSize];

					if(this.ReadBlock(*pAddressIndex, 0, recordBuffer, 0, recordBuffer.Length) == recordBuffer.Length)
						return this.ReleaseRecord(recordIndex, recordBuffer);
				}
			}

			//返回失败
			return -1;
		}

		unsafe private int ReleaseRecord(int recordIndex, byte[] recordBuffer)
		{
			if(recordIndex < 0 || recordIndex >= (recordBuffer.Length / AllocationRecord.Length) || recordBuffer == null || recordBuffer.Length < AllocationRecord.Length)
				return -1;

			fixed(byte* pBuffer = recordBuffer)
			{
				byte* pRecord = pBuffer + (recordIndex * AllocationRecord.Length);
				int head = *(int*)pRecord;
				int flags = Interlocked.CompareExchange(ref *(int*)(pRecord + 12), AllocationRecord.Unallocated, AllocationRecord.Allocated);

				if(flags == AllocationRecord.Allocated)
				{
					Interlocked.Decrement(ref _count);
					return head;
				}
			}

			//返回失败
			return -1;
		}

		unsafe private int AllocateBlocks(int count)
		{
			if(count < 1)
				return -1;

			int head = -1;
			int previous = -1;

			fixed(byte* pIndexerBuffer = _metadata.IndexerBuffer)
			{
				//注意：首个存储块为内部保留不能非配使用，因此从1开始遍历
				for(int i = 1; i < _blockCount; i++)
				{
					//获取当前索引项的对应索引区缓存的指针位置
					int* pIndexer = (int*)(pIndexerBuffer + (i * 4));

					//如果当前的索引项的值为零，则将其改为-1；如果不为零，则不作改动。而无论该条件是否成立均返回其原始值。
					int indexValue = Interlocked.CompareExchange(ref *pIndexer, -1, 0);

					//如果当前索引值为零，则表示对应的存储块是空闲块
					if(indexValue == 0)
					{
						if(previous < 0)
							head = i; //保存头索引为当前索引号
						else
						{
							//获取上一个索引项的对应索引区缓存的指针位置
							int* pPrevious = (int*)(pIndexerBuffer + (previous * 4));

							//将上一个块索引项的值更改为本次的块号
							*pPrevious = i;
						}

						//更新前趋指针值为当前项
						previous = i;
						//递减待分配块数的计数器
						count--;

						//如果剩下的待分配的块数为零则表示当前分配结束，即跳出当前循环
						if(count == 0)
							break;
					}
				}

				//如果经过上面的分配仍然还剩下块未分完，则会滚上面的分配
				if(count > 0)
				{
					int current = head;

					while(current >= 0)
					{
						//获取当前索引项的对应索引区缓存的指针位置
						int* pCurrent = (int*)(pIndexerBuffer + (current * 4));

						//首先将当前索引项所指向的下个索引块序号保存起来
						current = *pCurrent;
						//将当前索引项的内容置空，表示其对应的索引块为空闲状态
						*pCurrent = 0;
					}

					return -1;
				}
			}

			return head;
		}

		unsafe private void ReleaseBlocks(int blockId)
		{
			if(blockId < 1)
				return;

			fixed(byte* pIndexerBuffer = _metadata.IndexerBuffer)
			{
				while(blockId > 0)
				{
					//获取当前索引项的对应索引区缓存的指针位置
					int* pCurrent = (int*)(pIndexerBuffer + (blockId * 4));

					//首先将当前索引项所指向的下个索引块序号保存起来
					blockId = *pCurrent;
					//将当前索引项的内容置空，表示其对应的索引块为空闲状态
					*pCurrent = 0;
				}
			}
		}

		private int ReadBlock(int blockId, int blockOffset, byte[] buffer, int offset, int count)
		{
			//通过页面管理器获取指定块所在的页面，该方法内部确保始终能返回其所在的存储页面对象，注意：该方法始终进行线程同步。
			using(var page = _pageManager.GetPage(blockId))
			{
				//获取指定存储块所在缓存页的流，该方法内部确保将返回的流指针定位到对应的块起始位置并加上指定的偏移量。
				var stream = page.GetPageStream(blockId, blockOffset, ref count);

				//确保要读取的长度不超出目标缓存区的可用长度
				count = Math.Min(count, buffer.Length - offset);

				//从指定的内存映射流中读取数据到输出缓存区中
				if(count > 0)
					return stream.Read(buffer, offset, Math.Min(count, buffer.Length - offset));

				//返回失败
				return 0;
			}
		}

		private int WriteBlock(int blockId, int blockOffset, byte[] buffer, int offset, int count)
		{
			//通过页面管理器获取指定块所在的页面，该方法内部确保始终能返回其所在的存储页面对象，注意：该方法始终进行线程同步。
			using(var page = _pageManager.GetPage(blockId))
			{
				//获取指定存储块所在缓存页的流，该方法内部确保将返回的流指针定位到对应的块起始位置并加上指定的偏移量。
				var stream = page.GetPageStream(blockId, blockOffset, ref count);

				//确保要读取的长度不超出目标缓存区的可用长度
				count = Math.Min(count, buffer.Length - offset);

				//从指定的输入缓存区内容写入到对应的存储流中
				if(count > 0)
					stream.Write(buffer, offset, count);

				//返回失败
				return count;
			}
		}
		#endregion

		#region 记录结构
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		private struct AllocationRecord
		{
			#region 常量定义
			public const int Allocated = 1;
			public const int Unallocated = 0;
			#endregion

			#region 静态字段
			public static readonly int Length = Marshal.SizeOf(typeof(AllocationRecord));
			public static readonly AllocationRecord Empty = new AllocationRecord();
			#endregion

			#region 公共字段
			public int Head;
			public uint Size;
			public uint Timestamp;
			public uint Flags; //0:Unallocated; 1:Allocated; 2:Reading; 4:Writing
			#endregion

			#region 构造函数
			internal AllocationRecord(int head, uint size, uint timestamp, uint flags)
			{
				this.Head = head;
				this.Size = size;
				this.Timestamp = timestamp;
				this.Flags = flags;
			}
			#endregion
		}
		#endregion

		#region 内部子类
		private class MetadataInfo
		{
			#region 常量定义
			internal const int HeadSize = 20;
			#endregion

			#region 成员变量
			private long _symbol;
			private int _version;
			#endregion

			#region 私有变量
			private readonly int _blockSize;
			private readonly int _blockCount;
			#endregion

			#region 公共字段
			/// <summary>数据存储区的索引块缓存。</summary>
			public readonly byte[] IndexerBuffer;
			/// <summary>分配表存储缓存。</summary>
			public readonly byte[] AllocationTableBuffer;
			/// <summary>间接分配表的地址缓存。</summary>
			public readonly byte[] AllocationTableAddressBuffer;
			#endregion

			#region 构造函数
			private MetadataInfo(long symbol, int version, int blockSize, int blockCount)
			{
				_symbol = symbol;
				_version = version;

				_blockSize = blockSize;
				_blockCount = blockCount;

				IndexerBuffer = new byte[_blockCount * 4];
				AllocationTableBuffer = new byte[32 * BufferUtility.KB];
				AllocationTableAddressBuffer = new byte[BufferUtility.KB * 4];
			}
			#endregion

			#region 公共属性
			public long Symbol
			{
				get
				{
					return _symbol;
				}
			}

			public int Version
			{
				get
				{
					return _version;
				}
			}

			public int IndexerOffset
			{
				get
				{
					return 20;
				}
			}

			public int AllocationTableOffset
			{
				get
				{
					return IndexerOffset + IndexerBuffer.Length;
				}
			}

			public int AllocationTableAddressOffset
			{
				get
				{
					return AllocationTableOffset + AllocationTableBuffer.Length;
				}
			}

			public int AllocationTableAddressCount
			{
				get
				{
					return AllocationTableAddressBuffer.Length / 4;
				}
			}

			/// <summary>
			/// 获取数据存储区相对于总存储区的偏移量，即数据存储区的起始位置。
			/// </summary>
			public int StorageOffset
			{
				get
				{
					return AllocationTableAddressOffset + AllocationTableAddressBuffer.Length;
				}
			}
			#endregion

			#region 公共方法
			unsafe internal int GetIndex(int index)
			{
				fixed(byte* pIndexer = IndexerBuffer)
				{
					return *(int*)(pIndexer + index * 4);
				}
			}

			/// <summary>
			/// 将当前元数据信息写入到指定的流中。
			/// </summary>
			/// <param name="stream">待写入的数据流。</param>
			public void Update(Stream stream)
			{
				if(stream == null)
					throw new ArgumentNullException("stream");

				//重置流指针位置
				stream.Position = 0;

				//定义缓存文件的头部
				byte[] head = new byte[MetadataInfo.HeadSize];

				Buffer.BlockCopy(BitConverter.GetBytes(_symbol), 0, head, 0, 8);
				Buffer.BlockCopy(BitConverter.GetBytes(_version), 0, head, 8, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(_blockSize), 0, head, 12, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(_blockCount), 0, head, 16, 4);

				//写入缓存文件的头部信息
				stream.Write(head, 0, head.Length);

				//写入索引区数据
				stream.Write(IndexerBuffer, 0, IndexerBuffer.Length);
				//写入直接分配表记录数据
				stream.Write(AllocationTableBuffer, 0, AllocationTableBuffer.Length);
				//写入间接分配表地址数据
				stream.Write(AllocationTableAddressBuffer, 0, AllocationTableAddressBuffer.Length);
			}
			#endregion

			#region 静态方法
			public static MetadataInfo Create(int blockSize, int blockCount)
			{
				return new MetadataInfo(BitConverter.ToInt64(new byte[] { (byte)'Z', (byte)'S', (byte)'B', (byte)'u', (byte)'f', (byte)'f', (byte)'e', (byte)'r' }, 0),
									   0x01000000, blockSize, blockCount);
			}

			public static MetadataInfo Load(Stream stream)
			{
				if(stream == null)
					throw new ArgumentNullException("stream");

				//重置流指针位置
				stream.Position = 0;

				//定义缓存文件的头部
				byte[] head = new byte[MetadataInfo.HeadSize];

				//读取缓存文件的头部内容
				if(stream.Read(head, 0, head.Length) != head.Length)
					throw new InvalidOperationException();

				var symbol = BitConverter.ToInt64(head, 0);
				var version = BitConverter.ToInt32(head, 8);
				var blockSize = BitConverter.ToInt32(head, 12);
				var blockCount = BitConverter.ToInt32(head, 16);

				//创建元数据信息对象
				var metadata = new MetadataInfo(symbol, version, blockSize, blockCount);

				//读取索引区数据
				stream.Read(metadata.IndexerBuffer, 0, metadata.IndexerBuffer.Length);
				//读取直接分配表记录数据
				stream.Read(metadata.AllocationTableBuffer, 0, metadata.AllocationTableBuffer.Length);
				//读取间接分配表地址数据
				stream.Read(metadata.AllocationTableAddressBuffer, 0, metadata.AllocationTableAddressBuffer.Length);

				//返回构建成功的元数据对象
				return metadata;
			}
			#endregion
		}

		private class MemoryMappedPageManager
		{
			#region 私有常量
			private readonly int PageSize;
			private readonly int BlocksPerPage;
			#endregion

			#region 私有变量
			private readonly BufferManager _bufferManager;
			private readonly MemoryMappedPage[] _pages;
			private readonly uint _storagePosition;
			#endregion

			#region 构造函数
			internal MemoryMappedPageManager(BufferManager bufferManager, uint storagePosition)
			{
				if(bufferManager == null)
					throw new ArgumentNullException("bufferManager");

				_bufferManager = bufferManager;
				_storagePosition = storagePosition;
				_pages = new MemoryMappedPage[BufferManager.ViewPageCount];
				this.PageSize = (int)(BufferManager.ViewPageTotalSize / BufferManager.ViewPageCount);
				this.BlocksPerPage = this.PageSize / _bufferManager.BlockSize;
			}
			#endregion

			#region 公共方法
			public MemoryMappedPage GetPage(int blockId)
			{
				if(blockId < 0)
					throw new ArgumentOutOfRangeException("blockId");

				int pageIndex = blockId / this.BlocksPerPage;
				int index = 0;

				lock(_pages)
				{
					//在页面队列中查找指定页面号的页面对象，如果找到则直接返回
					for(; index < _pages.Length; index++)
					{
						if(_pages[index] == null)
							break;

						if(pageIndex == _pages[index].PageIndex)
							return _pages[index];
					}

					//获取即将要彻底销毁的页面对象
					var page = _pages[Math.Min(index, _pages.Length - 1)];

					//如果即将要销毁的页面对象不为空则彻底销毁它
					if(page != null)
						page.Kill();

					//以下循环移动待页面队列，以腾出位置给新页面对象
					for(int i = Math.Min(index, _pages.Length - 1); i > 0; i--)
					{
						_pages[i] = _pages[i - 1];
					}

					return _pages[0] = new MemoryMappedPage(this, pageIndex, _bufferManager.Storage.CreateViewStream(_storagePosition + (pageIndex * PageSize), PageSize));
				}
			}
			#endregion

			#region 嵌套子类
			public class MemoryMappedPage : IDisposable
			{
				#region 同步变量
				private readonly object _syncRoot;
				#endregion

				#region 成员字段
				private int _pageIndex;
				private Stream _pageStream;
				private MemoryMappedPageManager _manager;
				#endregion

				#region 构造函数
				internal MemoryMappedPage(MemoryMappedPageManager manager, int pageIndex, Stream pageStream)
				{
					if(manager == null)
						throw new ArgumentNullException("manager");

					if(pageIndex < 0)
						throw new ArgumentOutOfRangeException("pageIndex");

					if(pageStream == null)
						throw new ArgumentNullException("pageStream");

					_syncRoot = new object();
					_manager = manager;
					_pageIndex = pageIndex;
					_pageStream = pageStream;
				}
				#endregion

				#region 公共属性
				public int PageIndex
				{
					get
					{
						return _pageIndex;
					}
				}
				#endregion

				#region 公共方法
				/// <summary>
				/// 获取指定的存储块所在的分页的数据流。
				/// </summary>
				/// <param name="blockId"></param>
				/// <param name="blockOffset"></param>
				/// <param name="count"></param>
				/// <returns>返回指定存储块所在的分页流，返回流的当前指针位于指定的块所在起始位置。</returns>
				public Stream GetPageStream(int blockId, int blockOffset, ref int count)
				{
					if(_pageStream == null)
						throw new ObjectDisposedException(this.GetType().FullName);

					if(blockId < 0)
						throw new ArgumentOutOfRangeException("blockId");

					if(blockOffset < 0 || blockOffset >= _manager._bufferManager.BlockSize)
						throw new ArgumentOutOfRangeException("blockOffset");

					//以下代码确保写入的长度不会超出当前块的范围
					if(count > 0)
						count = Math.Min(_manager._bufferManager.BlockSize - blockOffset, count);
					else
						count = _manager._bufferManager.BlockSize - blockOffset;

					//对当前页进行锁定，必须使用本类的Dispose来解锁
					Monitor.Enter(_syncRoot);

					//更新当前页面的数据流的当前指针
					_pageStream.Position = (blockId % _manager.BlocksPerPage) * _manager._bufferManager.BlockSize + blockOffset;

					//返回准备好的页面数据流
					return _pageStream;
				}

				/// <summary>
				/// 该方法并非销毁内部资源，它仅释放对当前页面的线程同步锁。
				/// </summary>
				/// <remarks>
				///		如果要彻底销毁本页面对象，请使用<see cref="Kill"/>方法。
				/// </remarks>
				public void Dispose()
				{
					Monitor.Exit(_syncRoot);
				}
				#endregion

				#region 内部方法
				internal void Kill()
				{
					lock(_syncRoot)
					{
						_pageStream.Dispose();
						_pageStream = null;
						_manager = null;
					}
				}
				#endregion
			}
			#endregion
		}

		private class BufferViewStream : Stream
		{
			#region 成员变量
			private int _id;
			private long _position;
			private BufferManager _manager;
			private AllocationRecord _record;
			#endregion

			#region 构造函数
			internal BufferViewStream(int id, AllocationRecord record, BufferManager manager)
			{
				if(manager == null)
					throw new ArgumentNullException("manager");

				if(record.Flags == 0)
					throw new InvalidOperationException();

				_id = id;
				_record = record;
				_manager = manager;
				_position = 0;
			}
			#endregion

			#region 公共属性
			public override bool CanRead
			{
				get
				{
					if(_id < 0)
						return false;

					return true;
				}
			}

			public override bool CanSeek
			{
				get
				{
					if(_id < 0)
						return false;

					return true;
				}
			}

			public override bool CanWrite
			{
				get
				{
					if(_id < 0)
						return false;

					return true;
				}
			}

			public override long Length
			{
				get
				{
					if(_id < 0)
						throw new ObjectDisposedException(this.GetType().FullName);

					return _record.Size;
				}
			}

			public override long Position
			{
				get
				{
					if(_id < 0)
						throw new ObjectDisposedException(this.GetType().FullName);

					return _position;
				}
				set
				{
					if(_id < 0)
						throw new ObjectDisposedException(this.GetType().FullName);

					if(value < 0 || value > _record.Size)
						throw new ArgumentOutOfRangeException();

					_position = value;
				}
			}
			#endregion

			#region 公共方法
			public override long Seek(long offset, SeekOrigin origin)
			{
				if(_id < 0)
					throw new ObjectDisposedException(this.GetType().FullName);

				switch(origin)
				{
					case SeekOrigin.Begin:
						this.Position = offset;
						break;
					case SeekOrigin.Current:
						this.Position = _position + offset;
						break;
					case SeekOrigin.End:
						this.Position = _record.Size - 1 - offset;
						break;
				}

				return _position;
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Flush()
			{
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if(_id < 0)
					throw new ObjectDisposedException(this.GetType().FullName);

				if(offset + count > buffer.Length)
					throw new ArgumentOutOfRangeException();

				if(count < 1)
					return 0;

				//记录当前流的指针位置
				var position = _position;
				//计算当前流指针位置对应的存储块号
				int blockIndex = (int)(position / _manager.BlockSize);
				//计算当前流指针位置对应的存储块的偏移量
				int blockOffset = (int)(position % _manager.BlockSize);
				//初始化操作的存储块序号
				int blockId = _record.Head;

				unsafe
				{
					fixed(byte* pIndexer = _manager._metadata.IndexerBuffer)
					{
						//查找当前流位置所对应的存储块
						for(int i = 0; i < blockIndex; i++)
						{
							blockId = *(int*)(pIndexer + blockId * 4);
						}
					}
				}

				//如果定位当前存储块失败则返回
				if(blockId < 0)
					return 0;

				int total = 0;
				int bytesRead = 0;

				//确保最大读取长度不会超出分配的实际大小
				int length = Math.Min(count, (int)(_record.Size - position));

				while(length > 0 && (bytesRead = _manager.ReadBlock(blockId, blockOffset, buffer, offset, length)) > 0)
				{
					total += bytesRead;
					position += (uint)bytesRead;

					//更新当前流的指针位置
					Interlocked.Exchange(ref _position, position);

					//如果累计已经读取了指定的数量或者对当前块的读取不能完成(即说明当前块是尾块)则返回
					if(total == count || blockOffset + bytesRead < _manager.BlockSize)
						return total;

					//获取下一个存储块的序号
					blockId = _manager._metadata.GetIndex(blockId);

					//如果获取存储块失败则直接退出
					if(blockId < 0)
						return total;

					//增加目标缓存指针位置
					offset += bytesRead;
					//减少可读出的长度
					length -= bytesRead;

					//始终重置块内偏移量
					blockOffset = 0;
				}

				return total;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if(_id < 0)
					throw new ObjectDisposedException(this.GetType().FullName);

				if(offset + count > buffer.Length)
					throw new ArgumentOutOfRangeException();

				if(count < 1)
					return;

				//记录当前流的指针位置
				var position = _position;
				//计算当前流指针位置对应的存储块号
				int blockIndex = (int)Math.Floor((double)position / _manager.BlockSize);
				//计算当前流指针位置对应的存储块的偏移量
				int blockOffset = (int)(position % _manager.BlockSize);
				//初始化操作的存储块序号
				int blockId = _record.Head;

				unsafe
				{
					fixed(byte* pIndexer = _manager._metadata.IndexerBuffer)
					{
						//查找当前流位置所对应的存储块
						for(int i = 0; i < blockIndex; i++)
						{
							blockId = *(int*)(pIndexer + blockId * 4);
						}
					}
				}

				//如果定位当前存储块失败则返回
				if(blockId < 0)
					return;

				int total = 0;
				int bytesWrote = 0;

				//确保最大写入长度不会超出分配的实际大小
				int length = Math.Min(count, (int)(_record.Size - position));

				while(length > 0 && (bytesWrote = _manager.WriteBlock(blockId, blockOffset, buffer, offset, length)) > 0)
				{
					total += bytesWrote;
					position += (uint)bytesWrote;

					//更新当前流的指针位置
					Interlocked.Exchange(ref _position, position);

					//如果累计已经读取了指定的数量或者对当前块的读取不能完成(即说明当前块是尾块)则返回
					if(total == count || blockOffset + bytesWrote < _manager.BlockSize)
						return;

					//获取下一个存储块的序号
					blockId = _manager._metadata.GetIndex(blockId);

					//如果获取存储块失败则直接退出
					if(blockId < 0)
						return;

					//增加目标缓存指针位置
					offset += bytesWrote;
					//减少可读出的长度
					length -= bytesWrote;

					//始终重置块内偏移量
					blockOffset = 0;
				}
			}

			protected override void Dispose(bool disposing)
			{
				if(_id < 0)
					return;

				//释放当前缓存
				_manager.Release(_id);

				//重置状态
				_id = -1;
				_position = 0;

				//调用基类同名方法
				base.Dispose(disposing);
			}
			#endregion
		}
		#endregion

		#region 静态方法
		private static readonly ConcurrentDictionary<string, BufferManager> _items = new ConcurrentDictionary<string, BufferManager>(StringComparer.OrdinalIgnoreCase);

		public static BufferManager GetBufferManager(string name)
		{
			return GetBufferManager(name, null);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public static BufferManager GetBufferManager(string name, string directoryPath)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			return _items.GetOrAdd(name.Trim(), key => new BufferManager(GetCacheFilePath(key, directoryPath)));
		}

		private static string GetCacheFilePath(string name, string directoryPath)
		{
			name = string.IsNullOrWhiteSpace(name) ? "Zongsoft.BufferManager" : name.Trim();
			directoryPath = string.IsNullOrWhiteSpace(directoryPath) ? Path.GetTempPath() : directoryPath.Trim();

			var directory = new DirectoryInfo(directoryPath);
			var files = directory.GetFiles(name + "*.cache");
			var regex = new System.Text.RegularExpressions.Regex(name.Replace(".", @"\.") + @"(#(?<no>\d+))\.cache");

			int number = 0;

			foreach(var file in files)
			{
				try
				{
					file.Delete();
					return file.FullName;
				}
				catch
				{
					var match = regex.Match(file.Name);
					if(match.Success)
						number = Math.Max(number, int.Parse(match.Groups["no"].Value));
				}
			}

			return Path.Combine(directory.FullName, string.Format("{0}#{1}.cache", name, number + 1));
		}
		#endregion
	}
}
