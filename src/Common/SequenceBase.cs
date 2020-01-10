/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2015 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Threading;
using System.Collections.Generic;

namespace Zongsoft.Common
{
    public abstract class SequenceBase : ISequence
    {
        #region 内部结构
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct Entry
        {
            public long Value;      //当前序号值
            public long Threshold;  //本地递增的上限值
            public long Timestamp;  //最后更新时间，频率为秒
            public int Count;       //成长因子，自小为10
            public int Flags;       //同步锁标记

            public bool TrySet(int interval, out long value)
            {
                value = Interlocked.Add(ref Value, interval);

                if(interval > 0)
                    return value <= this.Threshold;
                else
                    return value >= this.Threshold;
            }
        }
        #endregion

        #region 常量定义
        private const int LOCKED_FLAG = 1;
        private const int UNLOCK_FLAG = 0;
        #endregion

        #region 成员字段
        private Dictionary<string, int> _map;
        private Entry[] _entries;
        #endregion

        #region 构造函数
        protected SequenceBase() : this(64)
        {
        }

        protected SequenceBase(int capacity)
        {
            if(capacity < 8)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _map = new Dictionary<string, int>(capacity);
            _entries = new Entry[capacity];
        }
        #endregion

        #region 公共方法
        public bool TryGetValue(string key, out long value)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if(_map.TryGetValue(key, out var index))
            {
                value = _entries[index].Value;
                return true;
            }

            value = 0;
            return false;
        }

        public virtual long Decrement(string key, int interval = 1, int seed = 0)
        {
            return this.Increment(key, -interval, seed);
        }

        public virtual long Increment(string key, int interval = 1, int seed = 0)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            int index;

            if(!_map.TryGetValue(key, out index))
            {
                lock(_map)
                {
                    if(!_map.TryGetValue(key, out index))
                    {
                        var count = _map.Count;

                        _map.Add(key, count);

                        if(count == _entries.Length)
                            Expand();
                    }
                }
            }

            unsafe
            {
                fixed(Entry* entry = &_entries[index])
                {
                    var result = Interlocked.Increment(ref entry->Value);

                    if(result >= entry->Threshold)
                    {
                        var hold = Interlocked.CompareExchange(ref entry->Flags, LOCKED_FLAG, UNLOCK_FLAG);

                        if(hold == 0)
                        {
                            try
                            {
                                if(result >= entry->Threshold)
                                {
                                    entry->Threshold = this.Reserve(key, entry->Timestamp, ref entry->Count, seed);
                                    entry->Timestamp = GetTimestamp();
                                }
                            }
                            finally
                            {
                                Interlocked.Exchange(ref entry->Flags, UNLOCK_FLAG);
                            }
                        }
                    }

                    return result;
                }
            }
        }

        public virtual void Reset(string key, int value = 0)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if(_map.TryGetValue(key, out var index))
            {
                unsafe
                {
                    fixed(Entry* entry = &_entries[index])
                    {
                        var hold = Interlocked.CompareExchange(ref entry->Flags, LOCKED_FLAG, UNLOCK_FLAG);

                        if(hold == 0)
                        {
                            try
                            {
                                entry->Value = value;
                                entry->Count = 10;
                                entry->Threshold = value + 10;
                                entry->Timestamp = GetTimestamp();

                                this.OnReset(key, value);
                            }
                            finally
                            {
                                Interlocked.Exchange(ref entry->Flags, UNLOCK_FLAG);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 抽象方法
        protected abstract void OnReset(string key, int value);
        protected abstract long OnReserve(string key, int count, int seed);
        #endregion

        #region 私有方法
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void Expand()
        {
            int capacity = _entries.Length == 0 ? 16 : _entries.Length * 2;

            if((uint)capacity > 0X7FEFFFFF)
                capacity = 0X7FEFFFFF;

            if(capacity < 16)
                capacity = 16;

            var entries = new Entry[capacity];

            if(_entries.Length > 0)
                Array.Copy(_entries, entries, _entries.Length);

            _entries = entries;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private long Reserve(string key, long lastest, ref int count, int seed)
        {
            var duration = GetTimestamp() - lastest;
            count = (int)Math.Max(10, count * Math.Max(0.1, 2 - (duration / 300)));
            return this.OnReserve(key, count, seed);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private long GetTimestamp()
        {
            return System.Diagnostics.Stopwatch.GetTimestamp() / System.Diagnostics.Stopwatch.Frequency;
        }
        #endregion
    }
}
