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
			var conditional = Entity.Build<IDummyConditional>();

			Assert.Null(conditional.CorporationId);
			Assert.Null(conditional.DepartmentId);
			Assert.Null(conditional.Name);
			Assert.Null(conditional.CreatedTime);

			Assert.Null(conditional.ToCondition());
			Assert.Null(conditional.ToConditions());

			conditional.CorporationId = 0;
			conditional.DepartmentId = null;
			Assert.NotNull(conditional.CorporationId);
			Assert.Null(conditional.DepartmentId);
			Assert.Equal(0, conditional.CorporationId);

			conditional.CreatedTime = new ConditionalRange<DateTime>(new DateTime(2010, 1, 1), null);
			Assert.NotNull(conditional.CreatedTime);
			Assert.Equal(new DateTime(2010, 1, 1), conditional.CreatedTime.From);

			var conditions = conditional.ToConditions();
			Assert.NotNull(conditions);
			Assert.True(conditions.Count > 0);
		}

		public interface IDummyConditional : IConditional
		{
			int? CorporationId
			{
				get; set;
			}

			short? DepartmentId
			{
				get; set;
			}

			[Conditional("Name", "PinYin")]
			string Name
			{
				get; set;
			}

			ConditionalRange<DateTime> CreatedTime
			{
				get; set;
			}
		}
	}
}
