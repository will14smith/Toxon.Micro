namespace Toxon.Micro.Routing
{
    public class AnyValueMatcher : IValueMatcher
    {
        public MatchResult Matches(object value) => new AnyValueMatchResult();

        public override string ToString()
        {
            return "*";
        }

        internal class AnyValueMatchResult : MatchResult
        {
            public override bool IsBetterMatchThan(MatchResult other)
            {
                return false;
            }
        }
    }
}