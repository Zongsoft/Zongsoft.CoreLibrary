using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Common;

using Xunit;

namespace Zongsoft.Common.Tests
{
	public class BufferTests
	{
		#region 成员字段
		private byte[] _data;
		#endregion

		public BufferTests()
		{
			this.Initialize();
		}

		public void Initialize()
		{
			_data = new byte[10];

			for(int i = 0; i < _data.Length; i++)
			{
				_data[i] = (byte)i;
			}
		}

		[Fact]
		public void CanReadTest()
		{
			var buffer = new Zongsoft.Common.Buffer(_data);

			Assert.True(buffer.CanRead());

			Assert.Equal(_data.Length, buffer.Read(_data, 0, _data.Length));
			Assert.Equal(_data.Length, buffer.Position);
			Assert.False(buffer.CanRead());
		}

		[Fact]
		public void ReadTest()
		{
			var data = new byte[_data.Length];
			var buffer = new Zongsoft.Common.Buffer(_data, 2);

			Assert.Equal(1, buffer.Read(data, 0, 1));
			Assert.Equal(2, data[0]);
			Assert.Equal(7, buffer.Read(data, 0, data.Length));

			using(var stream = new System.IO.MemoryStream(data))
			{
				buffer = new Zongsoft.Common.Buffer(_data, 2);

				Assert.Equal(1, buffer.Read(stream, 1));
				Assert.Equal(7, buffer.Read(stream, data.Length));
			}
		}
	}
}
