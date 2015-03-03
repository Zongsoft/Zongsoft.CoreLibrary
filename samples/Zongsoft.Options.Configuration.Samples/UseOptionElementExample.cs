using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zongsoft.Options.Configuration.Samples
{
	public class UseOptionElementExample
	{
		#region 成员字段
		private GeneralOptionElement _general;
		private Zongsoft.Options.IOptionProvider _options;
		#endregion

		#region 构造函数
		public UseOptionElementExample(Zongsoft.Options.IOptionProvider options)
		{
			_options = options;
		}
		#endregion

		#region 公共属性
		public GeneralOptionElement General
		{
			get
			{
				return _general;
			}
			//set
			//{
			//	_general = value;
			//}
		}
		#endregion

		#region 重写方法
		public override string ToString()
		{
			if(_general == null)
				return base.ToString();

			StringBuilder text = new StringBuilder(_general.GetType().FullName);
			text.AppendLine();
			text.AppendFormat("Int32Property = {0}", _general.Int32Property);
			text.AppendLine();

			text.AppendFormat("DateTimeProperty = {0}", _general.DateTimeProperty);
			text.AppendLine();

			text.AppendFormat("TextProperty = {0}", _general.TextProperty);
			text.AppendLine();

			foreach(CmdletOptionElement cmdlet in _general.Cmdlets)
			{
				text.AppendFormat("Cmdlet({0}[{1}])" + Environment.NewLine + "{2}", cmdlet.Name, cmdlet.CommandType, cmdlet.Text);
				text.AppendLine();
			}

			if(_general.Settings.Count > 0)
			{
				text.AppendLine();
				text.AppendLine("***** General.Settings[" + _general.Settings.Count + "] *****");

				foreach(SettingOptionElement setting in _general.Settings)
				{
					text.AppendFormat("{0} = {1}", setting.Name, setting.Value);
					text.AppendLine();
				}
			}

			var options = _options;

			if(options != null)
			{
				var settings = options.GetOptionObject("/Settings") as Zongsoft.Options.Configuration.SettingElementCollection;

				if(settings != null)
				{
					text.AppendLine();
					text.AppendLine();
					text.AppendLine("##### Global Settings[" + _general.Settings.Count + "] #####");

					foreach(SettingOptionElement setting in _general.Settings)
					{
						text.AppendFormat("{0} = {1}", setting.Name, setting.Value);
						text.AppendLine();
					}
				}
			}

			return text.ToString();
		}
		#endregion
	}
}
