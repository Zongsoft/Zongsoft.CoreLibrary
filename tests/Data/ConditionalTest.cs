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
			ConditionCollection conditions = null;

			Assert.Null(conditional.CorporationId);
			Assert.Null(conditional.DepartmentId);
			Assert.Null(conditional.Name);
			Assert.Null(conditional.CreatedTime);
			Assert.Null(conditional.ToConditions());

			conditional.CorporationId = 0;
			conditional.DepartmentId = null;
			Assert.NotNull(conditional.CorporationId);
			Assert.NotNull(conditional.DepartmentId);
			Assert.Equal(0, conditional.CorporationId);
			Assert.Null(conditional.DepartmentId.Value);

			conditions = conditional.ToConditions();
			Assert.Equal(2, conditions.Count);
			Assert.Equal("CorporationId", ((Condition)conditions[0]).Name);
			Assert.Equal(0, ((Condition)conditions[0]).Value);
			Assert.Equal("DepartmentId", ((Condition)conditions[1]).Name);
			Assert.Null(((Condition)conditions[1]).Value);
		}

		public class DummyConditional : Conditional
		{
			public ConditionalValue<int?> CorporationId
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

			public ConditionalValue<short?> DepartmentId
			{
				get
				{
					return this.GetPropertyValue(() => this.DepartmentId);
				}
				set
				{
					this.SetPropertyValue(() => this.DepartmentId, (ConditionalValue<short?>)value);
				}
			}

			[Conditional("Name", "PinYin")]
			public ConditionalValue<string> Name
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

			public ConditionalRange CreatedTime
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
