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
        private readonly Router<Func<IRequest, RequestMeta, Task<object>>> _router = new Router<Func<IRequest, RequestMeta, Task<object>>>();
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

        public Host Add(IRequestMatcher route, Func<IRequest, RequestMeta, Task<object>> handler)
        {
            _router.Add(route, handler);
            return this;
        }

        public Task<object> Act(IRequest request)
        {
            var matches = _router.Matches(request);
            if (matches.Count == 0)
            {
                throw new Exception("Handler not found");
            }

            return matches.First()(request, new RequestMeta(matches));
        }

        internal string Serialize(object value) => JsonConvert.SerializeObject(value, _serializerSettings);
        internal T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized, _serializerSettings);
    }

    public class RequestMeta
    {
        private readonly IReadOnlyList<Func<IRequest, RequestMeta, Task<object>>> _matches;

        public RequestMeta(IReadOnlyList<Func<IRequest, RequestMeta, Task<object>>> matches)
        {
            _matches = matches;
        }

        public bool TryPrior(IRequest request, out Task<object> prior)
        {
            if (_matches.Count <= 1)
            {
                prior = null;
                return false;
            }

            var priorMatch = _matches[1];
            var newPriors = _matches.Skip(1).ToList();

            prior = priorMatch(request, new RequestMeta(newPriors));
            return true;
        }
    }
}
