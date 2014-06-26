#region File Header

// Authors:
//    钟峰(Popeye Zhong) <zongsoft@gmail.com>
//    邓祥云(X.Z. Deng) <627825056@qq.com>
//  
// Copyright (C) 2011-2013 Zongsoft Corporation <http://www.zongsoft.com>
// 
// This file is part of Zongsoft.CoreLibrary.
// 
// Zongsoft.CoreLibrary is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with Zongsoft.CoreLibrary; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA

#endregion

#region Using

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zongsoft.Common;
using Zongsoft.Runtime.Caching;
using Buffer = Zongsoft.Common.Buffer;

#endregion

namespace Zongsoft.Communication.Net
{
    public class FtpPacketizer : IPacketizer, IDisposable
    {

        #region 私有变量

        private readonly byte[] _terminator = new byte[] {13, 10};
        private int _prevMatched;
        private MemoryStream _bufferStream;

        private Encoding _encoding;
        private BufferEvaluator _bufferEvaluator;

        #endregion

        #region 构造函数

        public FtpPacketizer()
        {
            _encoding = Encoding.ASCII;
            _bufferStream = new MemoryStream(1024);
            _bufferEvaluator = BufferEvaluator.Default;
        }

        #endregion

        #region 公共属性

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value ?? Encoding.ASCII; }
        }

        #endregion

        #region 打包方法

        public IEnumerable<Buffer> Pack(Buffer buffer)
        {
            yield return buffer;
        }

        public IEnumerable<Buffer> Pack(Stream stream)
        {
            int bufferSize = _bufferEvaluator.GetBufferSize(stream.Length - stream.Position);
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                var result = new Buffer(buffer, 0, bytesRead);
                buffer = new byte[bufferSize];
                yield return result;
            }
        }

        #endregion

        #region 解包方法

        public IEnumerable<object> Unpack(Buffer buffer)
        {
            int index;

            while ((index = SearchTerminatorToEnd(buffer.Value, buffer.Position, buffer.Count - buffer.Position)) >= 0)
            {
                buffer.Read(_bufferStream, index);
                var bytes = new byte[_bufferStream.Position - _terminator.Length];
                _bufferStream.Position = 0;
                _bufferStream.Read(bytes, 0, bytes.Length);
                _bufferStream.Position = 0;

                yield return this.Resolve(_encoding.GetString(bytes));
            }

            buffer.Read(_bufferStream, buffer.Count);
        }

        #endregion

        #region 私有方法

        private FtpStatement Resolve(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var parts = text.Split(new[] {' '}, 2);
            return new FtpStatement(parts[0].ToUpper(), parts.Length > 1 ? parts[1] : string.Empty);
        }

        /// <summary>
        /// 查找终结符结束位置
        /// 返回小于0表示未结束，否则为终结符结束后的索引(即为下一次数据的启始索引)
        /// </summary>
        /// <param name="array">数据缓冲区</param>
        /// <param name="startIndex">偏移位置</param>
        /// <param name="count">数据长度</param>
        /// <returns>如果在 array 中从 startIndex 开始、包含 count 所指定的元素个数的这部分元素中，找到终结符，则为终结符结束位置的索引；否则为-1。</returns>
        private int SearchTerminatorToEnd(byte[] array, int startIndex, int count)
        {
            var pos = startIndex;
            var end = startIndex + count;

            label_start:

            //查找起始位置
            if (_prevMatched == 0)
            {
                pos = Array.IndexOf(array, _terminator[0], pos, end - pos);
                if (pos < 0)
                    return -1;
            }

            while (true)
            {
                //匹配完全则返回当前索引(当前索引为结束后的索引)
                if (_prevMatched >= _terminator.Length)
                {
                    _prevMatched = 0;
                    return pos;
                }

                //数据已到结束位置
                if (pos >= end)
                {
                    break;
                }

                //匹配符号不相同则重新查找
                if (array[pos] != _terminator[_prevMatched])
                {
                    _prevMatched = 0;
                    goto label_start;
                }

                pos++;
                _prevMatched++;
            }

            return -1;
        }

        #endregion

        #region 公共方法

        public void Reset()
        {
            _bufferStream.Position = 0;
        }

        public void Dispose()
        {
            if (_bufferStream != null)
                _bufferStream.Close();
        }

        #endregion
    }
}