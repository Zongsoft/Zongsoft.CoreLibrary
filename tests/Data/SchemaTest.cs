using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using Zongsoft.Collections;

namespace Zongsoft.Data
{
	public class SchemaTest
	{
		[Fact]
		public void Test1()
		{
			Collections.IReadOnlyNamedCollection<Schema> schemas;

			schemas = Schema.Parse(null);
			Assert.Null(schemas);

			schemas = Schema.Parse(" \t ");
			Assert.Null(schemas);

			schemas = Schema.Parse("*");
			Assert.Equal(6, schemas.Count);
			Assert.True(schemas.Contains("a"));
			Assert.True(schemas.Contains("b"));
			Assert.True(schemas.Contains("c"));
			Assert.True(schemas.Contains("d"));
			Assert.True(schemas.Contains("e"));
			Assert.True(schemas.Contains("f"));

			schemas = Schema.Parse("*, !");
			Assert.Equal(0, schemas.Count);

			schemas = Schema.Parse("*, !a, !c, !f, c");
			Assert.Equal(4, schemas.Count);
			Assert.False(schemas.Contains("a"));
			Assert.True(schemas.Contains("b"));
			Assert.True(schemas.Contains("c"));
			Assert.True(schemas.Contains("d"));
			Assert.True(schemas.Contains("e"));
			Assert.False(schemas.Contains("f"));
		}

		[Fact]
		public void Test2()
		{
			Collections.IReadOnlyNamedCollection<Schema> schemas;

			schemas = Schema.Parse("*, !, a, !b, c, a, forums:100/2{*, !, a, !b, f, moderator{name, avatar}}");
			Assert.NotEmpty(schemas);
			Assert.Equal(3, schemas.Count);

			Assert.True(schemas.Contains("a"));
			Assert.Null(schemas["a"].Parent);
			Assert.Null(schemas["a"].Paging);
			Assert.Null(schemas["a"].Sortings);
			Assert.False(schemas["a"].HasChildren);

			Assert.True(schemas.Contains("c"));
			Assert.Null(schemas["c"].Parent);
			Assert.Null(schemas["c"].Paging);
			Assert.Null(schemas["c"].Sortings);
			Assert.False(schemas["c"].HasChildren);

			Assert.True(schemas.Contains("forums"));
			Assert.Null(schemas["forums"].Parent);
			Assert.Null(schemas["forums"].Sortings);
			Assert.NotNull(schemas["forums"].Paging);
			Assert.Equal(100, schemas["forums"].Paging.PageIndex);
			Assert.Equal(2, schemas["forums"].Paging.PageSize);
			Assert.True(schemas["forums"].HasChildren);
			Assert.Equal(3, schemas["forums"].Children.Count);

			var forums = (Schema)schemas["forums"];
			Assert.True(forums.HasChildren);
			Assert.True(forums.Children.Contains("a"));
			Assert.True(forums.Children.Contains("f"));
			Assert.True(forums.Children.Contains("moderator"));

			var moderator = (Schema)forums["moderator"];
			Assert.NotNull(moderator.Parent);
			Assert.True(moderator.HasChildren);
			Assert.True(moderator.Children.Contains("name"));
			Assert.True(moderator.Children.Contains("avatar"));
		}

