using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data
{
	public class ConditionalTest
	{
		[Fact]
		public void Test()
		{
			var conditional = new DummyConditional();

			Assert.Null(conditional.CorporationId);
			Assert.Null(conditional.DepartmentId);
			Assert.Null(conditional.Name);
			Assert.Null(conditional.CreatedTime);
			Assert.Null(conditional.ToConditions());

			conditional.CorporationId = 0;
			conditional.DepartmentId = null;
			Assert.NotNull(conditional.CorporationId);
			Assert.Null(conditional.DepartmentId);
			Assert.Equal(0, conditional.CorporationId);
		}

		public class DummyConditional : Conditional
		{
			public int? CorporationId
			{
				get
				{
					return this.GetPropertyValue(() => this.CorporationId);
				}
				set
				{
					this.SetPropertyValue(() => this.CorporationId, value);
				}
			}

			public short? DepartmentId
			{
				get
				{
					return this.GetPropertyValue(() => this.DepartmentId);
				}
				set
				{
					this.SetPropertyValue(() => this.DepartmentId, value);
				}
			}

			[Conditional("Name", "PinYin")]
			public string Name
			{
				get
				{
					return this.GetPropertyValue(() => this.Name);
				}
				set
				{
					this.SetPropertyValue(() => this.Name, value);
				}
			}

			public ConditionalRange<DateTime> CreatedTime
			{
				get
				{
					return this.GetPropertyValue(() => this.CreatedTime);
				}
				set
				{
					this.SetPropertyValue(() => this.CreatedTime, value);
				}
			}
		}
	}
}
