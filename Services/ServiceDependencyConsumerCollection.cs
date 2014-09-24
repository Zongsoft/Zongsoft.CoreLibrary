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
	[Obsolete]
	public class ServiceDependencyConsumerCollection : IEnumerable<ServiceDependencyConsumer>
	{
		private ServiceDependencyContainer _container;
		private List<ServiceDependencyConsumer> _innerList;

		public ServiceDependencyConsumerCollection(ServiceDependencyContainer container)
		{
			if(container == null)
				throw new ArgumentNullException("container");

			_container = container;
		}

		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}

		public ServiceDependencyConsumer Register(Type serviceType, IPredication predication, Action<ServiceDependencyNotifiedEventArgs> onNotified)
		{
			var consumer = new ServiceDependencyConsumer(_container, serviceType, predication, onNotified);
			_innerList.Add(consumer);
			return consumer;
		}

		public void Unregister(ServiceDependencyConsumer consumer)
		{
			if(consumer == null)
				return;

			if(_innerList.Contains(consumer))
			{
				_innerList.Remove(consumer);
			}
		}

		public IEnumerator<ServiceDependencyConsumer> GetEnumerator()
		{
			foreach(var consumer in _innerList)
				yield return consumer;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach(var consumer in _innerList)
				yield return consumer;
		}
	}
}
