using System.Linq;

namespace Toxon.Micro.Routing
{
    public class RouterPatternParser
    {
        public static IRequestMatcher Parse(string pattern)
        {
            var sections = pattern.Split(',');
            var matchers = sections.Select(section =>
            {
                var parts = section.Split(new[] {':'}, 2);

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                var valueMatcher = value == "*" ? (IValueMatcher) new AnyValueMatcher() : new EqualityValueMatcher(value);

                return (IRequestMatcher)new FieldMatcher(key, valueMatcher);
            }).ToArray();

            return matchers.Length == 1 ? matchers.Single() : new AndMatcher(matchers);
        }
    }
}
