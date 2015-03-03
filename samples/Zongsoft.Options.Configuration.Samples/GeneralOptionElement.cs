using System;
using System.Collections.Generic;

using Zongsoft.Options;
using Zongsoft.Options.Configuration;

namespace Zongsoft.Options.Configuration.Samples
{
	public class GeneralOptionElement : OptionConfigurationElement
	{
		#region 静态字段
		//private static OptionConfigurationPropertyCollection _properties;
		#endregion

		#region 静态构造
		//static GeneralOptionObject()
		//{
		//    var textProperty = new OptionConfigurationProperty("text", typeof(string), string.Empty);
		//    var datetimeProperty = new OptionConfigurationProperty("datetime", typeof(DateTime), new DateTime(1900, 1, 1));
		//    var int32Property = new OptionConfigurationProperty("int", typeof(int));

		//    _properties = new OptionConfigurationPropertyCollection();
		//    _properties.Add(textProperty);
		//    _properties.Add(datetimeProperty);
		//    _properties.Add(int32Property);
		//}
		#endregion

		#region 构造函数
		public GeneralOptionElement()
		{
		}
		#endregion

		//protected override OptionConfigurationPropertyCollection Properties
		//{
		//    get
		//    {
		//        return _properties;
		//    }
		//}

		#region 公共属性
		[OptionConfigurationProperty("text", "")]
		public string TextProperty
		{
			get
			{
				return (string)this["text"];
			}
			set
			{
				this["text"] = value ?? string.Empty;
			}
		}

		[OptionConfigurationProperty("datetime", "1900/1/1")]
		public DateTime DateTimeProperty
		{
			get
			{
				return (DateTime)this["datetime"];
			}
			set
			{
				this["datetime"] = value;
			}
		}

		[OptionConfigurationProperty("int")]
		public int Int32Property
		{
			get
			{
				return (int)this["int"];
			}
			set
			{
				this["int"] = value;
			}
		}

		[OptionConfigurationProperty("cmdlets")]
		public CmdletOptionElementCollection Cmdlets
		{
			get
			{
				return (CmdletOptionElementCollection)this["cmdlets"];
			}
		}

		[OptionConfigurationProperty("", ElementName = "setting")]
		public OptionConfigurationElementCollection<SettingOptionElement> Settings
		{
			get
			{
				return (OptionConfigurationElementCollection<SettingOptionElement>)this[""];
			}
		}
		#endregion

		#region 重写方法
		protected override string GetMappedPropertyName(string name)
		{
			switch(name.ToLowerInvariant())
			{
				case "text":
					return "TextProperty";
				case "int":
					return "Int32Property";
				case "datetime":
					return "DateTimeProperty";
				default:
					return base.GetMappedPropertyName(name);
			}
		}
		#endregion
	}
}
