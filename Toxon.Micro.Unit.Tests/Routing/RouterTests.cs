using Toxon.Micro.Routing;
using Xunit;

namespace Toxon.Micro.Unit.Tests.Routing
{
    public class RouterTests
    {
        [Theory]

        [InlineData(1, null, "A")]
        [InlineData(1, 1, "B")]
        [InlineData(1, 2, "C")]
        [InlineData(2, 2, "D")]
        [InlineData(2, null, null)]
        [InlineData(2, 1, null)]
        [InlineData(null, 1, null)]
        public void TestEqualityPatterns(int? x, int? y, string expected)
        {
            var router = new Router<string>();
            router.Add(RouterPatternParser.Parse("x:1"), "A");
            router.Add(RouterPatternParser.Parse("x:1,y:1"), "B");
            router.Add(RouterPatternParser.Parse("x:1,y:2"), "C");
            router.Add(RouterPatternParser.Parse("y:2"), "D");

            var response = router.Match(new Request1 { X = x?.ToString(), Y = y?.ToString() });

            Assert.Equal(expected, response);
        }

        [Theory]

        [InlineData(0, null, null, "A")]
        [InlineData(null, 1, null, "B")]
        [InlineData(null, null, 2, "C")]
        [InlineData(0, 1, null, "AB")]
        [InlineData(0, null, 2, "A")]
        [InlineData(null, 1, 2, "B")]
        public void TestOrdering(int? a, int? b, int? c, string expected)
        {
            var router = new Router<string>();
            router.Add(RouterPatternParser.Parse("a:0"), "A");
            router.Add(RouterPatternParser.Parse("b:1"), "B");
            router.Add(RouterPatternParser.Parse("c:2"), "C");
            router.Add(RouterPatternParser.Parse("a:0,b:1"), "AB");

            var response = router.Match(new Request2
            {
                A = a?.ToString(),
                B = b?.ToString(),
                C = c?.ToString(),
            });

            Assert.Equal(expected, response);
        }

        [Theory]

        [InlineData(0, null, null, "A")]
        [InlineData(1, null, null, "AA")]
        [InlineData(null, 1, "xhy", "BC")]
        public void TestOrderingGlob(int? a, int? b, string c, string expected)
        {
            var router = new Router<string>();
            router.Add(RouterPatternParser.Parse("a:0"), "A");
            router.Add(RouterPatternParser.Parse("a:*"), "AA");
            router.Add(RouterPatternParser.Parse("b:1,c:x*y"), "BC");

            var response = router.Match(new Request2
            {
                A = a?.ToString(),
                B = b?.ToString(),
                C = c,
            });

            Assert.Equal(expected, response);
        }

        private class Request1 : IRequest
        {
            public string X { get; set; }
            public string Y { get; set; }
        }

        private class Request2 : IRequest
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
        }
    }
}
