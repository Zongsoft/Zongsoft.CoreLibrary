using System;
using System.Collections;
using System.Collections.Generic;

using Zongsoft.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Services.Tests
{
	[TestClass]
	public class CommandTreeNodeCollectionTests
	{
		[TestMethod]
		public void AddTest()
		{
			var nodes = new CommandTreeNodeCollection(null);

			//测试增加空节点
			Assert.IsNotNull(nodes.Add("Empty"));
			Assert.IsNotNull(nodes["Empty"]);
			Assert.AreSame(typeof(CommandTreeNode), nodes["Empty"].GetType());
			Assert.IsNull(nodes["Empty"].Command);

			//以强类型的方式添加命令到命令树节点集合中
			Assert.IsNotNull(nodes.Add(new DummyCommand("StronglyDummy")));
			Assert.IsNotNull(nodes["StronglyDummy"]);
			Assert.AreSame(typeof(CommandTreeNode), nodes["StronglyDummy"].GetType());
			Assert.IsNotNull(nodes["StronglyDummy"].Command);
			Assert.AreSame(typeof(DummyCommand), nodes["StronglyDummy"].Command.GetType());

			var list = (System.Collections.IList)nodes;

			//以弱类型的方式添加命令到命令树节点集合中
			Assert.IsNotNull(list.Add(new DummyCommand("WeaklyDummy")));
			Assert.AreEqual(3, list.Count);

			Assert.IsNotNull(nodes["WeaklyDummy"]);
			Assert.AreSame(typeof(CommandTreeNode), nodes["WeaklyDummy"].GetType());
			Assert.IsNotNull(nodes["WeaklyDummy"].Command);
			Assert.AreSame(typeof(DummyCommand), nodes["WeaklyDummy"].Command.GetType());
		}

		private class DummyCommand : ICommand
		{
			public event EventHandler EnabledChanged;
			public event EventHandler<CommandExecutedEventArgs> Executed;
			public event EventHandler<CommandExecutingEventArgs> Executing;

			public DummyCommand(string name = "Dummy")
			{
				if(string.IsNullOrWhiteSpace(name))
					throw new ArgumentNullException("name");

				this.Name = name.Trim();
			}

			public string Name
			{
				get;
				private set;
			}

			public bool Enabled
			{
				get
				{
					return true;
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public bool CanExecute(object parameter)
			{
				return this.Enabled;
			}

			public object Execute(object parameter)
			{
				return null;
			}
		}
	}
}
