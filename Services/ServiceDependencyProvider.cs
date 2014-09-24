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
	public class ServiceDependencyProvider
	{
		private ServiceDependencyContainer _container;
		private object _instance;
		private Type[] _contractTypes;

		public ServiceDependencyProvider(object instance, Type contractType) : this(instance, new Type[]{ contractType })
		{
			if(contractType == null)
				throw new ArgumentNullException("contractType");
		}

		public ServiceDependencyProvider(object instance, Type[] contractTypes)
		{
			if(instance == null)
				throw new ArgumentNullException("instance");

			if(contractTypes == null)
				throw new ArgumentNullException("contractTypes");

			_instance = instance;
			_contractTypes = contractTypes;
		}

		public void Notify(string changeName, object newValue, object oldValue)
		{
			var consumers = _container.Consumers;

			if(consumers == null || consumers.Count < 1)
				return;

			var notifiedArgs = new ServiceDependencyNotifiedEventArgs(_instance, changeName, newValue, oldValue);

			foreach(var consumer in consumers)
			{
				foreach(var contractType in _contractTypes)
				{
					if(consumer.ServiceType == contractType)
					{
						if(consumer.Predication != null && consumer.Predication.Predicate(notifiedArgs))
							consumer.OnNotified(notifiedArgs);
					}
				}
			}
		}
	}
}
