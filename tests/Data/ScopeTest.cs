using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

namespace Zongsoft.Data
{
	public class ScopeTest
	{
		[Fact]
		public void Test1()
		{
			Collections.IReadOnlyNamedCollection<Scope.Segment> segments;

			segments = Scope.Resolve(null, token => null);
			Assert.Null(segments);

			segments = Scope.Resolve(" \t ", token => null);
			Assert.Null(segments);

			segments = Scope.Resolve("*", token => Resolve(token));
			Assert.Equal(6, segments.Count);
			Assert.True(segments.Contains("a"));
			Assert.True(segments.Contains("b"));
			Assert.True(segments.Contains("c"));
			Assert.True(segments.Contains("d"));
			Assert.True(segments.Contains("e"));
			Assert.True(segments.Contains("f"));

			segments = Scope.Resolve("*, !", token => Resolve(token));
			Assert.Equal(0, segments.Count);

			segments = Scope.Resolve("*, !a, !c, !f, c", token => Resolve(token));
			Assert.Equal(4, segments.Count);
			Assert.False(segments.Contains("a"));
			Assert.True(segments.Contains("b"));
			Assert.True(segments.Contains("c"));
			Assert.True(segments.Contains("d"));
			Assert.True(segments.Contains("e"));
			Assert.False(segments.Contains("f"));

		}

		[Fact]
		public void Test2()
		{
			Collections.IReadOnlyNamedCollection<Scope.Segment> segments;

			segments = Scope.Resolve("*, !, a, !b, c, a, forums:100/2(*, !, a, !b, f, moderator(name, avatar))", token => Resolve(token));
			Assert.NotEmpty(segments);
			Assert.Equal(3, segments.Count);

			Assert.True(segments.Contains("a"));
			Assert.Null(segments["a"].Parent);
			Assert.Null(segments["a"].Paging);
			Assert.Null(segments["a"].Sortings);
			Assert.False(segments["a"].HasChildren);

			Assert.True(segments.Contains("c"));
			Assert.Null(segments["c"].Parent);
			Assert.Null(segments["c"].Paging);
			Assert.Null(segments["c"].Sortings);
			Assert.False(segments["c"].HasChildren);

			Assert.True(segments.Contains("forums"));
			Assert.Null(segments["forums"].Parent);
			Assert.Null(segments["forums"].Sortings);
			Assert.NotNull(segments["forums"].Paging);
			Assert.Equal(100, segments["forums"].Paging.PageIndex);
			Assert.Equal(2, segments["forums"].Paging.PageSize);
			Assert.True(segments["forums"].HasChildren);
			Assert.Equal(3, segments["forums"].Children.Count);

			var forums = segments["forums"];
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
			Collections.IReadOnlyNamedCollection<Scope.Segment> segments;

			segments = Scope.Resolve("*, !, a, !b, c, a, forums:100/2[~timestamp, id](*, !, a, !b, f, moderator[name](name, avatar))", token => Resolve(token));
			Assert.NotEmpty(segments);
			Assert.Equal(3, segments.Count);

			Assert.True(segments.Contains("a"));
			Assert.Null(segments["a"].Parent);
			Assert.Null(segments["a"].Paging);
			Assert.Null(segments["a"].Sortings);
			Assert.False(segments["a"].HasChildren);

			Assert.True(segments.Contains("c"));
			Assert.Null(segments["c"].Parent);
			Assert.Null(segments["c"].Paging);
			Assert.Null(segments["c"].Sortings);
			Assert.False(segments["c"].HasChildren);

			Assert.True(segments.Contains("forums"));
			Assert.Null(segments["forums"].Parent);
			Assert.NotNull(segments["forums"].Paging);
			Assert.Equal(100, segments["forums"].Paging.PageIndex);
			Assert.Equal(2, segments["forums"].Paging.PageSize);
			Assert.NotNull(segments["forums"].Sortings);
			Assert.Equal(2, segments["forums"].Sortings.Length);
			Assert.Equal("timestamp", segments["forums"].Sortings[0].Name);
			Assert.Equal(SortingMode.Descending, segments["forums"].Sortings[0].Mode);
			Assert.Equal("id", segments["forums"].Sortings[1].Name);
			Assert.Equal(SortingMode.Ascending, segments["forums"].Sortings[1].Mode);
			Assert.True(segments["forums"].HasChildren);
			Assert.Equal(3, segments["forums"].Children.Count);

			var forums = segments["forums"];
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

		#region 私有方法
		private IEnumerable<Scope.Segment> Resolve(Scope.SegmentToken token)
		{
			if(string.IsNullOrWhiteSpace(token.Name))
				throw new InvalidOperationException();

			switch(token.Name)
			{
				case "*":
					return new Scope.Segment[]
					{
						new Scope.Segment("a"),
						new Scope.Segment("b"),
						new Scope.Segment("c"),
						new Scope.Segment("d"),
						new Scope.Segment("e"),
						new Scope.Segment("f"),
					};
			}

			return new Scope.Segment[] { new Scope.Segment(token.Name) };
		}
		#endregion
	}
}
