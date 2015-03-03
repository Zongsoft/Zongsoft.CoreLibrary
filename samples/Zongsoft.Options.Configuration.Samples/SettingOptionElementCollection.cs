using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Options.Configuration.Samples
{
	[Obsolete("不需要了，请使用 Zongsoft.Options.Configuration.SettingElementCollection 标准实现。")]
	public class SettingOptionElementCollection : OptionConfigurationElementCollection<SettingOptionElement>
	{
		public SettingOptionElementCollection() : base("setting")
		{
		}

		//protected override OptionConfigurationElement CreateNewElement()
		//{
		//    return new SettingOptionElement();
		//}

		//protected override string GetElementKey(OptionConfigurationElement element)
		//{
		//    return ((SettingOptionElement)element).Key;
		//}
	}
}
