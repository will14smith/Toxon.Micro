using System.Text.RegularExpressions;

namespace Toxon.Micro.Routing
{
    public class AnyValueMatcher : IValueMatcher
    {
        public MatchResult Matches(object value) => new AnyValueMatchResult();

        internal class AnyValueMatchResult : MatchResult
        {
            public override bool IsBetterMatchThan(MatchResult other)
            {
                return false;
            }
        }
    }
    public class WildcardValueMatcher : IValueMatcher
    {
        private readonly Regex _match;

        public WildcardValueMatcher(string match)
        {
            _match = new Regex(match.Replace("*", ".*"), RegexOptions.Compiled);
        }

        public MatchResult Matches(object value) => new AnyValueMatchResult();

        private class AnyValueMatchResult : MatchResult
        {
            public override bool IsBetterMatchThan(MatchResult other)
            {
                if (other is AnyValueMatcher.AnyValueMatchResult)
                {
                    return true;
                }

                return false;
            }
        }
    }
}