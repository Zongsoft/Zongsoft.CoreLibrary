/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2005-2008 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

namespace Zongsoft.Options
{
	public class Option : IOption
	{
		#region 事件定义
		public event EventHandler Changed;
		public event EventHandler Applied;
		public event EventHandler Resetted;
		public event CancelEventHandler Applying;
		public event CancelEventHandler Resetting;
		#endregion

		#region 成员变量
		private OptionNode _node;
		private object _optionObject;
		private bool _isDirty;
		private IOptionView _view;
		private IOptionViewBuilder _viewBuilder;
		private readonly ICollection<IOptionProvider> _providers;
		#endregion

		#region 构造函数
		public Option(OptionNode node) : this(node, null, (IOptionView)null)
		{
		}

		public Option(OptionNode node, IOptionProvider provider) : this(node, provider, (IOptionView)null)
		{
		}

		public Option(OptionNode node, IOptionProvider provider, IOptionView view)
		{
			_node = node ?? throw new ArgumentNullException(nameof(node));
			_providers = new List<IOptionProvider>() { provider };
			_view = view;
			_isDirty = false;
		}

		public Option(OptionNode node, IOptionProvider provider, IOptionViewBuilder viewBuilder)
		{
			_node = node ?? throw new ArgumentNullException(nameof(node));
			_providers = new List<IOptionProvider>() { provider };
			_viewBuilder = viewBuilder;
			_isDirty = false;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取当前选项的名称。
		/// </summary>
		public string Name
		{
			get
			{
				return _node.Name;
			}
		}

		public virtual bool IsDirty
		{
			get
			{
				if(_optionObject == null)
					return false;

				return _isDirty;
			}
		}

		public IOptionView View
		{
			get
			{
				if(_viewBuilder != null)
				{
					if(!_viewBuilder.IsValid(_view))
						_view = _viewBuilder.GetView(this);
				}

				return _view;
			}
			set
			{
				_view = value;
			}
		}

		public IOptionViewBuilder ViewBuilder
		{
			get
			{
				return _viewBuilder;
			}
			set
			{
				_viewBuilder = value;
			}
		}

		public ICollection<IOptionProvider> Providers
		{
			get
			{
				return _providers;
			}
		}

		public virtual object OptionObject
		{
			get
			{
				if(_providers.Count > 1)
				{
					Type contractType = null, collectionType = null;
					IList<object> list = new List<object>(_providers.Count);

					foreach(var provider in _providers)
					{
						var option = provider.GetOptionValue(_node.FullPath);

						if(option == null)
							continue;

						list.Add(option);

						if(collectionType == null)
						{
							var contracts = option.GetType().GetInterfaces()
								.Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>) && type.GetGenericArguments()[0] == typeof(string))
								.ToArray();

							if(contracts == null || contracts.Length == 0)
								throw new InvalidOperationException($"The '{_node.FullPath}' option has multiple providers, but the option objects for the provider are not dictionary, so they cannot be combined.");

							if(contracts.Length == 1)
								contractType = contracts[0];
							else
							{
								foreach(var contract in contracts)
								{
									if(contractType == null || contractType.GetGenericArguments()[1].IsAssignableFrom(contract.GetGenericArguments()[1]))
										contractType = contract;
								}
							}

							collectionType = typeof(MultiresultDictionary<>).MakeGenericType(contractType.GetGenericArguments()[1]);
						}
						else
						{
							if(!contractType.IsAssignableFrom(option.GetType()))
								throw new InvalidOperationException($"The '{_node.FullPath}' option has multiple providers, but these providers have different option object types, so they cannot be combined.");
						}
					}

					return Activator.CreateInstance(collectionType, new object[] { _node, list });
				}

				if(_optionObject == null)
				{
					INotifyPropertyChanged notifiable = _optionObject as INotifyPropertyChanged;
					if(notifiable != null)
						notifiable.PropertyChanged -= new PropertyChangedEventHandler(OptionObject_PropertyChanged);

					if(_providers.Count == 0)
						return _optionObject = null;

					if(_providers.Count == 1)
						_optionObject = _providers.FirstOrDefault().GetOptionValue(_node.FullPath);

					notifiable = _optionObject as INotifyPropertyChanged;
					if(notifiable != null)
						notifiable.PropertyChanged += new PropertyChangedEventHandler(OptionObject_PropertyChanged);

					//更新当前选项的数据，以备重置的时候进行还原之用
					OptionHelper.UpdateOptionObject(_node.FullPath, _optionObject);
				}

				return _optionObject;
			}
		}
		#endregion

