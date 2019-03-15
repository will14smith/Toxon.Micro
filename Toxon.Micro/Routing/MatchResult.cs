using System;

namespace Toxon.Micro.Routing
{
    public abstract class MatchResult
    {
        public static readonly MatchResult NoMatch = new NonMatchResult();

        public virtual bool Matched => true;

        public abstract bool IsBetterMatchThan(MatchResult other);

        internal class NonMatchResult : MatchResult
        {
            public override bool Matched => false;
            public override bool IsBetterMatchThan(MatchResult other)
            {
                throw new NotSupportedException();
            }
        }
    }
}
