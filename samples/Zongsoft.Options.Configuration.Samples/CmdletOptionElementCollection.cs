using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Options.Configuration.Samples
{
	public class CmdletOptionElementCollection : OptionConfigurationElementCollection
	{
		protected override string ElementName
		{
			get
			{
				return "cmdlet";
			}
		}

		protected override OptionConfigurationElement CreateNewElement()
		{
			return new CmdletOptionElement();
		}

		protected override string GetElementKey(OptionConfigurationElement element)
		{
			return ((CmdletOptionElement)element).Name;
		}
	}
}
