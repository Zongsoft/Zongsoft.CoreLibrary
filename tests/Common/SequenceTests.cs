using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class SequenceTests
	{
		const int ENTRY_COUNT = 100;
		const int CLICK_COUNT = 1000;

		[Fact]
		private void Test()
		{
			var sequence = new MySequence();

			Parallel.For(0, ENTRY_COUNT, i =>
			{
				var key = "Key-" + i.ToString();

				Parallel.For(0, CLICK_COUNT, j =>
				{
					sequence.Increment(key);
				});
			});

			for(int i = 0; i < ENTRY_COUNT; i++)
			{
				var key = "Key-" + i.ToString();

				Assert.True(sequence.TryGetValue(key, out var value));
				Assert.Equal(CLICK_COUNT, value);
			}
		}
	}

	public class MySequence : SequenceBase
	{
		private ConcurrentDictionary<string, long> _cache = new ConcurrentDictionary<string, long>();

		protected override void OnReset(string key, int value)
		{
			_cache[key] = value;
			Thread.SpinWait(100);
		}

		protected override long OnReserve(string key, int count, int seed)
		{
			if(count == 0)
			{
				if(_cache.TryGetValue(key, out var value))
					return value;
				else
					return seed;
			}

			return _cache.AddOrUpdate(key, seed + count, (_, x) => x + count);
		}
	}
}
