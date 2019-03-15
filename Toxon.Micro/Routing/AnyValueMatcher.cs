using System;

namespace Toxon.Micro.Routing
{
    public class AnyValueMatcher : IValueMatcher
    {
        public MatchResult Matches(object value) => new AnyValueMatchResult();

        private class AnyValueMatchResult : MatchResult
        {
            public override bool IsBetterMatchThan(MatchResult other)
            {
                throw new NotImplementedException();
            }
        }
    }
}