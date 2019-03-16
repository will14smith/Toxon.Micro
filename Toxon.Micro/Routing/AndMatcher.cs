using System;
using System.Collections.Generic;
using System.Linq;

namespace Toxon.Micro.Routing
{
    public class AndMatcher : IRequestMatcher
    {
        private readonly IReadOnlyCollection<IRequestMatcher> _requestMatchers;

        public AndMatcher(params IRequestMatcher[] requestMatchers)
        {
            _requestMatchers = requestMatchers;
        }

        public MatchResult Matches(IRequest request)
        {
            var results = new List<MatchResult>();
            foreach (var matcher in _requestMatchers)
            {
                var result = matcher.Matches(request);
                if (!result.Matched)
                {
                    return MatchResult.NoMatch;
                }
                results.Add(result);
            }

            return new AndMatchResult(results);
        }

        public override string ToString()
        {
            return $"&& {string.Join(" ", _requestMatchers.Select(x => $"({x})"))}";
        }

        private class AndMatchResult : MatchResult
        {
            private readonly IReadOnlyList<MatchResult> _results;

            public AndMatchResult(IReadOnlyList<MatchResult> results)
            {
                _results = results;
            }

            public override bool IsBetterMatchThan(MatchResult other)
            {
                if (_results.Count == 1)
                {
                    return _results[0].IsBetterMatchThan(other);
                }

                if (other is AndMatchResult otherAnd)
                {
                    throw new NotImplementedException();
                }

                return _results.Count > 1;
            }
        }
    }
}
