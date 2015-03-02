using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Common.Tests
{
	[TestClass]
	public class BufferTests
	{
		#region 成员字段
		private byte[] _data;
		#endregion

		[TestInitialize]
		public void Initialize()
		{
			_data = new byte[10];

			for(int i = 0; i < _data.Length; i++)
			{
				_data[i] = (byte)i;
			}
		}

		[TestMethod]
		public void CanReadTest()
		{
			var buffer = new Zongsoft.Common.Buffer(_data);

			Assert.IsTrue(buffer.CanRead());

			Assert.AreEqual(_data.Length, buffer.Read(_data, 0, _data.Length));
			Assert.AreEqual(_data.Length, buffer.Position);
			Assert.IsFalse(buffer.CanRead());
		}

		[TestMethod]
		public void ReadTest()
		{
			var data = new byte[_data.Length];
			var buffer = new Zongsoft.Common.Buffer(_data, 2);

			Assert.AreEqual(1, buffer.Read(data, 0, 1));
			Assert.AreEqual(2, data[0]);
			Assert.AreEqual(7, buffer.Read(data, 0, data.Length));

			using(var stream = new System.IO.MemoryStream(data))
			{
				buffer = new Zongsoft.Common.Buffer(_data, 2);

				Assert.AreEqual(1, buffer.Read(stream, 1));
				Assert.AreEqual(7, buffer.Read(stream, data.Length));
			}
		}
	}
}
