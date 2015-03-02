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
	}
}
