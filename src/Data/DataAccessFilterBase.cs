/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2010-2017 Zongsoft Corporation <http://www.zongsoft.com>
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
using System.Collections.Generic;

namespace Zongsoft.Data
{
	public abstract class DataAccessFilterBase : IDataAccessFilter, Zongsoft.Services.IPredication<DataAccessContextBase>
	{
		#region 成员字段
		private string[] _names;
		private DataAccessMethod[] _methods;
		#endregion

		#region 构造函数
		protected DataAccessFilterBase(params string[] names)
		{
			_names = names;
		}

		protected DataAccessFilterBase(DataAccessMethod method, params string[] names)
		{
			_methods = new DataAccessMethod[] { method };
			_names = names;
		}

		protected DataAccessFilterBase(IEnumerable<DataAccessMethod> methods, params string[] names)
		{
			if(methods != null)
				_methods = methods.ToArray();

			_names = names;
		}
		#endregion

		#region 保护属性
		protected virtual Zongsoft.Security.CredentialPrincipal Principal
		{
			get
			{
				return Zongsoft.ComponentModel.ApplicationContextBase.Current.Principal as Zongsoft.Security.CredentialPrincipal;
			}
		}
		#endregion

		#region 过滤方法
		protected virtual void OnFiltered(DataAccessContextBase context)
		{
			switch(context.Method)
			{
				case DataAccessMethod.Count:
					this.OnCounted((DataCountContext)context);
					break;
				case DataAccessMethod.Execute:
					this.OnExecuted((DataExecutionContext)context);
					break;
				case DataAccessMethod.Exists:
					this.OnExisted((DataExistenceContext)context);
					break;
				case DataAccessMethod.Increment:
					this.OnIncremented((DataIncrementContext)context);
					break;
				case DataAccessMethod.Select:
					this.OnSelected((DataSelectionContext)context);
					break;
				case DataAccessMethod.Delete:
					this.OnDeleted((DataDeletionContext)context);
					break;
				case DataAccessMethod.Insert:
					this.OnInserted((DataInsertionContext)context);
					break;
				case DataAccessMethod.Update:
					this.OnUpdated((DataUpdationContext)context);
					break;
				case DataAccessMethod.Upsert:
					this.OnUpserted((DataUpsertionContext)context);
					break;
			}
		}

		protected virtual void OnFiltering(DataAccessContextBase context)
		{
			switch(context.Method)
			{
				case DataAccessMethod.Count:
					this.OnCounting((DataCountContext)context);
					break;
				case DataAccessMethod.Execute:
					this.OnExecuting((DataExecutionContext)context);
					break;
				case DataAccessMethod.Exists:
					this.OnExisting((DataExistenceContext)context);
					break;
				case DataAccessMethod.Increment:
					this.OnIncrementing((DataIncrementContext)context);
					break;
				case DataAccessMethod.Select:
					this.OnSelecting((DataSelectionContext)context);
					break;
				case DataAccessMethod.Delete:
					this.OnDeleting((DataDeletionContext)context);
					break;
				case DataAccessMethod.Insert:
					this.OnInserting((DataInsertionContext)context);
					break;
				case DataAccessMethod.Update:
					this.OnUpdating((DataUpdationContext)context);
					break;
				case DataAccessMethod.Upsert:
					this.OnUpserting((DataUpsertionContext)context);
					break;
			}
		}

		void IDataAccessFilter.OnFiltered(DataAccessContextBase context)
		{
			this.OnFiltered(context);
		}

		void IDataAccessFilter.OnFiltering(DataAccessContextBase context)
		{
			this.OnFiltering(context);
		}
		#endregion

		#region 虚拟方法
		protected virtual void OnCounting(DataCountContext context)
		{
		}

		protected virtual void OnCounted(DataCountContext context)
		{
		}

		protected virtual void OnExecuting(DataExecutionContext context)
		{
		}

		protected virtual void OnExecuted(DataExecutionContext context)
		{
		}

		protected virtual void OnExisting(DataExistenceContext context)
		{
		}

		protected virtual void OnExisted(DataExistenceContext context)
		{
		}

		protected virtual void OnIncrementing(DataIncrementContext context)
		{
		}

		protected virtual void OnIncremented(DataIncrementContext context)
		{
		}

		protected virtual void OnSelecting(DataSelectionContext context)
		{
		}

		protected virtual void OnSelected(DataSelectionContext context)
		{
		}

		protected virtual void OnDeleting(DataDeletionContext context)
		{
		}

		protected virtual void OnDeleted(DataDeletionContext context)
		{
		}

		protected virtual void OnInserting(DataInsertionContext context)
		{
		}

		protected virtual void OnInserted(DataInsertionContext context)
		{
		}

		protected virtual void OnUpdating(DataUpdationContext context)
		{
		}

		protected virtual void OnUpdated(DataUpdationContext context)
		{
		}

		protected virtual void OnUpserting(DataUpsertionContext context)
		{
		}

		protected virtual void OnUpserted(DataUpsertionContext context)
		{
		}
		#endregion

		#region 断言方法
		public virtual bool Predicate(DataAccessContextBase context)
		{
			var result = true;

			if(result && (_methods != null && _methods.Length > 0))
				result &= _methods.Contains(context.Method);

			if(result && (_names != null && _names.Length > 0))
				result &= _names.Contains(context.Name, StringComparer.OrdinalIgnoreCase);

			return result;
		}

		bool Zongsoft.Services.IPredication.Predicate(object parameter)
		{
			var context = parameter as DataAccessContextBase;

			if(context == null)
				return false;
			else
				return this.Predicate(context);
		}
		#endregion
	}
}
