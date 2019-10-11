using System;

using Xunit;

namespace Zongsoft.Runtime.Caching.Tests
{
	public class MemoryCacheTest
	{
		private MemoryCache _cache;

		public MemoryCacheTest()
		{
			_cache = MemoryCache.Default;
		}

		[Fact]
		public void RenameTest()
		{
			_cache.SetValue("Key1", "Value #1");
			_cache.SetValue("Key2", "Value #2");
			_cache.SetValue("Key3", "Value #3");

			Assert.True(_cache.Exists("Key2"));
			Assert.False(_cache.Exists("KeyX"));

			_cache.Rename("Key2", "KeyX");

			Assert.False(_cache.Exists("Key2"));
			Assert.True(_cache.Exists("KeyX"));
		}

		[Fact]
		public void ChangedEventTest()
		{
			_cache.Changed += Cache_Changed;

			_cache.SetValue("Key1", "Value #1", TimeSpan.FromSeconds(60));
			_cache.SetValue("Key2", "Value #2");
			_cache.SetValue("Key3", "Value #3");

			Assert.Equal(3, _cache.Count);

			_cache.Remove("Key2");

			Assert.Equal(2, _cache.Count);
		}

		private void Cache_Changed(object sender, CacheChangedEventArgs e)
		{
			switch(e.Reason)
			{
				case CacheChangedReason.Removed:
					Assert.Equal("Key2", e.OldKey);
					break;
				case CacheChangedReason.Expired:
					Assert.Equal("Key1", e.OldKey);
					break;
			}
		}
	}
}
