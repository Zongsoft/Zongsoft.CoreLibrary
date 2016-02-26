using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Data
{
	[TestClass]
	public class ConditionTest
	{
		private const string TEXT = "This's a string.";

		public void ToStringTest()
		{
			Condition condition;
			var conditions = new List<ICondition>();

			conditions.Add(condition = new Condition("Equals Boolean", true, ConditionOperator.Equal));
			Console.WriteLine(condition);
			conditions.Add(condition = new Condition("Equals Integer", 100, ConditionOperator.Equal));
			Console.WriteLine(condition);
			conditions.Add(condition = new Condition("Equals Currency", 5000.5m, ConditionOperator.Equal));
			Console.WriteLine(condition);
			conditions.Add(condition = new Condition("Equals Datetime", DateTime.Now, ConditionOperator.Equal));
			Console.WriteLine(condition);
			conditions.Add(condition = new Condition("Equals String", TEXT, ConditionOperator.Equal));
			Console.WriteLine(condition);
			conditions.Add(condition = new Condition("Equals Char", 'X', ConditionOperator.Equal));
			Console.WriteLine(condition);
			conditions.Add(condition = new Condition("Equals Null", null, ConditionOperator.Equal));
			Console.WriteLine(condition);
			conditions.Add(condition = new Condition("Equals DBNull", DBNull.Value, ConditionOperator.Equal));
			Console.WriteLine(condition);

			Console.WriteLine(new ConditionCollection(ConditionCombine.And, conditions));

			Console.WriteLine("--------------------------------");

			condition = new Condition("NotEquals Boolean", true, ConditionOperator.NotEqual);
			Console.WriteLine(condition);
			condition = new Condition("NotEquals Integer", 100, ConditionOperator.NotEqual);
			Console.WriteLine(condition);
			condition = new Condition("NotEquals Currency", 5000.5m, ConditionOperator.NotEqual);
			Console.WriteLine(condition);
			condition = new Condition("NotEquals Datetime", DateTime.Now, ConditionOperator.NotEqual);
			Console.WriteLine(condition);
			condition = new Condition("NotEquals String", TEXT, ConditionOperator.NotEqual);
			Console.WriteLine(condition);
			condition = new Condition("NotEquals Char", 'X', ConditionOperator.NotEqual);
			Console.WriteLine(condition);
			condition = new Condition("NotEquals Null", null, ConditionOperator.NotEqual);
			Console.WriteLine(condition);
			condition = new Condition("NotEquals DBNull", DBNull.Value, ConditionOperator.NotEqual);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("GreaterThan Boolean", true, ConditionOperator.GreaterThan);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThan Integer", 100, ConditionOperator.GreaterThan);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThan Currency", 5000.5m, ConditionOperator.GreaterThan);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThan Datetime", DateTime.Now, ConditionOperator.GreaterThan);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThan String", TEXT, ConditionOperator.GreaterThan);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThan Char", 'X', ConditionOperator.GreaterThan);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThan Null", null, ConditionOperator.GreaterThan);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThan DBNull", DBNull.Value, ConditionOperator.GreaterThan);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("GreaterThanEqual Boolean", true, ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThanEqual Integer", 100, ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThanEqual Currency", 5000.5m, ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThanEqual Datetime", DateTime.Now, ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThanEqual String", TEXT, ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThanEqual Char", 'X', ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThanEqual Null", null, ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("GreaterThanEqual DBNull", DBNull.Value, ConditionOperator.GreaterThanEqual);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("LessThan Boolean", true, ConditionOperator.LessThan);
			Console.WriteLine(condition);
			condition = new Condition("LessThan Integer", 100, ConditionOperator.LessThan);
			Console.WriteLine(condition);
			condition = new Condition("LessThan Currency", 5000.5m, ConditionOperator.LessThan);
			Console.WriteLine(condition);
			condition = new Condition("LessThan Datetime", DateTime.Now, ConditionOperator.LessThan);
			Console.WriteLine(condition);
			condition = new Condition("LessThan String", TEXT, ConditionOperator.LessThan);
			Console.WriteLine(condition);
			condition = new Condition("LessThan Char", 'X', ConditionOperator.LessThan);
			Console.WriteLine(condition);
			condition = new Condition("LessThan Null", null, ConditionOperator.LessThan);
			Console.WriteLine(condition);
			condition = new Condition("LessThan DBNull", DBNull.Value, ConditionOperator.LessThan);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("LessThanEqual Boolean", true, ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("LessThanEqual Integer", 100, ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("LessThanEqual Currency", 5000.5m, ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("LessThanEqual Datetime", DateTime.Now, ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("LessThanEqual String", TEXT, ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("LessThanEqual Char", 'X', ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("LessThanEqual Null", null, ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);
			condition = new Condition("LessThanEqual DBNull", DBNull.Value, ConditionOperator.LessThanEqual);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("Like Boolean", true, ConditionOperator.Like);
			Console.WriteLine(condition);
			condition = new Condition("Like Integer", 100, ConditionOperator.Like);
			Console.WriteLine(condition);
			condition = new Condition("Like Currency", 5000.5m, ConditionOperator.Like);
			Console.WriteLine(condition);
			condition = new Condition("Like Datetime", DateTime.Now, ConditionOperator.Like);
			Console.WriteLine(condition);
			condition = new Condition("Like String", TEXT, ConditionOperator.Like);
			Console.WriteLine(condition);
			condition = new Condition("Like Char", 'X', ConditionOperator.Like);
			Console.WriteLine(condition);
			condition = new Condition("Like Null", null, ConditionOperator.Like);
			Console.WriteLine(condition);
			condition = new Condition("Like DBNull", DBNull.Value, ConditionOperator.Like);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("Between Boolean", new[] { true, false }, ConditionOperator.Between);
			Console.WriteLine(condition);
			condition = new Condition("Between Integer", new[] { 100, 200 }, ConditionOperator.Between);
			Console.WriteLine(condition);
			condition = new Condition("Between Currency", new[] { 1000m, 2000m }, ConditionOperator.Between);
			Console.WriteLine(condition);
			condition = new Condition("Between Datetime", new[] { DateTime.Now, DateTime.Now }, ConditionOperator.Between);
			Console.WriteLine(condition);
			condition = new Condition("Between String", new[] { TEXT, TEXT }, ConditionOperator.Between);
			Console.WriteLine(condition);
			condition = new Condition("Between Char", new[] { 'X', 'Y', 'Z' }, ConditionOperator.Between);
			Console.WriteLine(condition);
			condition = new Condition("Between Null", null, ConditionOperator.Between);
			Console.WriteLine(condition);
			condition = new Condition("Between DBNull", DBNull.Value, ConditionOperator.Between);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("In Boolean", new[] { true, false }, ConditionOperator.In);
			Console.WriteLine(condition);
			condition = new Condition("In Integer", new[] { 100, 200 }, ConditionOperator.In);
			Console.WriteLine(condition);
			condition = new Condition("In Currency", new[] { 1000m, 2500m }, ConditionOperator.In);
			Console.WriteLine(condition);
			condition = new Condition("In Datetime", new[] { DateTime.Now, DateTime.Now }, ConditionOperator.In);
			Console.WriteLine(condition);
			condition = new Condition("In String", new[] { TEXT, TEXT }, ConditionOperator.In);
			Console.WriteLine(condition);
			condition = new Condition("In Char", new[] { 'X', 'Y', 'Z' }, ConditionOperator.In);
			Console.WriteLine(condition);
			condition = new Condition("In Null", null, ConditionOperator.In);
			Console.WriteLine(condition);
			condition = new Condition("In DBNull", DBNull.Value, ConditionOperator.In);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");

			condition = new Condition("NotIn Boolean", new[] { true, false }, ConditionOperator.NotIn);
			Console.WriteLine(condition);
			condition = new Condition("NotIn Integer", new[] { 100, 200 }, ConditionOperator.NotIn);
			Console.WriteLine(condition);
			condition = new Condition("NotIn Currency", new[] { 1000.0m, 5000m }, ConditionOperator.NotIn);
			Console.WriteLine(condition);
			condition = new Condition("NotIn Datetime", new[] { DateTime.Now, DateTime.Now }, ConditionOperator.NotIn);
			Console.WriteLine(condition);
			condition = new Condition("NotIn String", new[] { TEXT, TEXT }, ConditionOperator.NotIn);
			Console.WriteLine(condition);
			condition = new Condition("NotIn Char", new[] { 'X', 'Y', 'Z' }, ConditionOperator.NotIn);
			Console.WriteLine(condition);
			condition = new Condition("NotIn Null", null, ConditionOperator.NotIn);
			Console.WriteLine(condition);
			condition = new Condition("NotIn DBNull", DBNull.Value, ConditionOperator.NotIn);
			Console.WriteLine(condition);

			Console.WriteLine("--------------------------------");
		}
	}
}
