using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

namespace Zongsoft.Services.Tests
{
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

			ServiceProviderFactory.Instance.Default = new ServiceProvider();
			ServiceProviderFactory.Instance.Default.Register("string", "I'm a service.");

			_provider1.Register("MC1", new Zongsoft.Runtime.Caching.MemoryCache("MemoryCache-1"), typeof(Zongsoft.Runtime.Caching.ICache));
			_provider1.Register(typeof(Zongsoft.Tests.Address));

			_provider2.Register("MC2", new Zongsoft.Runtime.Caching.MemoryCache("MemoryCache-2"), typeof(Zongsoft.Runtime.Caching.ICache));
			_provider2.Register(typeof(Zongsoft.Tests.Department));

			_provider3.Register("MC3", new Zongsoft.Runtime.Caching.MemoryCache("MemoryCache-3"), typeof(Zongsoft.Runtime.Caching.ICache));
			_provider3.Register(typeof(Zongsoft.Tests.Person));
		}

		[Fact]
		public void ResolveTest()
		{
			Zongsoft.Runtime.Caching.ICache cache = null;

			cache = _provider1.Resolve<Zongsoft.Runtime.Caching.ICache>();
			Assert.NotNull(cache);
			Assert.Equal("MemoryCache-1", cache.Name);

			cache = _provider1.Resolve("MC1") as Zongsoft.Runtime.Caching.ICache;
			Assert.NotNull(cache);
			Assert.Equal("MemoryCache-1", cache.Name);

			cache = _provider1.Resolve("MC2") as Zongsoft.Runtime.Caching.ICache;
			Assert.Null(cache);

			Assert.NotNull(_provider1.Resolve("string"));
			Assert.IsAssignableFrom<string>(_provider1.Resolve("string"));
			Assert.NotNull(_provider2.Resolve<string>());
			Assert.NotNull(_provider3.Resolve<string>());
			Assert.IsAssignableFrom<string>(_provider3.Resolve("string"));

			//将二号服务容器加入到一号服务容器中
			_provider1.Register(_provider2);

			cache = _provider1.Resolve("MC2") as Zongsoft.Runtime.Caching.ICache;
			Assert.NotNull(cache);
			Assert.Equal("MemoryCache-2", cache.Name);

			cache = _provider1.Resolve("MC3") as Zongsoft.Runtime.Caching.ICache;
			Assert.Null(cache);

			//将三号服务容器加入到二号服务容器中
			_provider2.Register(_provider3);

			//将一号服务容器加入到三号服务容器中（形成循环链）
			_provider3.Register(_provider1);

			cache = _provider1.Resolve("MC3") as Zongsoft.Runtime.Caching.ICache;
			Assert.NotNull(cache);
			Assert.Equal("MemoryCache-3", cache.Name);

			var address = _provider1.Resolve<Zongsoft.Tests.Address>();
			Assert.NotNull(address);

			var department = _provider1.Resolve<Zongsoft.Tests.Department>();
			Assert.NotNull(department);

			var person = _provider1.Resolve<Zongsoft.Tests.Person>();
			Assert.NotNull(person);

			Assert.NotNull(_provider1.Resolve("string"));
			Assert.IsAssignableFrom<string>(_provider1.Resolve("string"));
			Assert.NotNull(_provider2.Resolve<string>());
			Assert.NotNull(_provider3.Resolve<string>());
			Assert.IsAssignableFrom<string>(_provider3.Resolve("string"));

			//测试不存在的服务
			Assert.Null(_provider1.Resolve<IWorker>());
			Assert.Null(_provider1.Resolve("NoExisted"));
		}
	}
}
