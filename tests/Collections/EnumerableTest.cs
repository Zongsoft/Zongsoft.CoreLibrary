using System;
using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Collections
{
	public class EnumerableTest
	{
		[Fact]
		public void TestEmpty()
		{
			var objects = Enumerable.Empty(typeof(object));
			var strings = Enumerable.Empty(typeof(string));
			var integers = Enumerable.Empty(typeof(int));
			var currency = Enumerable.Empty(typeof(decimal));

			Assert.NotNull(objects);
			Assert.NotNull(strings);
			Assert.NotNull(integers);
			Assert.NotNull(currency);

			Assert.Empty(objects);
			Assert.Empty(strings);
			Assert.Empty(integers);
			Assert.Empty(currency);

			Assert.IsAssignableFrom<IEnumerable<object>>(objects);
			Assert.IsAssignableFrom<IEnumerable<string>>(strings);
			Assert.IsAssignableFrom<IEnumerable<int>>(integers);
			Assert.IsAssignableFrom<IEnumerable<decimal>>(currency);
		}

		[Fact]
		public void TestEnumerate()
		{
			const string FIRST = "First";

			var objects = Enumerable.Enumerate(new object(), typeof(object));
			var strings = Enumerable.Enumerate(FIRST, typeof(string));
			var integers = Enumerable.Enumerate(100, typeof(int));
			var currency = Enumerable.Enumerate(100, typeof(decimal));
			var dates = Enumerable.Enumerate(new[] { DateTime.MinValue, DateTime.MaxValue }, typeof(DateTime));
			var items = Enumerable.Enumerate(new Items(), typeof(Zongsoft.Tests.IPerson));

			Assert.NotNull(objects);
			Assert.NotNull(strings);
			Assert.NotNull(integers);
			Assert.NotNull(currency);
			Assert.NotNull(dates);
			Assert.NotNull(items);

			Assert.NotEmpty(objects);
			Assert.NotEmpty(strings);
			Assert.NotEmpty(integers);
			Assert.NotEmpty(currency);
			Assert.NotEmpty(dates);
			Assert.NotEmpty(items);

			Assert.IsAssignableFrom<IEnumerable<object>>(objects);
			Assert.IsAssignableFrom<IEnumerable<string>>(strings);
			Assert.IsAssignableFrom<IEnumerable<int>>(integers);
			Assert.IsAssignableFrom<IEnumerable<decimal>>(currency);
			Assert.IsAssignableFrom<IEnumerable<DateTime>>(dates);
			Assert.IsAssignableFrom<IEnumerable<Zongsoft.Tests.IPerson>>(items);

			var objectsIterator = objects.GetEnumerator();
			var stringsIterator = strings.GetEnumerator();
			var integersIterator = integers.GetEnumerator();
			var currencyIterator = currency.GetEnumerator();
			var datesIterator = dates.GetEnumerator();
			var itemsIterator = items.GetEnumerator();

			Assert.Throws<InvalidOperationException>(() => objectsIterator.Current);
			Assert.Throws<InvalidOperationException>(() => stringsIterator.Current);
			Assert.Throws<InvalidOperationException>(() => integersIterator.Current);
			Assert.Throws<InvalidOperationException>(() => currencyIterator.Current);
			Assert.Throws<InvalidOperationException>(() => datesIterator.Current);
			Assert.Throws<InvalidOperationException>(() => itemsIterator.Current);

			Assert.True(objectsIterator.MoveNext());
			Assert.True(stringsIterator.MoveNext());
			Assert.True(integersIterator.MoveNext());
			Assert.True(currencyIterator.MoveNext());

			Assert.NotNull(objectsIterator.Current);
			Assert.Equal(FIRST, stringsIterator.Current);
			Assert.Equal(100, integersIterator.Current);
			Assert.Equal(100m, currencyIterator.Current);

			Assert.True(datesIterator.MoveNext());
			Assert.Equal(DateTime.MinValue, datesIterator.Current);
			Assert.True(datesIterator.MoveNext());
			Assert.Equal(DateTime.MaxValue, datesIterator.Current);

			Assert.True(itemsIterator.MoveNext());
			Assert.IsAssignableFrom<Zongsoft.Tests.IEmployee>(itemsIterator.Current);
			Assert.True(itemsIterator.MoveNext());
			Assert.IsAssignableFrom<Zongsoft.Tests.ICustomer>(itemsIterator.Current);

			Assert.False(objectsIterator.MoveNext());
			Assert.False(stringsIterator.MoveNext());
			Assert.False(integersIterator.MoveNext());
			Assert.False(currencyIterator.MoveNext());
			Assert.False(datesIterator.MoveNext());
			Assert.False(itemsIterator.MoveNext());

			Assert.Throws<InvalidOperationException>(() => objectsIterator.Current);
			Assert.Throws<InvalidOperationException>(() => stringsIterator.Current);
			Assert.Throws<InvalidOperationException>(() => integersIterator.Current);
			Assert.Throws<InvalidOperationException>(() => currencyIterator.Current);
			Assert.Throws<InvalidOperationException>(() => datesIterator.Current);
			Assert.Throws<InvalidOperationException>(() => itemsIterator.Current);
		}

		private class Items : IEnumerable
		{
			private readonly object[] _items = new object[]
			{
				new Zongsoft.Tests.Employee(100, "Popeye"),
				new Zongsoft.Tests.Customer("Sophia"),
			};

			public IEnumerator GetEnumerator()
			{
				return new ItemEnumerator(_items);
			}

			private class ItemEnumerator : IEnumerator
			{
				private int _index;
				private object[] _items;

				public ItemEnumerator(object[] items)
				{
					_index = -1;
					_items = items;
				}

				public object Current
				{
					get
					{
						var index = _index;

						if(index >= 0 && index < _items.Length)
							return _items[index];

						throw new InvalidOperationException();
					}
				}

				public bool MoveNext()
				{
					var index = System.Threading.Interlocked.Increment(ref _index);

					if(index < _items.Length)
						return true;

					System.Threading.Interlocked.Exchange(ref _index, _items.Length);
					return false;
				}

				public void Reset()
				{
					_index = -1;
				}
			}
		}
	}
}
