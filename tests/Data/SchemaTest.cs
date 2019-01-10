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
			INamedCollection<SchemaMember> schemas;

			schemas = SchemaParser.Instance.Parse(null);
			Assert.Null(schemas);

			schemas = SchemaParser.Instance.Parse(" \t ");
			Assert.Null(schemas);

			schemas = SchemaParser.Instance.Parse("*");
			Assert.Equal(6, schemas.Count);
			Assert.True(schemas.Contains("a"));
			Assert.True(schemas.Contains("b"));
			Assert.True(schemas.Contains("c"));
			Assert.True(schemas.Contains("d"));
			Assert.True(schemas.Contains("e"));
			Assert.True(schemas.Contains("f"));

			schemas = SchemaParser.Instance.Parse("*, !");
			Assert.Equal(0, schemas.Count);

			schemas = SchemaParser.Instance.Parse(" ,  ,  *, ,,!a, !c, !f, c");
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
			INamedCollection<SchemaMember> schemas;

			schemas = SchemaParser.Instance.Parse("*, !, a, !b, c, a, forums:100/2{*, !, a, !b, f, moderator{name, avatar}}");
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

			var forums = schemas["forums"];
			Assert.True(forums.HasChildren);
			Assert.True(forums.Children.Contains("a"));
			Assert.True(forums.Children.Contains("f"));
			Assert.True(forums.Children.Contains("moderator"));

			var moderator = forums["moderator"];
			Assert.NotNull(moderator.Parent);
			Assert.True(moderator.HasChildren);
			Assert.True(moderator.Children.Contains("name"));
			Assert.True(moderator.Children.Contains("avatar"));
		}

		[Fact]
		public void Test3()
		{
			INamedCollection<SchemaMember> schemas;

			schemas = SchemaParser.Instance.Parse(@"*, !, a, !b, c, a, forums:100/2(~timestamp, id){*, !, a, !b, f, moderator(name){name, avatar}}");
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

			var moderator = forums["moderator"];
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
		public class SchemaMember : SchemaMemberBase
		{
			#region 成员字段
			private SchemaMember _parent;
			private INamedCollection<SchemaMember> _children;
			#endregion

			#region 构造函数
			public SchemaMember(string name) : base(name)
			{
			}
			#endregion

			#region 公共属性
			public SchemaMember Parent
			{
				get
				{
					return _parent;
				}
			}

			public SchemaMember this[string name]
			{
				get
				{
					if(this.HasChildren && this.Children.TryGet(name, out var child))
						return (SchemaMember)child;

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

			public IReadOnlyNamedCollection<SchemaMember> Children
			{
				get
				{
					return (IReadOnlyNamedCollection<SchemaMember>)_children;
				}
			}
			#endregion

			#region 重写方法
			protected override SchemaMemberBase GetParent()
			{
				return _parent;
			}

			protected override void SetParent(SchemaMemberBase parent)
			{
				_parent = (parent as SchemaMember) ?? throw new ArgumentException();
			}

			protected override bool TryGetChild(string name, out SchemaMemberBase child)
			{
				child = null;

				if(_children != null && _children.TryGet(name, out var schema))
				{
					child = schema;
					return true;
				}

				return false;
			}

			protected override void AddChild(SchemaMemberBase child)
			{
				if(!(child is SchemaMember schema))
					throw new ArgumentException();

				if(_children == null)
					System.Threading.Interlocked.CompareExchange(ref _children, new NamedCollection<SchemaMember>(item => item.Name), null);

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
		}

		public class SchemaParser : SchemaParserBase<SchemaMember>
		{
			#region 单例字段
			public static readonly SchemaParser Instance = new SchemaParser();
			#endregion

			#region 构造函数
			private SchemaParser()
			{
			}
			#endregion

			#region 解析方法
			public INamedCollection<SchemaMember> Parse(string expression)
			{
				return base.Parse(expression, token => Resolve(token), null);
			}

			public override ISchema<SchemaMember> Parse(string name, string expression, Type entityType)
			{
				throw new NotImplementedException();
			}
			#endregion

			#region 私有方法
			private static IEnumerable<SchemaMember> Resolve(SchemaEntryToken token)
			{
				if(string.IsNullOrWhiteSpace(token.Name))
					throw new InvalidOperationException();

				switch(token.Name)
				{
					case "*":
						return new SchemaMember[]
						{
							new SchemaMember("a"),
							new SchemaMember("b"),
							new SchemaMember("c"),
							new SchemaMember("d"),
							new SchemaMember("e"),
							new SchemaMember("f"),
						};
				}

				return new SchemaMember[] { new SchemaMember(token.Name) };
			}
			#endregion
		}
		#endregion
	}
}
