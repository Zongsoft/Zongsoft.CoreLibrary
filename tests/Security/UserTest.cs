using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Zongsoft.Security
{
	public class UserTest
	{
		[Fact]
		public void TestName()
		{
			var user = new Zongsoft.Security.Membership.User();

			user.Name = string.Empty;
			user.Name = "admin";
			user.Name = "$123";
		}

		[Fact]
		public void TestNameException1()
		{
			Assert.Throws<ArgumentNullException>(() => new Zongsoft.Security.Membership.User(1, ""));
		}

		[Fact]
		public void TestNameException2()
		{
			var user = new Zongsoft.Security.Membership.User(1, "admin");

			Assert.Throws<ArgumentNullException>(() => user.Name = string.Empty);
		}

		[Fact]
		public void TestNameException3()
		{
			var user = new Zongsoft.Security.Membership.User();
			Assert.Throws<ArgumentOutOfRangeException>(() => user.Name = "a");
		}

		[Fact]
		public void TestNameException4()
		{
			var user = new Zongsoft.Security.Membership.User();
			Assert.Throws<ArgumentException>(() => user.Name = "123");
		}

		[Fact]
		public void TestNamespace()
		{
			var user = new Zongsoft.Security.Membership.User(1, "admin", "zongsoft");
			user.Namespace = "Zongsoft.CMS";

			Assert.Equal("Zongsoft.CMS", user.Namespace);
		}

		[Fact]
		public void TestNamespaceException1()
		{
			Assert.Throws<ArgumentException>(() => new Zongsoft.Security.Membership.User(1, "admin", "zongsoft-tests"));
		}
	}
}
