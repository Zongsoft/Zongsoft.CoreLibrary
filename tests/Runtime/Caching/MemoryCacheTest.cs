using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Runtime.Caching.Tests
{
	[TestClass]
	public class MemoryCacheTest
	{
		private MemoryCache _cache;

		public MemoryCacheTest()
		{
			_cache = MemoryCache.Default;
		}

		[TestMethod]
		public void RenameTest()
		{
			_cache.SetValue("Key1", "Value #1");
			_cache.SetValue("Key2", "Value #2");
			_cache.SetValue("Key3", "Value #3");

			Assert.IsTrue(_cache.Exists("Key2"));
			Assert.IsFalse(_cache.Exists("KeyX"));

			_cache.Rename("Key2", "KeyX");

			Assert.IsFalse(_cache.Exists("Key2"));
			Assert.IsTrue(_cache.Exists("KeyX"));
		}

		[TestMethod]
		public void ChangedEventTest()
		{
			_cache.Changed += Cache_Changed;

			_cache.SetValue("Key1", "Value #1", TimeSpan.FromSeconds(5));
			_cache.SetValue("Key2", "Value #2");
			_cache.SetValue("Key3", "Value #3");

			Assert.AreEqual(3, _cache.Count);

			_cache.Remove("Key2");

			Assert.AreEqual(2, _cache.Count);
		}

		private void Cache_Changed(object sender, CacheChangedEventArgs e)
		{
			switch(e.Reason)
			{
				case CacheChangedReason.Removed:
					Assert.AreEqual("Key2", e.OldKey);
					break;
				case CacheChangedReason.Expired:
					Assert.AreEqual("Key1", e.OldKey);
					break;
			}
		}
	}
}
