using System.Text.RegularExpressions;

namespace Toxon.Micro.Routing
{
    public class WildcardValueMatcher : IValueMatcher
    {
        private readonly Regex _match;

        public WildcardValueMatcher(string match)
        {
            _match = new Regex(match.Replace("*", ".*"), RegexOptions.Compiled);
        }

        public MatchResult Matches(object value) => new WildcardValueMatchResult();

        public override string ToString()
        {
            return _match.ToString();
        }

        private class WildcardValueMatchResult : MatchResult
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