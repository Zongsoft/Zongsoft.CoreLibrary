using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Common;
using Zongsoft.Tests;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class ObjectReferenceTests
	{
		[Fact]
		public void ObjectReferenceTest()
		{
			var reference = new ObjectReference<Person>(() => new Person());

			Assert.False(reference.HasTarget);
			var target = reference.Target;
			Assert.True(reference.HasTarget);
		}

		[Fact]
		public void DisposeTest()
		{
			var reference = new ObjectReference<Person>(() => new Person());
			reference.Disposed += Reference_Disposed;

			Assert.False(reference.HasTarget);
			Assert.False(reference.IsDisposed);

			reference.Dispose();
			Assert.True(reference.IsDisposed);

			ObjectDisposedException exception = null;

			try
			{
				var target = reference.Target;
			}
			catch(ObjectDisposedException ex)
			{
				exception = ex;
			}

			Assert.NotNull(exception);
		}

		private void Reference_Disposed(object sender, DisposedEventArgs e)
		{
			var disposableObject = (IDisposableObject)sender;

			Assert.True(disposableObject.IsDisposed);
		}
	}
}
