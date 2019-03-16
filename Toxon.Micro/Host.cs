using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Toxon.Micro.Routing;

namespace Toxon.Micro
{
    public class Host
    {
        private readonly Router<RouteHandler> _router = new Router<RouteHandler>();
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

        public Host Add(IRequestMatcher route, RouteHandler handler)
        {
            _router.Add(route, handler);
            return this;
        }

        public async Task Broadcast(IRequest request)
        {
            var matches = _router.Matches(request);

            foreach (var match in matches)
            {
                await match.Handler(request, new RequestMeta(new RouteHandler[0]));
            }
        }

        public async Task<object> Act(IRequest request)
        {
            var matches = _router.Matches(request);

            var hasConsumedMatch = false;
            object result = default;

            foreach (var match in matches)
            {
                switch (match.Mode)
                {
                    case RouteMode.Consume:
                        if (hasConsumedMatch) continue;

                        var remainingConsumeMatches = matches.Where(x => x.Mode == RouteMode.Consume).Skip(1).ToList();
                        result = await match.Handler(request, new RequestMeta(remainingConsumeMatches));

                        hasConsumedMatch = true;
                        break;

                    case RouteMode.Observe:
                        await match.Handler(request, new RequestMeta(new RouteHandler[0]));
                        break;

                    default: throw new ArgumentOutOfRangeException();
                }
            }

            if (!hasConsumedMatch)
            {
                throw new Exception("Consuming handler was not found");
            }

            return result;
        }

        internal string Serialize(object value) => JsonConvert.SerializeObject(value, _serializerSettings);
        internal T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized, _serializerSettings);
    }

    public class RouteHandler
    {
        public RouteHandler(Func<IRequest, RequestMeta, Task<object>> handler, RouteMode mode = RouteMode.Consume)
        {
            Handler = handler;
            Mode = mode;
        }

        public RouteMode Mode { get; }
        public Func<IRequest, RequestMeta, Task<object>> Handler { get; }
    }

    public enum RouteMode
    {
        Consume,
        Observe
    }

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
