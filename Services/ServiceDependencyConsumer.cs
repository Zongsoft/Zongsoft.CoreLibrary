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
using System.Collections.Generic;

namespace Zongsoft.Services
{
	public class ServiceDependencyConsumer
	{
		private ServiceDependencyContainer _container;
		private object _consumer;
		private Type _serviceType;
		private IPredication _predication;
		private Action<ServiceDependencyNotifiedEventArgs> _onNotified;

		public ServiceDependencyConsumer(ServiceDependencyContainer container, Type serviceType, IPredication predication, Action<ServiceDependencyNotifiedEventArgs> onNotified)
		{
			if(container == null)
				throw new ArgumentNullException("container");

			if(serviceType == null)
				throw new ArgumentNullException("serviceType");

			_container = container;
			_serviceType = serviceType;
			_predication = predication;
			_onNotified = onNotified;
		}

		public Type ServiceType
		{
			get
			{
				return _serviceType;
			}
		}

		public IPredication Predication
		{
			get
			{
				return _predication;
			}
		}

		public Action<ServiceDependencyNotifiedEventArgs> OnNotified
		{
			get
			{
				return _onNotified;
			}
		}
	}

	public class ServiceDependencyNotifiedEventArgs
	{
		private object _notifier;
		private string _changeName;
		private object _newValue;
		private object _oldValue;

		public ServiceDependencyNotifiedEventArgs(object notifier, string changeName, object newValue, object oldValue)
		{
			_notifier = notifier;
			_changeName = changeName;
			_newValue = newValue;
			_oldValue = oldValue;
		}

		public object Notifier
		{
			get
			{
				return _notifier;
			}
		}

		public string ChangeName
		{
			get
			{
				return _changeName;
			}
		}

		public object NewValue
		{
			get
			{
				return _newValue;
			}
		}

		public object OldValue
		{
			get
			{
				return _oldValue;
			}
		}
	}
}
