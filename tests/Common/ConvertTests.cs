using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Common.Tests
{
	[TestClass]
	public class ConvertTests
	{
		[TestMethod]
		public void ConvertValueTest()
		{
			Assert.AreEqual(Gender.Male, Zongsoft.Common.Convert.ConvertValue("male", typeof(Gender)));
			Assert.AreEqual(Gender.Male, Zongsoft.Common.Convert.ConvertValue("Male", typeof(Gender)));
			Assert.AreEqual(Gender.Female, Zongsoft.Common.Convert.ConvertValue("female", typeof(Gender)));
			Assert.AreEqual(Gender.Female, Zongsoft.Common.Convert.ConvertValue("Female", typeof(Gender)));

			Assert.AreEqual(Gender.Male, Zongsoft.Common.Convert.ConvertValue(0, typeof(Gender)));
			Assert.AreEqual(Gender.Male, Zongsoft.Common.Convert.ConvertValue("0", typeof(Gender)));
			Assert.AreEqual(Gender.Female, Zongsoft.Common.Convert.ConvertValue(1, typeof(Gender)));
			Assert.AreEqual(Gender.Female, Zongsoft.Common.Convert.ConvertValue("1", typeof(Gender)));

			//根据枚举项的 AliasAttribute 值来解析
			Assert.AreEqual(Gender.Male, Zongsoft.Common.Convert.ConvertValue<Gender>("M"));
			Assert.AreEqual(Gender.Female, Zongsoft.Common.Convert.ConvertValue<Gender>("F"));

			//根据枚举项的 DescriptionAttribute 值来解析
			Assert.AreEqual(Gender.Male, Zongsoft.Common.Convert.ConvertValue<Gender>("男士"));
			Assert.AreEqual(Gender.Female, Zongsoft.Common.Convert.ConvertValue<Gender>("女士"));
		}

		[TestMethod]
		public void GetValueTest()
		{
			var person = new Person()
			{
				Name = "Popeye Zhong",
				Gender = Gender.Male,
				HomeAddress = new Address
				{
					CountryId = 123,
					City = "Wuhan",
					Detail = "****",
				},
			};

			Assert.AreEqual("Popeye Zhong", Zongsoft.Common.Convert.GetValue(person, "Name"));
			Assert.AreEqual("Wuhan", Zongsoft.Common.Convert.GetValue(person, "HomeAddress.City"));

			Zongsoft.Common.Convert.SetValue(person, "Name", "Popeye");
			Assert.AreEqual("Popeye", Zongsoft.Common.Convert.GetValue(person, "Name"));

			Zongsoft.Common.Convert.SetValue(person, "HomeAddress.City", "Shenzhen");
			Assert.AreEqual("Shenzhen", Zongsoft.Common.Convert.GetValue(person, "HomeAddress.City"));
		}

		[TestMethod]
		public void ToHexStringTest()
		{
			var source = new byte[16];

			for(int i = 0; i < source.Length; i++)
				source[i] = (byte)i;

			var hexString1 = Zongsoft.Common.Convert.ToHexString(source);
			var hexString2 = Zongsoft.Common.Convert.ToHexString(source, '-');

			Assert.AreEqual("000102030405060708090A0B0C0D0E0F", hexString1);
			Assert.AreEqual("00-01-02-03-04-05-06-07-08-09-0A-0B-0C-0D-0E-0F", hexString2);

			var bytes1 = Zongsoft.Common.Convert.FromHexString(hexString1);
			var bytes2 = Zongsoft.Common.Convert.FromHexString(hexString2, '-');

			Assert.AreEqual(source.Length, bytes1.Length);
			Assert.AreEqual(source.Length, bytes2.Length);

			Assert.IsTrue(Zongsoft.Collections.BinaryComparer.Default.Equals(source, bytes1));
			Assert.IsTrue(Zongsoft.Collections.BinaryComparer.Default.Equals(source, bytes2));
		}

		[TestMethod]
		public void BitVector32Test()
		{
			Zongsoft.Common.BitVector32 vector = 1;

			Assert.AreEqual(1, vector.Data);
			Assert.IsTrue(vector[1]);
			Assert.IsFalse(vector[2]);
			Assert.IsFalse(vector[3]);
			Assert.IsFalse(vector[4]);
			Assert.IsFalse(vector[5]);

			vector[5] = true;
			Assert.AreEqual(5, vector.Data);
			Assert.IsTrue(vector[1]);
			Assert.IsFalse(vector[2]);
			Assert.IsFalse(vector[3]);
			Assert.IsTrue(vector[4]);
			Assert.IsTrue(vector[5]);
		}
	}
}
