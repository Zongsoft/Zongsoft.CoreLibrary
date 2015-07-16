using System;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Runtime.Serialization
{
	public class TextSerializationSettings : SerializationSettings
	{
		#region 成员字段
		private bool _isIndented;
		private SerializationNamingConvention _namingConvention;
		#endregion

		#region 构造函数
		public TextSerializationSettings()
		{
			_isIndented = false;
			_namingConvention = SerializationNamingConvention.None;
		}
		#endregion

		#region 公共属性
		public bool IsIndented
		{
			get
			{
				return _isIndented;
			}
			set
			{
				this.SetPropertyValue(() => this.IsIndented, ref _isIndented, value);
			}
		}

		public SerializationNamingConvention NamingConvention
		{
			get
			{
				return _namingConvention;
			}
			set
			{
				this.SetPropertyValue(() => this.NamingConvention, ref _namingConvention, value);
			}
		}
		#endregion
	}
}
