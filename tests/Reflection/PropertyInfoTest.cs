using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Reflection
{
	public class PropertyInfoTest
	{
		[Fact]
		public void TestClassEntity()
		{
			var entity = new ClassEntity()
			{
				Id = 100,
				Name = "Popeye",
			};

			//init the target variable
			var target = (object)entity;

			//test the instance property(get and set)
			var property = typeof(ClassEntity).GetProperty(nameof(ClassEntity.Id));
			var getter = property.GenerateGetter();
			var setter = property.GenerateSetter();

			Assert.NotNull(getter);
			Assert.NotNull(setter);

			var value = getter(ref target);
			Assert.Equal(100, value);

			setter(ref target, 200);
			value = getter(ref target);
			Assert.Equal(200, value);

			property = typeof(ClassEntity).GetProperty(nameof(ClassEntity.Birthdate));
			getter = property.GenerateGetter();
			setter = property.GenerateSetter();

			value = getter(ref target);
			Assert.Null(value);

			setter(ref target, new DateTime(2000, 12, 31));
			value = getter(ref target);
			Assert.Equal(new DateTime(2000, 12, 31), value);

			//test the readonly property
			property = typeof(ClassEntity).GetProperty(nameof(ClassEntity.Age));
			getter = property.GetGetter();
			setter = property.GetSetter();

			Assert.NotNull(getter);
			Assert.Null(setter);
			value = getter(ref target);
			Assert.True((int)Convert.ChangeType(value, typeof(int)) > 0);

			//test the static property
			property = typeof(ClassEntity).GetProperty(nameof(ClassEntity.Instance));
			getter = property.GetGetter();
			setter = property.GetSetter();
			Assert.NotNull(getter);
			Assert.NotNull(setter);

			value = getter(ref target);
			Assert.Null(value);

			target = null;
			value = getter(ref target);
			Assert.Null(value);

			value = new ClassEntity();
			setter(ref target, value);
			var newValue = getter(ref target);
			Assert.Equal(value, newValue);

			//reset the target
			target = entity;

			//test the indexer property
			var members = typeof(ClassEntity).GetDefaultMembers();
			property = (System.Reflection.PropertyInfo)members[0];
			getter = property.GetGetter();
			setter = property.GetSetter();
			Assert.NotNull(getter);
			Assert.NotNull(setter);

			Assert.Null(getter(ref target, 0));
			Assert.Equal(123, getter(ref target, 1));
			Assert.Equal("ABC", getter(ref target, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => getter(ref target, 3));

			Assert.Throws<ArgumentException>(() => setter(ref target, 996));
			setter(ref target, 996, 1);
			setter(ref target, "XYZ", 2);
			Assert.Equal(996, getter(ref target, 1));
			Assert.Equal("XYZ", getter(ref target, 2));
		}

		[Fact]
		public void TestValueEntity()
		{
			var entity = new ValueEntity(100)
			{
				Name = "Popeye",
			};

			//init the target variable
			var target = (object)entity;

			//test the instance property(get and set)
			var property = typeof(ValueEntity).GetProperty(nameof(ValueEntity.Id));
			var getter = property.GenerateGetter();
			var setter = property.GenerateSetter();

			Assert.NotNull(getter);
			Assert.NotNull(setter);

			var value = getter(ref target);
			Assert.Equal(100, value);

			setter(ref target, 200);
			value = getter(ref target);
			Assert.Equal(200, value);

			property = typeof(ValueEntity).GetProperty(nameof(ValueEntity.Birthdate));
			getter = property.GenerateGetter();
			setter = property.GenerateSetter();

			value = getter(ref target);
			Assert.Null(value);

			setter(ref target, new DateTime(2000, 12, 31));
			value = getter(ref target);
			Assert.Equal(new DateTime(2000, 12, 31), value);

			//test the readonly property
			property = typeof(ValueEntity).GetProperty(nameof(ValueEntity.Age));
			getter = property.GetGetter();
			setter = property.GetSetter();

			Assert.NotNull(getter);
			Assert.Null(setter);
			value = getter(ref target);
			Assert.True((int)Convert.ChangeType(value, typeof(int)) > 0);

			//reset the target
			target = entity;

			//test the indexer property
			var members = typeof(ValueEntity).GetDefaultMembers();
			property = (System.Reflection.PropertyInfo)members[0];
			getter = property.GetGetter();
			setter = property.GetSetter();
			Assert.NotNull(getter);
			Assert.NotNull(setter);

			Assert.Null(getter(ref target, 0));
			Assert.Equal(123, getter(ref target, 1));
			Assert.Equal("ABC", getter(ref target, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => getter(ref target, 3));

			Assert.Throws<ArgumentException>(() => setter(ref target, 996));
			setter(ref target, 996, 1);
			setter(ref target, "XYZ", 2);
			Assert.Equal(996, getter(ref target, 1));
			Assert.Equal("XYZ", getter(ref target, 2));
		}

		#region 嵌套子类
		private class ClassEntity
		{
			private IList<object> _record = new List<object>(new object[] { null, 123, "ABC", });

			public object this[int index]
			{
				get => _record[index];
				set => _record[index] = value;
			}

			public object this[int index, string tag]
			{
				get => _record[index];
				set => _record[index] = value;
			}

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

			public static ClassEntity Instance
			{
				get; set;
			}
		}

		private struct ValueEntity
		{
			private IList<object> _record;

			public ValueEntity(int id)
			{
				this.Id = id;
				this.Name = null;
				this.Birthdate = null;

				_record = new List<object>(new object[] { null, 123, "ABC", });
			}

			public object this[int index]
			{
				get => _record[index];
				set => _record[index] = value;
			}

			public object this[int index, string tag]
			{
				get => _record[index];
				set => _record[index] = value;
			}

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
		}
		#endregion
	}
}
