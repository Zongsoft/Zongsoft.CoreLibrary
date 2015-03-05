using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Common;
using Zongsoft.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Common.Tests
{
	[TestClass]
	public class ObjectReferenceTests
	{
		[TestMethod]
		public void ObjectReferenceTest()
		{
			var reference = new ObjectReference<Person>(() => new Person());

			Assert.IsFalse(reference.HasTarget);
			var target = reference.Target;
			Assert.IsTrue(reference.HasTarget);
		}

		[TestMethod]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void DisposeTest()
		{
			var reference = new ObjectReference<Person>(() => new Person());

			Assert.IsFalse(reference.HasTarget);
			Assert.IsFalse(reference.IsDisposed);

			reference.Dispose();
			Assert.IsTrue(reference.IsDisposed);

			var target = reference.Target;
		}
	}
}
