using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Options.Configuration.Samples
{
	public class CmdletOptionElement : OptionConfigurationElement
	{
		[OptionConfigurationProperty("name", "")]
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

		[OptionConfigurationProperty("type", CmdletType.Text)]
		public CmdletType CommandType
		{
			get
			{
				return (CmdletType)this["type"];
			}
			set
			{
				this["type"] = value;
			}
		}

		[OptionConfigurationProperty("text", "")]
		public string Text
		{
			get
			{
				return (string)this["text"];
			}
			set
			{
				this["text"] = value;
			}
		}
	}
}
