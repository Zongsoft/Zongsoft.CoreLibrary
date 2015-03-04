using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Zongsoft.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Common.Tests
{
	[TestClass]
	public class TypeExtensionTests
	{
		[TestMethod]
		public void IsAssignableFromTest()
		{
			var baseType = typeof(ICollection<Person>);
			var instanceType = typeof(Collection<Person>);

			Assert.IsTrue(Zongsoft.Common.TypeExtension.IsAssignableFrom(baseType, instanceType));
			Assert.IsTrue(baseType.IsAssignableFrom(instanceType));

			baseType = typeof(ICollection<>);

			Assert.IsTrue(Zongsoft.Common.TypeExtension.IsAssignableFrom(baseType, instanceType));
			Assert.IsFalse(baseType.IsAssignableFrom(instanceType));
		}

		[TestMethod]
		public void GetTypeTest()
		{
			Assert.AreSame(typeof(void), Zongsoft.Common.TypeExtension.GetType("void"));
			Assert.AreSame(typeof(object), Zongsoft.Common.TypeExtension.GetType("object"));
			Assert.AreSame(typeof(object), Zongsoft.Common.TypeExtension.GetType("System.object"));

			Assert.AreSame(typeof(string), Zongsoft.Common.TypeExtension.GetType("string"));
			Assert.AreSame(typeof(string), Zongsoft.Common.TypeExtension.GetType("System.string"));

			Assert.AreSame(typeof(int), Zongsoft.Common.TypeExtension.GetType("int"));
			Assert.AreSame(typeof(int), Zongsoft.Common.TypeExtension.GetType("int32"));
			Assert.AreSame(typeof(int), Zongsoft.Common.TypeExtension.GetType("System.Int32"));
			Assert.AreSame(typeof(int?), Zongsoft.Common.TypeExtension.GetType("int?"));
			Assert.AreSame(typeof(int[]), Zongsoft.Common.TypeExtension.GetType("int[]"));

			Assert.AreSame(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("date"));
			Assert.AreSame(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("time"));
			Assert.AreSame(typeof(DateTime), Zongsoft.Common.TypeExtension.GetType("datetime"));
			Assert.AreSame(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("date?"));
			Assert.AreSame(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("time?"));
			Assert.AreSame(typeof(DateTime?), Zongsoft.Common.TypeExtension.GetType("datetime?"));
			Assert.AreSame(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("date[]"));
			Assert.AreSame(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("time[]"));
			Assert.AreSame(typeof(DateTime[]), Zongsoft.Common.TypeExtension.GetType("datetime[]"));

			Assert.AreSame(typeof(Guid), Zongsoft.Common.TypeExtension.GetType("GUID"));
			Assert.AreSame(typeof(Guid), Zongsoft.Common.TypeExtension.GetType("system.guid"));
			Assert.AreSame(typeof(Guid?), Zongsoft.Common.TypeExtension.GetType("guid?"));
			Assert.AreSame(typeof(Guid[]), Zongsoft.Common.TypeExtension.GetType("guid[]"));

			Assert.AreSame(typeof(Person), Zongsoft.Common.TypeExtension.GetType("Zongsoft.Common.Tests.Person, Zongsoft.CoreLibrary.Tests"));
		}
	}
}
