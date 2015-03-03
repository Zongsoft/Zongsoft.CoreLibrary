using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Options.Configuration.Samples
{
	public class AdvanceOptionElement : OptionConfigurationElement
	{
		#region 公共属性
		[OptionConfigurationProperty("name", "")]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
			set
			{
				this["name"] = value ?? string.Empty;
			}
		}
		#endregion
	}
}
