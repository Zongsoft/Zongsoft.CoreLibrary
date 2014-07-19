using System;
using System.Collections.Generic;

namespace Zongsoft.Services
{
	public abstract class CommandLoaderBase : ICommandLoader
	{
		#region 同步变量
		private readonly object _syncRoot = new object();
		#endregion

		#region 成员字段
		private bool _isLoaded;
		#endregion

		#region 公共属性
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
		}
		#endregion

		#region 公共方法
		public void Load(CommandTreeNode node)
		{
			if(node == null || _isLoaded)
				return;

			lock(_syncRoot)
			{
				if(_isLoaded)
					return;

				_isLoaded = this.OnLoad(node);
			}
		}
		#endregion

		#region 抽象方法
		/// <summary>
		/// 执行加载命令的实际操作。
		/// </summary>
		/// <param name="node">待加载的命令树节点。</param>
		/// <returns>如果加载成功则返回真(true)，否则返回假(false)。</returns>
		protected abstract bool OnLoad(CommandTreeNode node);
		#endregion
	}
}
