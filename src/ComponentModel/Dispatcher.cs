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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Zongsoft.Collections;
using Zongsoft.ComponentModel;

namespace Zongsoft.ComponentModel
{
	public class Dispatcher<TKey> : IDispatcher<TKey>
	{
		#region 事件定义
		public event EventHandler<DispatcherRepliedEventArgs> Replied;
		#endregion

		#region 成员变量
		private IDispatchProvider _provider;
		private DispatchStateCollection _dispatching;
		#endregion

		#region 构造函数
		protected Dispatcher(IDispatchProvider provider)
		{
			_provider = provider;
			_dispatching = new DispatchStateCollection(this);

			//foreach(var channel in _channels)
			//{
			//    channel.Dispatched += new EventHandler<DispatchEventArgs>(Channel_Dispatched);
			//}
		}

		private void Channel_Dispatched(object sender, DispatchEventArgs e)
		{
			IAssociable<TKey> associable = e.Data as IAssociable<TKey>;
			if(associable == null)
				return;

			var key = associable.GetAssociatedKey();

			//if(e.Succeed)
			//    _dispatching[key].Channels.Remove((IDispatch)sender);
			//else
			//    _dispatching[key].Channels[(IDispatch)sender] = true;

			if(_dispatching[key].Channels.Count < 1)
				this.Reply(associable);
		}
		#endregion

		#region 公共属性
		public DispatchStateCollection Dispatching
		{
			get
			{
				return _dispatching;
			}
		}
		#endregion

		#region 抽象方法
		public virtual void Dispatch(object data)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			var dispatchs = _provider.GetDispatchs();

			foreach(var dispatch in dispatchs)
			{
				dispatch.Dispatch(data);
			}

			//首先将通讯包加入缓存器中
			//_cache.Enqueue(data);

			//IRelatedObject

			//Package package;
			//IServiceProviderFactory factory;
			//var provider = factory.GetProvider("Comunication");
			//ISender sender = provider.Resolve<ISender>(package);
			//sender.Send(package);
		}
		#endregion

		#region 公共方法
		public void Dispatch(IObjectIdentity<TKey> data, object owner)
		{
			if(data == null)
				throw new ArgumentNullException("data");

			//if(owner != null)
			//    _dispatching.Add(new DispatchState(data.IdentityId, data, owner, _channels));

			this.Dispatch(data);
		}
		#endregion

		#region 数据分发
		private void Dispatch(DispatchState state)
		{
			if(state == null)
				return;

			//foreach(var channel in _channels)
			//{
			//    channel.Dispatch(state.SourceData);
			//}
		}
		#endregion

		#region 保护方法
		protected void ResetDispatching(TimeSpan duration)
		{
			foreach(var item in _dispatching)
			{
				if(item.DispactedDuration > duration)
					this.RaiseReplied(item.Key, new DispatcherRepliedEventArgs(item.SourceData, DispatcherRepliedReason.Reset));
			}
		}

		protected void Reply(IAssociable<TKey> repliedData)
		{
			//如果返回的数据为空或者“Replied”事件为空则直接退出
			if(repliedData == null || this.Replied == null)
				return;

			//获取返回对象的关联对象的键值
			var associatedKey = repliedData.GetAssociatedKey();

			//回调当前返回数据对应的所有回调方法
			RaiseReplied(associatedKey, new DispatcherRepliedEventArgs(repliedData, DispatcherRepliedReason.Reply));
		}
		#endregion

		#region 私有方法
		private void RaiseReplied(TKey associatedKey, DispatcherRepliedEventArgs args)
		{
			try
			{
				if(_dispatching.Contains(associatedKey))
				{
					object owner = _dispatching[associatedKey].SourceOwner;

					foreach(var callback in this.Replied.GetInvocationList())
					{
						if(callback.Target == owner)
							callback.DynamicInvoke(this, args);
					}
				}
			}
			finally
			{
				_dispatching.Remove(associatedKey);
			}
		}
		#endregion

		#region 嵌套子类
		public class DispatchState
		{
			#region 成员变量
			private TKey _key;
			private object _sourceData;
			private object _sourceOwner;
			private DateTime _dispatchTime;
			private KeyedCollection<IDispatch, ChannelState> _channels;
			#endregion

			#region 构造函数
			internal DispatchState(TKey key, object data, object owner, IEnumerable<IDispatch> channels)
			{
				if(owner == null)
					throw new ArgumentNullException("owner");

				_key = key;
				_sourceData = data;
				_sourceOwner = owner;
				_dispatchTime = DateTime.Now;
				//_channels = new KeyedCollection<IDispatch, ChannelState>(channels);
			}
			#endregion

			#region 公共属性
			public TKey Key
			{
				get
				{
					return _key;
				}
			}

			public object SourceData
			{
				get
				{
					return _sourceData;
				}
			}

			public object SourceOwner
			{
				get
				{
					return _sourceOwner;
				}
			}

			public DateTime DispatchTime
			{
				get
				{
					return _dispatchTime;
				}
			}

			public TimeSpan DispactedDuration
			{
				get
				{
					return DateTime.Now - _dispatchTime;
				}
			}

			public KeyedCollection<IDispatch, ChannelState> Channels
			{
				get
				{
					return _channels;
				}
			}
			#endregion

			public struct ChannelState
			{
				public IDispatch Channel;
				public bool IsCompleted;
				public object UserToken;
			}
		}

		public class DispatchStateCollection : ICollection<DispatchState>
		{
			#region 成员变量
			private Dispatcher<TKey> _dispatcher;
			private IDictionary<TKey, DispatchState> _items;
			#endregion

			#region 构造函数
			internal DispatchStateCollection(Dispatcher<TKey> dispatcher)
			{
				if(dispatcher == null)
					throw new ArgumentNullException("dispatcher");

				_dispatcher = dispatcher;
				_items = new Dictionary<TKey, DispatchState>();
			}
			#endregion

			#region 公共属性
			public DispatchState this[TKey key]
			{
				get
				{
					return _items[key];
				}
			}

			public int Count
			{
				get
				{
					return _items.Count;
				}
			}

			public ICollection<TKey> Keys
			{
				get
				{
					return _items.Keys;
				}
			}

			public ICollection<DispatchState> Values
			{
				get
				{
					return _items.Values;
				}
			}

			public Dispatcher<TKey> Dispatcher
			{
				get
				{
					return _dispatcher;
				}
			}
			#endregion

			#region 显式实现
			bool ICollection<DispatchState>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			void ICollection<DispatchState>.Add(DispatchState item)
			{
				throw new NotSupportedException();
			}

			void ICollection<DispatchState>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<DispatchState>.Remove(DispatchState item)
			{
				throw new NotSupportedException();
			}

			bool ICollection<DispatchState>.Contains(DispatchState item)
			{
				if(item == null)
					return false;
				else
					return _items.ContainsKey(item.Key);
			}

			void ICollection<DispatchState>.CopyTo(DispatchState[] array, int arrayIndex)
			{
				throw new NotSupportedException();
			}
			#endregion

			#region 公共方法
			public bool Contains(TKey key)
			{
				return _items.ContainsKey(key);
			}
			#endregion

			#region 内部方法
			internal void Add(DispatchState item)
			{
				if(item == null)
					throw new ArgumentNullException("item");

				_items[item.Key] = item;
			}

			internal void Remove(TKey key)
			{
				_items.Remove(key);
			}
			#endregion

			#region 枚举实现
			public IEnumerator<DispatchState> GetEnumerator()
			{
				foreach(var item in _items)
					yield return item.Value;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
			#endregion
		}
		#endregion
	}
}
