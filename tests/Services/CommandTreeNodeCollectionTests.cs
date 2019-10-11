using System;
using System.Collections;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Services.Tests
{
	public class CommandTreeNodeCollectionTests
	{
		[Fact]
		public void AddTest()
		{
			var nodes = new CommandTreeNodeCollection(null);

			//测试增加空节点
			Assert.NotNull(nodes.Add("Empty"));
			Assert.NotNull(nodes["Empty"]);
			Assert.Same(typeof(CommandTreeNode), nodes["Empty"].GetType());
			Assert.Null(nodes["Empty"].Command);

			//以强类型的方式添加命令到命令树节点集合中
			Assert.NotNull(nodes.Add(new DummyCommand("StronglyDummy")));
			Assert.NotNull(nodes["StronglyDummy"]);
			Assert.Same(typeof(CommandTreeNode), nodes["StronglyDummy"].GetType());
			Assert.NotNull(nodes["StronglyDummy"].Command);
			Assert.Same(typeof(DummyCommand), nodes["StronglyDummy"].Command.GetType());

			var commands = (ICollection<ICommand>)nodes;

			commands.Add(new DummyCommand("WeaklyDummy"));
			Assert.Equal(3, commands.Count);

			Assert.NotNull(nodes["WeaklyDummy"]);
			Assert.Same(typeof(CommandTreeNode), nodes["WeaklyDummy"].GetType());
			Assert.NotNull(nodes["WeaklyDummy"].Command);
			Assert.Same(typeof(DummyCommand), nodes["WeaklyDummy"].Command.GetType());
		}

		private class DummyCommand : ICommand
		{
			#region 事件声明
			public event EventHandler EnabledChanged;
			public event EventHandler<CommandExecutedEventArgs> Executed;
			public event EventHandler<CommandExecutingEventArgs> Executing;
			#endregion

			#region 成员字段
			private string _name;
			private bool _enabled;
			#endregion

			#region 构造函数
			public DummyCommand(string name = "Dummy")
			{
				if(string.IsNullOrWhiteSpace(name))
					throw new ArgumentNullException("name");

				_name = name.Trim();
				_enabled = true;
			}
			#endregion

			#region 公共属性
			public string Name
			{
				get
				{
					return _name;
				}
				private set
				{
					if(string.IsNullOrWhiteSpace(value))
						throw new ArgumentNullException();

					_name = value.Trim();
				}
			}

			public bool Enabled
			{
				get
				{
					return true;
				}
				set
				{
					if(value == _enabled)
						return;

					_enabled = value;

					var enabledChanged = this.EnabledChanged;

					if(enabledChanged != null)
						enabledChanged(this, EventArgs.Empty);
				}
			}
			#endregion

			#region 公共方法
			public bool CanExecute(object parameter)
			{
				return this.Enabled;
			}

			public object Execute(object parameter)
			{
				this.OnExecuting(new CommandExecutingEventArgs(parameter, null));

				try
				{
					return null;
				}
				finally
				{
					this.OnExecuted(new CommandExecutedEventArgs(parameter, null, null));
				}
			}
			#endregion

			#region 激发事件
			protected virtual void OnExecuting(CommandExecutingEventArgs args)
			{
				var e = this.Executing;

				if(e != null)
					e(this, args);
			}

			protected virtual void OnExecuted(CommandExecutedEventArgs args)
			{
				var e = this.Executed;

				if(e != null)
					e(this, args);
			}
			#endregion
		}
	}
}
