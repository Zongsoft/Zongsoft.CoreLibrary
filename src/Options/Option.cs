/*
 * Authors:
 *   �ӷ�(Popeye Zhong) <zongsoft@gmail.com>
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
using System.ComponentModel;
using System.Collections.Generic;

namespace Zongsoft.Options
{
	public class Option : IOption
	{
		#region �¼�����
		public event EventHandler Changed;
		public event EventHandler Applied;
		public event EventHandler Resetted;
		public event CancelEventHandler Applying;
		public event CancelEventHandler Resetting;
		#endregion

		#region ��Ա����
		private OptionNode _node;
		private object _optionObject;
		private bool _isDirty;
		private IOptionView _view;
		private IOptionViewBuilder _viewBuilder;
		private readonly ICollection<IOptionProvider> _providers;
		#endregion

		#region ���캯��
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

		#region ��������
		/// <summary>
		/// ��ȡ��ǰѡ������ơ�
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
				if(_optionObject == null && _providers != null)
				{
					INotifyPropertyChanged notifiable = _optionObject as INotifyPropertyChanged;
					if(notifiable != null)
						notifiable.PropertyChanged -= new PropertyChangedEventHandler(OptionObject_PropertyChanged);

					foreach(var provider in _providers)
					{
						_optionObject = provider.GetOptionValue(_node.FullPath);

						if(_optionObject != null)
							break;
					}

					notifiable = _optionObject as INotifyPropertyChanged;
					if(notifiable != null)
						notifiable.PropertyChanged += new PropertyChangedEventHandler(OptionObject_PropertyChanged);

					//���µ�ǰѡ������ݣ��Ա����õ�ʱ����л�ԭ֮��
					OptionHelper.UpdateOptionObject(_node.FullPath, _optionObject);
				}

				return _optionObject;
			}
		}
		#endregion

		#region ��������
		public void Reset()
		{
			//����ڲ�ѡ�����Ϊ�գ����ʾ��ûʹ�ù�ѡ��ʴ�������б�����
			if(_optionObject == null)
				return;

			//������CancelEventArgs���¼���������
			CancelEventArgs cancelArgs = new CancelEventArgs();
			//������Resetting���¼�����ʾReset������������
			this.OnResetting(cancelArgs);
			//�����CancelEventArgs���¼��������󷵻�ȡ�������ʾȡ����������
			if(cancelArgs.Cancel)
				return;

			//ִ�����ö���
			this.OnReset();

			//��������״̬Ϊδ�ı�
			_isDirty = false;

			//������Resetted���¼�����ʾReset����ִ�����
			this.OnResetted(EventArgs.Empty);
		}

		public void Apply()
		{
			//����ڲ�ѡ�����Ϊ�գ����ʾ��ûʹ�ù�ѡ��ʴ�������б�����
			if(_optionObject == null)
				return;

			//������CancelEventArgs���¼���������
			CancelEventArgs cancelArgs = new CancelEventArgs();
			//������Applying���¼�����ʾApply������������
			this.OnApplying(cancelArgs);
			//�����CancelEventArgs���¼��������󷵻�ȡ�������ʾȡ����������
			if(cancelArgs.Cancel)
				return;

			//ִ��Ӧ�ö���
			this.OnApply();

			//��������״̬Ϊδ�ı�
			_isDirty = false;

			//���µ�ǰѡ������ݣ��Ա����õ�ʱ����л�ԭ֮��
			OptionHelper.UpdateOptionObject(_node.FullPath, _optionObject);

			//������Applied���¼�����ʾApply����ִ�����
			this.OnApplied(EventArgs.Empty);
		}
		#endregion

		#region ���ⷽ��
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

		#region �����¼�
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

		#region ˽�з���
		private void OptionObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//��������״̬Ϊ�Ķ�
			_isDirty = true;

			//������Changed���¼�
			this.OnChanged(EventArgs.Empty);
		}
		#endregion
	}
}