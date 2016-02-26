using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Data
{
	[TestClass]
	public class ConditionCollectionTest
	{
		[TestMethod]
		public void OperatorTest()
		{
			var and1 = new Condition("a", 1) & new Condition("b", 2);
			Assert.AreEqual(ConditionCombine.And, and1.ConditionCombine);
			Assert.AreEqual(2, and1.Count);

			var and2 = new Condition("a", 1) & new Condition("b", 2) & new Condition("c", 3);
			Assert.AreEqual(ConditionCombine.And, and2.ConditionCombine);
			Assert.AreEqual(3, and2.Count);

			var or1 = new Condition("a", 1) | new Condition("b", 2);
			Assert.AreEqual(ConditionCombine.Or, or1.ConditionCombine);
			Assert.AreEqual(2, or1.Count);

			var or2 = new Condition("a", 1) | new Condition("b", 2) | new Condition("c", 3);
			Assert.AreEqual(ConditionCombine.Or, or2.ConditionCombine);
			Assert.AreEqual(3, or2.Count);

			ConditionCollection cs;

			//测试加法运算符

			cs = and1 + new Condition("key", "value");
			Assert.AreEqual(ConditionCombine.And, cs.ConditionCombine);
			Assert.AreEqual(3, cs.Count);

			cs = new Condition("key", "value") + and2;
			Assert.AreEqual(ConditionCombine.And, cs.ConditionCombine);
			Assert.AreEqual(4, cs.Count);

			//开始条件集合的并运算

			cs = and1 & and2;
			Assert.AreEqual(ConditionCombine.And, cs.ConditionCombine);
			Assert.AreEqual(5, cs.Count);

			cs = and1 & or2;
			Assert.AreEqual(ConditionCombine.And, cs.ConditionCombine);
			Assert.AreEqual(3, cs.Count);

			cs = or1 & and2;
			Assert.AreEqual(ConditionCombine.And, cs.ConditionCombine);
			Assert.AreEqual(4, cs.Count);

			cs = or1 & or2;
			Assert.AreEqual(ConditionCombine.And, cs.ConditionCombine);
			Assert.AreEqual(2, cs.Count);


			//开始条件集合的或运算

			cs = or1 | or2;
			Assert.AreEqual(ConditionCombine.Or, cs.ConditionCombine);
			Assert.AreEqual(5, cs.Count);

			cs = and1 | or2;
			Assert.AreEqual(ConditionCombine.Or, cs.ConditionCombine);
			Assert.AreEqual(4, cs.Count);

			cs = or1 | and2;
			Assert.AreEqual(ConditionCombine.Or, cs.ConditionCombine);
			Assert.AreEqual(3, cs.Count);

			cs = and1 | and2;
			Assert.AreEqual(ConditionCombine.Or, cs.ConditionCombine);
			Assert.AreEqual(2, cs.Count);

		}
	}
}
