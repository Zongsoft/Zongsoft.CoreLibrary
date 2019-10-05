using System;

namespace Zongsoft.Samples.Models
{
	internal struct PropertyToken<T>
	{
		public PropertyToken(int ordinal, Func<T, object> getter, Action<T, object> setter)
		{
			this.Ordinal = ordinal;
			this.Getter = getter;
			this.Setter = setter;
		}

		public readonly int Ordinal;
		public readonly Func<T, object> Getter;
		public readonly Action<T, object> Setter;
	}
}
