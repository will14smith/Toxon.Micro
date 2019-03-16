using System;
using System.Collections;
using System.Collections.Generic;

namespace Toxon.Micro.Routing
{
    public class EqualityValueMatcher : IValueMatcher
    {
        public object MatchValue { get; }
        public IEqualityComparer Equality { get; }

        public EqualityValueMatcher(object matchValue)
            : this(matchValue, EqualityComparer<object>.Default) { }
        public EqualityValueMatcher(object matchValue, IEqualityComparer equality)
        {
            MatchValue = matchValue;
            Equality = equality;
        }

        public MatchResult Matches(object value)
        {
            if (!Equality.Equals(MatchValue, value))
            {
                return MatchResult.NoMatch;
            }

            return new EqualityValueMatchResult();
        }

        private class EqualityValueMatchResult : MatchResult
        {
            public override bool IsBetterMatchThan(MatchResult other)
            {
                if (other is EqualityValueMatchResult) return false;

                return !other.IsBetterMatchThan(this);
            }
        }
    }
}