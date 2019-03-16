using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toxon.Micro
{
    public class RequestMeta
    {
        private readonly IReadOnlyList<RouteHandler> _remainingConsumeMatches;

        public RequestMeta(IReadOnlyList<RouteHandler> remainingConsumeMatches)
        {
            _remainingConsumeMatches = remainingConsumeMatches;
        }

        public bool TryPrior(IRequest request, out Task<object> prior)
        {
            if (_remainingConsumeMatches.Count <= 1)
            {
                prior = null;
                return false;
            }

            var priorMatch = _remainingConsumeMatches[1];
            var newPriors = _remainingConsumeMatches.Skip(1).ToList();

            prior = priorMatch.Handler(request, new RequestMeta(newPriors));
            return true;
        }
    }
}