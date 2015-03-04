using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zongsoft.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zongsoft.Collections.Tests
{
	[TestClass]
	public class CategoryTests
	{
		private Category _root;

		public CategoryTests()
		{
			_root = new Category();

			var file = _root.Children.Add("File", "File", "文件");
			var edit = _root.Children.Add("Edit", "Edit", "编辑");
			var help = _root.Children.Add("Help", "Help", "帮助");

			file.Children.Add("Open", "Open", "打开");
			file.Children.Add("Close", "Close", "关闭");
			file.Children.Add("Save", "Save", "保存");
			file.Children.Add("SaveAs", "SaveAs", "另存为");
			file.Children.Add("Recents", "Recents", "最近的文件").Children.AddRange(
				new Category[]
				{
					new Category("Document-1"),
					new Category("Document-2"),
				}
			);
		}

		[TestMethod]
		public void FindTest()
		{
			Assert.IsNotNull(_root.Find("File"));
			Assert.IsNotNull(_root.Find("Edit"));
			Assert.IsNotNull(_root.Find("Help"));

			Assert.IsNotNull(_root.Find("File/Save"));
			Assert.IsNotNull(_root.Find("File/Recents"));
			Assert.IsNotNull(_root.Find("File/Recents/Document-1"));

			Assert.IsNotNull(_root.Find("/File/Save"));
			Assert.IsNotNull(_root.Find("/File/Recents"));
			Assert.IsNotNull(_root.Find("/File/Recents/Document-2"));

			Assert.IsNotNull(_root.Find("File").Find("Open"));
			Assert.IsNotNull(_root.Find("File").Find("Recents"));
			Assert.IsNotNull(_root.Find("File").Find("Recents").Find("Document-2"));
		}
	}
}
