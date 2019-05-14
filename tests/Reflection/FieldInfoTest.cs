using System;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Reflection
{
	public class FieldInfoTest
	{
		[Fact]
		public void TestClassEntity()
		{
			var entity = new ClassEntity(100)
			{
				Name = "Popeye",
			};

			//init the target variable
			var target = (object)entity;

			//test the instance field(get and set)
			var field = typeof(ClassEntity).GetField(nameof(ClassEntity.Name));
			var getter = field.GenerateGetter();
			var setter = field.GenerateSetter();

			Assert.NotNull(getter);
			Assert.NotNull(setter);

			var value = getter(ref target);
			Assert.Equal("Popeye", value);

			setter(ref target, "Popeye Zhong");
			value = getter(ref target);
			Assert.Equal("Popeye Zhong", value);

			field = typeof(ClassEntity).GetField(nameof(ClassEntity.Birthdate));
			getter = field.GenerateGetter();
			setter = field.GenerateSetter();

			value = getter(ref target);
			Assert.Null(value);

			setter(ref target, new DateTime(2000, 12, 31));
			value = getter(ref target);
			Assert.Equal(new DateTime(2000, 12, 31), value);

			//test the readonly field
			field = typeof(ClassEntity).GetField(nameof(ClassEntity.Id));
			getter = field.GetGetter();
			setter = field.GetSetter();

			Assert.NotNull(getter);
			Assert.Null(setter);
			value = getter(ref target);
			Assert.Equal(100, value);

			//test the static field
			field = typeof(ClassEntity).GetField(nameof(ClassEntity.Instance));
			getter = field.GetGetter();
			setter = field.GetSetter();
			Assert.NotNull(getter);
			Assert.NotNull(setter);

			value = getter(ref target);
			Assert.Null(value);

			target = null;
			value = getter(ref target);
			Assert.Null(value);

			value = new ClassEntity(1);
			setter(ref target, value);
			var newValue = getter(ref target);
			Assert.Equal(value, newValue);
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

			//test the instance field(get and set)
			var field = typeof(ValueEntity).GetField(nameof(ValueEntity.Name));
			var getter = field.GenerateGetter();
			var setter = field.GenerateSetter();

			Assert.NotNull(getter);
			Assert.NotNull(setter);

			var value = getter(ref target);
			Assert.Equal("Popeye", value);

			setter(ref target, "Popeye Zhong");
			value = getter(ref target);
			Assert.Equal("Popeye Zhong", value);

			field = typeof(ValueEntity).GetField(nameof(ValueEntity.Birthdate));
			getter = field.GenerateGetter();
			setter = field.GenerateSetter();

			value = getter(ref target);
			Assert.Null(value);

			setter(ref target, new DateTime(2000, 12, 31));
			value = getter(ref target);
			Assert.Equal(new DateTime(2000, 12, 31), value);

			//test the readonly field
			field = typeof(ValueEntity).GetField(nameof(ValueEntity.Id));
			getter = field.GetGetter();
			setter = field.GetSetter();

			Assert.NotNull(getter);
			Assert.Null(setter);
			value = getter(ref target);
			Assert.Equal(100, value);
		}

		#region 嵌套子类
		private class ClassEntity
		{
			public ClassEntity(int id)
			{
				this.Id = id;
			}

			public readonly int Id;
			public string Name;
			public DateTime? Birthdate;

			public static ClassEntity Instance;
		}

		private struct ValueEntity
		{
			public ValueEntity(int id)
			{
				this.Id = id;
				this.Name = null;
				this.Birthdate = null;
			}

			public readonly int Id;
			public string Name;
			public DateTime? Birthdate;
		}
		#endregion
	}
}
