using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Zongsoft.Common;
using Zongsoft.Tests;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class TypeExtensionTests
	{
		[Fact]
		public void IsAssignableFromTest()
		{
			var baseType = typeof(ICollection<Person>);
			var instanceType = typeof(Collection<Person>);

			Assert.True(Zongsoft.Common.TypeExtension.IsAssignableFrom(baseType, instanceType));
			Assert.True(baseType.IsAssignableFrom(instanceType));

			baseType = typeof(ICollection<>);

			Assert.True(Zongsoft.Common.TypeExtension.IsAssignableFrom(baseType, instanceType));
			Assert.False(baseType.IsAssignableFrom(instanceType));
		}

		[Fact]
		public void GetTypeTest()
		{
			Assert.Same(typeof(void), Zongsoft.Common.TypeExtension.GetType("void"));
			Assert.Same(typeof(object), Zongsoft.Common.TypeExtension.GetType("object"));
			Assert.Same(typeof(object), Zongsoft.Common.TypeExtension.GetType("System.object"));

			Assert.Same(typeof(string), Zongsoft.Common.TypeExtension.GetType("string"));
			Assert.Same(typeof(string), Zongsoft.Common.TypeExtension.GetType("System.string"));

			Assert.Same(typeof(int), Zongsoft.Common.TypeExtension.GetType("int"));
			Assert.Same(typeof(int), Zongsoft.Common.TypeExtension.GetType("int32"));
			Assert.Same(typeof(int), Zongsoft.Common.TypeExtension.GetType("System.Int32"));
			Assert.Same(typeof(int?), Zongsoft.Common.TypeExtension.GetType("int?"));
			Assert.Same(typeof(int[]), Zongsoft.Common.TypeExtension.GetType("int[]"));

			Assert.Same(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("date"));
			Assert.Same(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("time"));
			Assert.Same(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("datetime"));
			Assert.Same(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("date?"));
			Assert.Same(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("time?"));
			Assert.Same(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("datetime?"));
			Assert.Same(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("date[]"));
			Assert.Same(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("time[]"));
			Assert.Same(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("datetime[]"));

			Assert.Same(typeof(Guid), Zongsoft.Common.TypeExtension.GetType("GUID"));
			Assert.Same(typeof(Guid), Zongsoft.Common.TypeExtension.GetType("system.guid"));
			Assert.Same(typeof(Guid?), Zongsoft.Common.TypeExtension.GetType("guid?"));
			Assert.Same(typeof(Guid[]), Zongsoft.Common.TypeExtension.GetType("guid[]"));

			Assert.Same(typeof(Person), Zongsoft.Common.TypeExtension.GetType("Zongsoft.Tests.Person, Zongsoft.CoreLibrary.Tests"));
		}
	}
}