		[Fact]
		public void Test3()
		{
			Collections.IReadOnlyNamedCollection<Schema> schemas;

			schemas = Schema.Parse(@"*, !, a, !b, c, a, forums:100/2(~timestamp, id){*, !, a, !b, f, moderator(name){name, avatar}}");
			Assert.NotEmpty(schemas);
			Assert.Equal(3, schemas.Count);

			Assert.True(schemas.Contains("a"));
			Assert.Null(schemas["a"].Parent);
			Assert.Null(schemas["a"].Paging);
			Assert.Null(schemas["a"].Sortings);
			Assert.False(schemas["a"].HasChildren);

			Assert.True(schemas.Contains("c"));
			Assert.Null(schemas["c"].Parent);
			Assert.Null(schemas["c"].Paging);
			Assert.Null(schemas["c"].Sortings);
			Assert.False(schemas["c"].HasChildren);

			Assert.True(schemas.Contains("forums"));
			Assert.Null(schemas["forums"].Parent);
			Assert.NotNull(schemas["forums"].Paging);
			Assert.Equal(100, schemas["forums"].Paging.PageIndex);
			Assert.Equal(2, schemas["forums"].Paging.PageSize);
			Assert.NotNull(schemas["forums"].Sortings);
			Assert.Equal(2, schemas["forums"].Sortings.Length);
			Assert.Equal("timestamp", schemas["forums"].Sortings[0].Name);
			Assert.Equal(SortingMode.Descending, schemas["forums"].Sortings[0].Mode);
			Assert.Equal("id", schemas["forums"].Sortings[1].Name);
			Assert.Equal(SortingMode.Ascending, schemas["forums"].Sortings[1].Mode);
			Assert.True(schemas["forums"].HasChildren);
			Assert.Equal(3, schemas["forums"].Children.Count);

			var forums = schemas["forums"];
			Assert.True(forums.HasChildren);
			Assert.True(forums.Children.Contains("a"));
			Assert.True(forums.Children.Contains("f"));
			Assert.True(forums.Children.Contains("moderator"));

			var moderator = (Schema)forums["moderator"];
			Assert.True(moderator.HasChildren);
			Assert.NotNull(moderator.Parent);
			Assert.Null(moderator.Paging);
			Assert.NotNull(moderator.Sortings);
			Assert.Equal(1, moderator.Sortings.Length);
			Assert.Equal("name", moderator.Sortings[0].Name);
			Assert.Equal(SortingMode.Ascending, moderator.Sortings[0].Mode);
			Assert.True(moderator.Children.Contains("name"));
			Assert.True(moderator.Children.Contains("avatar"));
		}

		#region 嵌套子类
		public class Schema : SchemaBase
		{
			#region 成员字段
			private Schema _parent;
			private INamedCollection<Schema> _children;
			#endregion

			#region 构造函数
			public Schema(string name) : base(name)
			{
			}
			#endregion

			#region 公共属性
			public Schema Parent
			{
				get
				{
					return _parent;
				}
			}

			public Schema this[string name]
			{
				get
				{
					if(this.HasChildren && this.Children.TryGet(name, out var child))
						return (Schema)child;

					return null;
				}
			}

			public override bool HasChildren
			{
				get
				{
					return _children != null && _children.Count > 0;
				}
			}

			public IReadOnlyNamedCollection<Schema> Children
			{
				get
				{
					return (IReadOnlyNamedCollection<Schema>)_children;
				}
			}
			#endregion

			#region 重写方法
			protected override SchemaBase GetParent()
			{
				return _parent;
			}

			protected override void SetParent(SchemaBase parent)
			{
				_parent = (parent as Schema) ?? throw new ArgumentException();
			}

			protected override bool TryGetChild(string name, out SchemaBase child)
			{
				child = null;

				if(_children != null && _children.TryGet(name, out var schema))
				{
					child = schema;
					return true;
				}

				return false;
			}

			protected override void AddChild(SchemaBase child)
			{
				if(!(child is Schema schema))
					throw new ArgumentException();

				if(_children == null)
					System.Threading.Interlocked.CompareExchange(ref _children, new NamedCollection<Schema>(item => item.Name), null);

				_children.Add(schema);
				schema._parent = this;
			}

			protected override void RemoveChild(string name)
			{
				_children?.Remove(name);
			}

			protected override void ClearChildren()
			{
				_children?.Clear();
			}
			#endregion

			#region 解析方法
			public static Collections.IReadOnlyNamedCollection<Schema> Parse(string text)
			{
				return SchemaBase.Parse(text, token => Resolve(token));
			}

			public static Collections.IReadOnlyNamedCollection<Schema> Parse(string text, Action<string> onError)
			{
				return SchemaBase.Parse(text, token => Resolve(token), onError);
			}
			#endregion

			#region 私有方法
			private static IEnumerable<Schema> Resolve(Token token)
			{
				if(string.IsNullOrWhiteSpace(token.Name))
					throw new InvalidOperationException();

				switch(token.Name)
				{
					case "*":
						return new Schema[]
						{
							new Schema("a"),
							new Schema("b"),
							new Schema("c"),
							new Schema("d"),
							new Schema("e"),
							new Schema("f"),
						};
				}

				return new Schema[] { new Schema(token.Name) };
			}
			#endregion
		}
		#endregion
	}
}
