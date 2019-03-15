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

        public bool Matches(object value)
        {
            return Equality.Equals(MatchValue, value);
        }
    }
}