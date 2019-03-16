using System.Collections.Generic;
using System.Linq;

namespace Toxon.Micro.Routing
{
    public class Router<TData>
    {
        private readonly List<(IRequestMatcher Route, TData Data)> _routes = new List<(IRequestMatcher, TData)>();

        public void Add(IRequestMatcher route, TData data)
        {
            _routes.Add((route, data));
        }

        public IReadOnlyList<TData> Matches(IRequest request)
        {
            return _routes
                .Select(x => (Match: x.Route.Matches(request), Data: x.Data))
                .Where(x => x.Match.Matched)
                .OrderByDescending(x => x.Match, new MatchComparer())
                .Select(x => x.Data)
                .ToList();
        }
    }

    public class MatchComparer : IComparer<MatchResult>
    {
        public int Compare(MatchResult x, MatchResult y)
        {
            if (x.IsBetterMatchThan(y)) return 1;
            if (y.IsBetterMatchThan(x)) return -1;

            return 0;

        }
    }
}
