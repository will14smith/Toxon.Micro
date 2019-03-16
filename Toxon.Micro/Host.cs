using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Toxon.Micro.Routing;

namespace Toxon.Micro
{
    public class Host
    {
        private readonly Router<Func<IRequest, Task<IResponse>>> _router = new Router<Func<IRequest, Task<IResponse>>>();
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

        public void Add(IRequestMatcher route, Func<IRequest, Task<IResponse>> handler)
        {
            _router.Add(route, handler);
        }

        public Task<IResponse> Act(IRequest request)
        {
            var chosenHandler = _router.Match(request);
            if (chosenHandler == null)
            {
                throw new Exception("Handler not found");
            }

            return chosenHandler(request);
        }

        internal string Serialize(object value) => JsonConvert.SerializeObject(value, _serializerSettings);
        internal T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized, _serializerSettings);
    }
}
