using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Options.Configuration.Samples
{
	[Obsolete("不需要了，请使用 Zongsoft.Options.Configuration.SettingElement 标准实现。")]
	public class SettingOptionElement : OptionConfigurationElement
	{
		[OptionConfigurationProperty("name", OptionConfigurationPropertyBehavior.IsKey)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
			set
			{
				this["name"] = value;
			}
		}

		[OptionConfigurationProperty("value")]
		public string Value
		{
			get
			{
				return (string)this["value"];
			}
			set
			{
				this["value"] = value;
			}
		}
	}
}
