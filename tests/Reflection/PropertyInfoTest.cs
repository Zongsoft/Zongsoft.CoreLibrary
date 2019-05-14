using System;
using System.Linq;
using System.Reflection;

using Xunit;

using Zongsoft.Reflection;

namespace Zongsoft.Reflection
{
	public class PropertyInfoTest
	{
		[Fact]
		public void Test()
		{
			var entity = new Entity()
			{
				Id = 100,
				Name = "Popeye",
				Birthdate = new DateTime(2000, 12, 31),
			};

			var property = typeof(Entity).GetProperty(nameof(Entity.Id));
			var getter = property.GenerateGetter();
			var setter = property.GenerateSetter();

			Assert.NotNull(getter);
			Assert.NotNull(setter);

			var target = (object)entity;
			var value = getter(ref target);
			Assert.Equal(100, value);

			setter(ref target, 200);
			value = getter(ref target);
			Assert.Equal(200, value);

			property = typeof(Entity).GetProperty(nameof(Entity.Age));
			getter = property.GetGetter();
			setter = property.GetSetter();

			Assert.NotNull(getter);
			Assert.Null(setter);
			value = getter(ref target);
			Assert.True((int)Convert.ChangeType(value, typeof(int)) > 0);
		}

		#region 嵌套子类
		public class Entity
		{
			public int Id
			{
				get; set;
			}

			public string Name
			{
				get; set;
			}

			public int Age
			{
				get
				{
					var birthdate = this.Birthdate;
					return birthdate.HasValue ? (int)Math.Ceiling((DateTime.Today - birthdate.Value).TotalDays / 365) : 0;
				}
			}

			public DateTime? Birthdate
			{
				get; set;
			}

			public static Entity Instance
			{
				get; set;
			}
		}
		#endregion
	}
}
