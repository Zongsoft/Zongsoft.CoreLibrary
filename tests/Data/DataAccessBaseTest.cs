using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Data
{
	[TestClass]
	public class DataAccessBaseTest
	{
		private DataAccessDummy _dataAccess;

		public DataAccessBaseTest()
		{
			_dataAccess = new DataAccessDummy();
		}

		[TestMethod]
		public void IsScalarTypeTest()
		{
			Assert.IsFalse(_dataAccess.IsScalarType(typeof(object)));

			Assert.IsTrue(_dataAccess.IsScalarType(typeof(string)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(string[])));

			Assert.IsTrue(_dataAccess.IsScalarType(typeof(int)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(int?)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(int[])));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(uint)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(ushort)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(ulong)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(byte)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(sbyte)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(char)));

			Assert.IsTrue(_dataAccess.IsScalarType(typeof(Guid)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(Guid?)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(Guid[])));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(DateTime)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(DateTime?)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(DateTime[])));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(TimeSpan)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(TimeSpan?)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(TimeSpan[])));

			Assert.IsFalse(_dataAccess.IsScalarType(typeof(Zongsoft.Tests.Address)));
			Assert.IsFalse(_dataAccess.IsScalarType(typeof(Zongsoft.Tests.Address[])));

			Assert.IsTrue(_dataAccess.IsScalarType(typeof(Zongsoft.Tests.Gender)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(Zongsoft.Tests.Gender?)));
			Assert.IsTrue(_dataAccess.IsScalarType(typeof(Zongsoft.Tests.Gender[])));

			Assert.IsFalse(_dataAccess.IsScalarType(typeof(Zongsoft.Tests.Customer)));
			Assert.IsFalse(_dataAccess.IsScalarType(typeof(Zongsoft.Tests.Customer[])));
		}

		private class DataAccessDummy : DataAccessBase
		{
			public override object Execute(string name, System.Collections.Generic.IDictionary<string, object> inParameters, out System.Collections.Generic.IDictionary<string, object> outParameters)
			{
				throw new NotImplementedException();
			}

			protected override int Count(string name, ICondition condition, string[] includes)
			{
				throw new NotImplementedException();
			}

			protected override IEnumerable<T> Select<T>(string name, ICondition condition, string[] members, Paging paging, Grouping grouping, params Sorting[] sorting)
			{
				throw new NotImplementedException();
			}

			protected override int Delete(string name, ICondition condition, string[] cascades)
			{
				throw new NotImplementedException();
			}

			protected override int Insert(string name, object entity, string[] members)
			{
				throw new NotImplementedException();
			}

			protected override int Insert<T>(string name, IEnumerable<T> entities, string[] members)
			{
				throw new NotImplementedException();
			}

			protected override int Update(string name, object entity, ICondition condition, string[] members)
			{
				throw new NotImplementedException();
			}

			protected override int Update<T>(string name, IEnumerable<T> entities, ICondition condition, string[] members)
			{
				throw new NotImplementedException();
			}

			protected override Type GetEntityType(string name)
			{
				return null;
			}

			internal new bool IsScalarType(Type type)
			{
				return base.IsScalarType(type);
			}
		}
	}
}
