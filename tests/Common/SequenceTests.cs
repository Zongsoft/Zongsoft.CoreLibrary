using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class SequenceTests
	{
		[Fact]
		private void Test()
		{
			var sequence = new MySequence();

			Parallel.For(0, 10, i =>
			{
				var key = "Key-" + i.ToString();

				Parallel.For(0, 100, j =>
				{
					sequence.Increment(key, 1, 0);
				});
			});

			for(int i = 0; i < 10; i++)
			{
				var key = "Key-" + i.ToString();

				Assert.True(sequence.TryGetValue(key, out var value));
				Assert.Equal(99, value);
			}
		}
	}

	public class MySequence : SequenceBase
	{
		private ConcurrentDictionary<string, long> _values = new ConcurrentDictionary<string, long>();

		protected override void OnReset(string key, int value)
		{
			Thread.SpinWait(100);
		}

		protected override long OnReserve(string key, int count, int seed)
		{
			return _values.AddOrUpdate(key, seed, (_, value) => value + count);
		}
	}
}
