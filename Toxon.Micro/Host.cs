using System;
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
                throw new NotMatchingHandlerException(request);
            }

            return result;
        }

        internal string Serialize(object value) => JsonConvert.SerializeObject(value, _serializerSettings);
        internal T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized, _serializerSettings);
    }
}
