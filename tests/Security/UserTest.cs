using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Security
{
	[TestClass]
	public class UserTest
	{
		[TestMethod]
		public void TestName()
		{
			var user = new Zongsoft.Security.Membership.User();

			user.Name = string.Empty;
			user.Name = "admin";
			user.Name = "$123";
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNameException1()
		{
			var user = new Zongsoft.Security.Membership.User(1, "");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestNameException2()
		{
			var user = new Zongsoft.Security.Membership.User(1, "admin");
			user.Name = string.Empty;
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestNameException3()
		{
			var user = new Zongsoft.Security.Membership.User();
			user.Name = "a";
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestNameException4()
		{
			var user = new Zongsoft.Security.Membership.User();
			user.Name = "123";
		}

		[TestMethod]
		public void TestNamespace()
		{
			var user = new Zongsoft.Security.Membership.User(1, "admin", "zongsoft");
			user.Namespace = "Zongsoft.CMS";
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestNamespaceException1()
		{
			var user = new Zongsoft.Security.Membership.User(1, "admin", "zongsoft-tests");
		}
	}
}
