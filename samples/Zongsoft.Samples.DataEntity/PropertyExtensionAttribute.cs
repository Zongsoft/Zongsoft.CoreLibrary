using System;

namespace Zongsoft.Samples.DataEntity
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class PropertyExtensionAttribute : Attribute
	{
		#region 构造函数
		public PropertyExtensionAttribute(Type type)
		{
			this.Type = type ?? throw new ArgumentNullException(nameof(type));
		}
		#endregion

		#region 公共属性
		public Type Type
		{
			get;
		}
		#endregion
	}
}
