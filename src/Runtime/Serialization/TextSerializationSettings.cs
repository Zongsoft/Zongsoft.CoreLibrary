using System;
using System.Collections.Generic;
using System.Text;

namespace Zongsoft.Runtime.Serialization
{
	public class TextSerializationSettings : SerializationSettings
	{
		#region 成员字段
		private bool _indented;
		private bool _typed;
		private SerializationNamingConvention _namingConvention;
		#endregion

		#region 构造函数
		public TextSerializationSettings()
		{
			_indented = false;
			_namingConvention = SerializationNamingConvention.None;
		}
		#endregion

		#region 公共属性
		/// <summary>
		/// 获取或设置一个值，指示序列化后的文本是否保持缩进风格。
		/// </summary>
		public bool Indented
		{
			get
			{
				return _indented;
			}
			set
			{
				_indented = value;
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示序列化的文本是否保持强类型信息。
		/// </summary>
		public bool Typed
		{
			get
			{
				return _typed;
			}
			set
			{
				_typed = value;
			}
		}

		/// <summary>
		/// 获取或设置一个值，指示序列化成员的命名转换方式。
		/// </summary>
		public SerializationNamingConvention NamingConvention
		{
			get
			{
				return _namingConvention;
			}
			set
			{
				_namingConvention = value;
			}
		}
		#endregion
	}
}
