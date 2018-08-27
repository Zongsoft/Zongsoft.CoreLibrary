using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using Zongsoft.Collections;

namespace Zongsoft.Data
{
	public class ScopeTest
	{
		[Fact]
		public void Test1()
		{
			Collections.IReadOnlyNamedCollection<Scope> scopes;

			scopes = Scope.Parse(null);
			Assert.Null(scopes);

			scopes = Scope.Parse(" \t ");
			Assert.Null(scopes);

			scopes = Scope.Parse("*");
			Assert.Equal(6, scopes.Count);
			Assert.True(scopes.Contains("a"));
			Assert.True(scopes.Contains("b"));
			Assert.True(scopes.Contains("c"));
			Assert.True(scopes.Contains("d"));
			Assert.True(scopes.Contains("e"));
			Assert.True(scopes.Contains("f"));

			scopes = Scope.Parse("*, !");
			Assert.Equal(0, scopes.Count);

			scopes = Scope.Parse("*, !a, !c, !f, c");
			Assert.Equal(4, scopes.Count);
			Assert.False(scopes.Contains("a"));
			Assert.True(scopes.Contains("b"));
			Assert.True(scopes.Contains("c"));
			Assert.True(scopes.Contains("d"));
			Assert.True(scopes.Contains("e"));
			Assert.False(scopes.Contains("f"));
		}

		[Fact]
		public void Test2()
		{
			Collections.IReadOnlyNamedCollection<Scope> scopes;

			scopes = Scope.Parse("*, !, a, !b, c, a, forums:100/2(*, !, a, !b, f, moderator(name, avatar))");
			Assert.NotEmpty(scopes);
			Assert.Equal(3, scopes.Count);

			Assert.True(scopes.Contains("a"));
			Assert.Null(scopes["a"].Parent);
			Assert.Null(scopes["a"].Paging);
			Assert.Null(scopes["a"].Sortings);
			Assert.False(scopes["a"].HasChildren);

			Assert.True(scopes.Contains("c"));
			Assert.Null(scopes["c"].Parent);
			Assert.Null(scopes["c"].Paging);
			Assert.Null(scopes["c"].Sortings);
			Assert.False(scopes["c"].HasChildren);

			Assert.True(scopes.Contains("forums"));
			Assert.Null(scopes["forums"].Parent);
			Assert.Null(scopes["forums"].Sortings);
			Assert.NotNull(scopes["forums"].Paging);
			Assert.Equal(100, scopes["forums"].Paging.PageIndex);
			Assert.Equal(2, scopes["forums"].Paging.PageSize);
			Assert.True(scopes["forums"].HasChildren);
			Assert.Equal(3, scopes["forums"].Children.Count);

			var forums = (Scope)scopes["forums"];
			Assert.True(forums.HasChildren);
			Assert.True(forums.Children.Contains("a"));
			Assert.True(forums.Children.Contains("f"));
			Assert.True(forums.Children.Contains("moderator"));

			var moderator = (Scope)forums["moderator"];
			Assert.NotNull(moderator.Parent);
			Assert.True(moderator.HasChildren);
			Assert.True(moderator.Children.Contains("name"));
			Assert.True(moderator.Children.Contains("avatar"));
		}

		[Fact]
		public void Test3()
		{
			Collections.IReadOnlyNamedCollection<Scope> scopes;

			scopes = Scope.Parse("*, !, a, !b, c, a, forums:100/2[~timestamp, id](*, !, a, !b, f, moderator[name](name, avatar))");
			Assert.NotEmpty(scopes);
			Assert.Equal(3, scopes.Count);

			Assert.True(scopes.Contains("a"));
			Assert.Null(scopes["a"].Parent);
			Assert.Null(scopes["a"].Paging);
			Assert.Null(scopes["a"].Sortings);
			Assert.False(scopes["a"].HasChildren);

			Assert.True(scopes.Contains("c"));
			Assert.Null(scopes["c"].Parent);
			Assert.Null(scopes["c"].Paging);
			Assert.Null(scopes["c"].Sortings);
			Assert.False(scopes["c"].HasChildren);

			Assert.True(scopes.Contains("forums"));
			Assert.Null(scopes["forums"].Parent);
			Assert.NotNull(scopes["forums"].Paging);
			Assert.Equal(100, scopes["forums"].Paging.PageIndex);
			Assert.Equal(2, scopes["forums"].Paging.PageSize);
			Assert.NotNull(scopes["forums"].Sortings);
			Assert.Equal(2, scopes["forums"].Sortings.Length);
			Assert.Equal("timestamp", scopes["forums"].Sortings[0].Name);
			Assert.Equal(SortingMode.Descending, scopes["forums"].Sortings[0].Mode);
			Assert.Equal("id", scopes["forums"].Sortings[1].Name);
			Assert.Equal(SortingMode.Ascending, scopes["forums"].Sortings[1].Mode);
			Assert.True(scopes["forums"].HasChildren);
			Assert.Equal(3, scopes["forums"].Children.Count);

			var forums = scopes["forums"];
			Assert.True(forums.HasChildren);
			Assert.True(forums.Children.Contains("a"));
			Assert.True(forums.Children.Contains("f"));
			Assert.True(forums.Children.Contains("moderator"));

			var moderator = (Scope)forums["moderator"];
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
		public class Scope : Zongsoft.Data.ScopeBase
		{
			#region 构造函数
			public Scope(string name) : base(name)
			{
			}
			#endregion

			#region 公共属性
			public Scope Parent
			{
				get
				{
					return (Scope)base.GetParent();
				}
			}

			public Scope this[string name]
			{
				get
				{
					if(this.HasChildren && this.Children.TryGet(name, out var child))
						return (Scope)child;

					return null;
				}
			}
			#endregion

			#region 解析方法
			public static Collections.IReadOnlyNamedCollection<Scope> Parse(string text)
			{
				return ScopeBase.Parse(text, token => Resolve(token));
			}

			public static Collections.IReadOnlyNamedCollection<Scope> Parse(string text, Action<string> onError)
			{
				return ScopeBase.Parse(text, token => Resolve(token), onError);
			}
			#endregion

			#region 私有方法
			private static IEnumerable<Scope> Resolve(Token token)
			{
				if(string.IsNullOrWhiteSpace(token.Name))
					throw new InvalidOperationException();

				switch(token.Name)
				{
					case "*":
						return new Scope[]
						{
							new Scope("a"),
							new Scope("b"),
							new Scope("c"),
							new Scope("d"),
							new Scope("e"),
							new Scope("f"),
						};
				}

				return new Scope[] { new Scope(token.Name) };
			}
			#endregion
		}
		#endregion
	}
}