		#region 公共方法
		public void Reset()
		{
			//如果内部选项对象为空，则表示从没使用过选项故此无需进行本操作
			if(_optionObject == null)
				return;

			//创建“CancelEventArgs”事件参数对象
			CancelEventArgs cancelArgs = new CancelEventArgs();
			//激发“Resetting”事件，表示Reset操作即将进行
			this.OnResetting(cancelArgs);
			//如果“CancelEventArgs”事件参数对象返回取消，则表示取消后续操作
			if(cancelArgs.Cancel)
				return;

			//执行重置动作
			this.OnReset();

			//设置数据状态为未改变
			_isDirty = false;

			//激发“Resetted”事件，表示Reset操作执行完成
			this.OnResetted(EventArgs.Empty);
		}

		public void Apply()
		{
			//如果内部选项对象为空，则表示从没使用过选项故此无需进行本操作
			if(_optionObject == null)
				return;

			//创建“CancelEventArgs”事件参数对象
			CancelEventArgs cancelArgs = new CancelEventArgs();
			//激发“Applying”事件，表示Apply操作即将进行
			this.OnApplying(cancelArgs);
			//如果“CancelEventArgs”事件参数对象返回取消，则表示取消后续操作
			if(cancelArgs.Cancel)
				return;

			//执行应用动作
			this.OnApply();

			//设置数据状态为未改变
			_isDirty = false;

			//更新当前选项的数据，以备重置的时候进行还原之用
			OptionHelper.UpdateOptionObject(_node.FullPath, _optionObject);

			//激发“Applied”事件，表示Apply操作执行完成
			this.OnApplied(EventArgs.Empty);
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnReset()
		{
			OptionHelper.RejectOptionObject(_node.FullPath, _optionObject);
		}

		protected virtual void OnApply()
		{
			foreach(var provider in _providers)
				provider.SetOptionValue(_node.FullPath, _optionObject);
		}
		#endregion

		#region 激发事件
		protected virtual void OnChanged(EventArgs args)
		{
			if(Changed != null)
				Changed(this, args);
		}

		protected virtual void OnApplied(EventArgs args)
		{
			if(Applied != null)
				Applied(this, args);
		}

		protected virtual void OnApplying(CancelEventArgs args)
		{
			if(Applying != null)
				Applying(this, args);
		}

		protected virtual void OnResetted(EventArgs args)
		{
			if(Resetted != null)
				Resetted(this, args);
		}

		protected virtual void OnResetting(CancelEventArgs args)
		{
			if(Resetting != null)
				Resetting(this, args);
		}
		#endregion

		#region 私有方法
		private void OptionObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//设置数据状态为改动
			_isDirty = true;

			//激发“Changed”事件
			this.OnChanged(EventArgs.Empty);
		}
		#endregion

		#region 嵌套子类
		private class MultiresultDictionary<T> : IReadOnlyDictionary<string, T>, Collections.IReadOnlyNamedCollection<T>, IEnumerable<T>
		{
			private readonly OptionNode _node;
			private readonly IEnumerable<IReadOnlyDictionary<string, T>> _items;

			public MultiresultDictionary(OptionNode node, IEnumerable items)
			{
				_node = node;
				_items = items.Cast<IReadOnlyDictionary<string, T>>();
			}

			public int Count
			{
				get => _items.Sum(p => p.Count);
			}

			public IEnumerable<string> Keys
			{
				get
				{
					foreach(var item in _items)
					{
						foreach(var key in item.Keys)
							yield return key;
					}
				}
			}

			public IEnumerable<T> Values
			{
				get
				{
					foreach(var item in _items)
					{
						foreach(var value in item.Values)
							yield return value;
					}
				}
			}

			public T this[string key]
			{
				get => this.Get(key);
			}

			public bool Contains(string key)
			{
				return this.ContainsKey(key);
			}

			public bool ContainsKey(string key)
			{
				foreach(var item in _items)
				{
					if(item.ContainsKey(key))
						return true;
				}

				return false;
			}

			public T Get(string name)
			{
				foreach(var item in _items)
				{
					if(item.TryGetValue(name, out var value))
						return value;
				}

				throw new KeyNotFoundException($"The specified '{name}' key does not exist in the '{_node.FullPath}' option.");
			}

			public bool TryGet(string name, out T value)
			{
				return this.TryGetValue(name, out value);
			}

			public bool TryGetValue(string key, out T value)
			{
				foreach(var item in _items)
				{
					if(item.TryGetValue(key, out value))
						return true;
				}

				value = default(T);
				return false;
			}

			public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
			{
				foreach(var item in _items)
				{
					var iterator = item.GetEnumerator();

					while(iterator.MoveNext())
					{
						yield return iterator.Current;
					}
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				foreach(var item in _items)
				{
					foreach(var value in item.Values)
						yield return value;
				}
			}
		}
		#endregion
	}
}