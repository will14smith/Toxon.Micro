using System.Collections.Generic;

namespace Toxon.Micro.Routing
{
    public class Router<TData>
    {
        private readonly List<(IRequestMatcher Route, TData Data)> _routes = new List<(IRequestMatcher, TData)>();

        public void Add(IRequestMatcher route, TData data)
        {
            _routes.Add((route, data));
        }

        public TData Match(IRequest request)
        {
            TData chosenData = default;
            MatchResult chosenMatch = null;

            foreach (var (route, data) in _routes)
            {
                var match = route.Matches(request);
                if (!match.Matched)
                {
                    continue;
                }

                if (chosenMatch != null && !match.IsBetterMatchThan(chosenMatch))
                {
                    continue;
                }

                chosenData = data;
                chosenMatch = match;
            }

            return chosenData;
        }
    }
}
