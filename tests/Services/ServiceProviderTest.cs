using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Services.Tests
{
	[TestClass]
	public class ServiceProviderTest
	{
		private ServiceProvider _provider1;
		private ServiceProvider _provider2;
		private ServiceProvider _provider3;

		public ServiceProviderTest()
		{
			_provider1 = new ServiceProvider();
			_provider2 = new ServiceProvider();
			_provider3 = new ServiceProvider();

			_provider1.Register("MC1", new Zongsoft.Runtime.Caching.MemoryCache("MemoryCache-1"), typeof(Zongsoft.Runtime.Caching.ICache));
			_provider1.Register(typeof(Zongsoft.Tests.Address));

			_provider2.Register("MC2", new Zongsoft.Runtime.Caching.MemoryCache("MemoryCache-2"), typeof(Zongsoft.Runtime.Caching.ICache));
			_provider2.Register(typeof(Zongsoft.Tests.Department));

			_provider3.Register("MC3", new Zongsoft.Runtime.Caching.MemoryCache("MemoryCache-3"), typeof(Zongsoft.Runtime.Caching.ICache));
			_provider3.Register(typeof(Zongsoft.Tests.Person));
		}

		[TestMethod]
		public void ResolveTest()
		{
			Zongsoft.Runtime.Caching.ICache cache = null;

			cache = _provider1.Resolve<Zongsoft.Runtime.Caching.ICache>();
			Assert.IsNotNull(cache);
			Assert.AreEqual("MemoryCache-1", cache.Name);

			cache = _provider1.Resolve("MC1") as Zongsoft.Runtime.Caching.ICache;
			Assert.IsNotNull(cache);
			Assert.AreEqual("MemoryCache-1", cache.Name);

			cache = _provider1.Resolve("MC2") as Zongsoft.Runtime.Caching.ICache;
			Assert.IsNull(cache);

			//将二号服务容器加入到一号服务容器中
			_provider1.Register(_provider2);

			cache = _provider1.Resolve("MC2") as Zongsoft.Runtime.Caching.ICache;
			Assert.IsNotNull(cache);
			Assert.AreEqual("MemoryCache-2", cache.Name);

			cache = _provider1.Resolve("MC3") as Zongsoft.Runtime.Caching.ICache;
			Assert.IsNull(cache);

			//将三号服务容器加入到二号服务容器中
			_provider2.Register(_provider3);

			//将一号服务容器加入到三号服务容器中（形成循环链）
			_provider3.Register(_provider1);

			cache = _provider1.Resolve("MC3") as Zongsoft.Runtime.Caching.ICache;
			Assert.IsNotNull(cache);
			Assert.AreEqual("MemoryCache-3", cache.Name);

			var address = _provider1.Resolve<Zongsoft.Tests.Address>();
			Assert.IsNotNull(address);

			var department = _provider1.Resolve<Zongsoft.Tests.Department>();
			Assert.IsNotNull(department);

			var person = _provider1.Resolve<Zongsoft.Tests.Person>();
			Assert.IsNotNull(person);

			//测试不存在的服务
			Assert.IsNull(_provider1.Resolve<IWorker>());
			Assert.IsNull(_provider1.Resolve("NoExisted"));
		}
	}
}
