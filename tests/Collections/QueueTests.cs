using System;
using System.Collections.Generic;

using Zongsoft.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Collections.Tests
{
	[TestClass]
	public class QueueTests
	{
		[TestMethod]
		public void QueueTest()
		{
			var queue = new Queue(32);

			Assert.AreEqual(32, queue.Capacity);
			Assert.AreEqual(0, queue.Count);
		}

		[TestMethod]
		public void ClearTest()
		{
			var queue = new Queue();

			Assert.AreEqual(0, queue.Count);

			for(int i = 0; i < 100; i++)
			{
				queue.Enqueue(i.ToString());
			}

			Assert.AreEqual(100, queue.Count);
		}

		[TestMethod]
		public void TrimToSizeTest()
		{
			var queue = new Queue(64);

			Assert.AreEqual(0, queue.Count);
			Assert.AreEqual(64, queue.Capacity);

			queue.Enqueue("No.1");
			queue.Enqueue("No.2");

			Assert.AreEqual(2, queue.Count);
			Assert.AreEqual(64, queue.Capacity);

			queue.TrimToSize();

			Assert.AreEqual(2, queue.Count);
			Assert.AreEqual(2, queue.Capacity);
		}

		[TestMethod]
		public void ToArrayTest()
		{
			var queue = new Queue();
			var array = queue.ToArray();

			Assert.AreEqual(0, array.Length);

			queue.Enqueue("No.1");
			queue.Enqueue("No.2");
			queue.Enqueue("No.3");

			array = queue.ToArray();
			Assert.AreEqual(3, array.Length);
		}

		[TestMethod]
		public void DequeueTest()
		{
			var queue = new Queue(100);

			for(int i = 0; i < 100; i++)
			{
				queue.Enqueue("No." + (i + 1).ToString());
			}

			var result = queue.Dequeue();
			Assert.AreEqual("No.1", result);
			result = queue.Dequeue();
			Assert.AreEqual("No.2", result);

			var index = 3;
			var items = queue.Dequeue(8);

			foreach(var item in items)
			{
				Assert.AreEqual("No." + (index++).ToString(), item);
			}
		}

		[TestMethod]
		public void EnqueueTest()
		{
			var queue = new Queue();

			Assert.AreEqual(0, queue.Count);

			queue.Enqueue("No.1");
			queue.Enqueue("No.2");
			queue.Enqueue("No.3");

			Assert.AreEqual(3, queue.Count);

			queue.Enqueue(1);
			queue.Enqueue(DateTime.Now);
			queue.Enqueue(Guid.NewGuid());

			Assert.AreEqual(6, queue.Count);

			queue.Enqueue(new object[] { "xyz", new Zongsoft.Tests.Person(), 123 });

			Assert.AreEqual(9, queue.Count);
		}

		[TestMethod]
		public void PeekTest()
		{
			var queue = new Queue();

			for(int i = 0; i < 100; i++)
			{
				queue.Enqueue(i.ToString());
			}

			Assert.AreEqual(100, queue.Count);
			Assert.AreEqual("0", queue.Peek());
			Assert.AreEqual("0", queue.Peek());
			Assert.AreEqual("0", queue.Peek());
			Assert.AreEqual(100, queue.Count);

			var items = queue.Peek(10);
			var index = 0;

			Assert.AreEqual(100, queue.Count);

			foreach(var item in items)
			{
				Assert.AreEqual(index++.ToString(), item);
			}

			Assert.AreEqual(10, index);
		}

		[TestMethod]
		public void TakeTest()
		{
			var queue = new Queue();

			for(int i = 0; i < 100; i++)
			{
				queue.Enqueue(i.ToString());
			}

			Assert.AreEqual(100, queue.Count);
			Assert.AreEqual("0", queue.Take(0));
			Assert.AreEqual("1", queue.Take(1));
			Assert.AreEqual("2", queue.Take(2));
			Assert.AreEqual(100, queue.Count);

			var items = queue.Take(10, 10);
			var index = 10;

			Assert.AreEqual(100, queue.Count);

			foreach(var item in items)
			{
				Assert.AreEqual(index++.ToString(), item);
			}

			Assert.AreEqual(20, index);
		}

		[TestMethod]
		public void GetEnumeratorTest()
		{
			var queue = new Queue();

			for(int i = 0; i < 100; i++)
			{
				queue.Enqueue(i.ToString());
			}

			int index = 0;

			foreach(var item in queue)
			{
				Assert.AreEqual(index++.ToString(), item);
			}
		}

		[TestMethod]
		public void CopyToTest()
		{
			var queue = new Queue();

			for(int i = 0; i < 100; i++)
			{
				queue.Enqueue(i.ToString());
			}

			var array = new object[queue.Count];
			queue.CopyTo(array, 0);

			Assert.AreEqual(queue.Count, array.Length);

			for(int i = 0; i < array.Length; i++)
			{
				Assert.AreEqual(i.ToString(), array[i].ToString());
			}
		}
	}
}
