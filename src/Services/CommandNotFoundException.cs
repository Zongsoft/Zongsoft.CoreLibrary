using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Services
{
	public class CommandNotFoundException : CommandException
	{
		#region 成员字段
		private string _path;
		#endregion

		#region 构造函数
		public CommandNotFoundException(string path)
		{
			_path = path ?? string.Empty;
		}
		#endregion

		#region 公共属性
		public string Path
		{
			get
			{
				return _path;
			}
		}
		#endregion
	}
}
