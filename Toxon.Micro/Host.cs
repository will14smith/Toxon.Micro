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

        public void Add(IRequestMatcher router, Func<IRequest, Task<IResponse>> handler)
        {
            _router.Add(router, handler);
        }
        public void Add<TRequest>(IRequestMatcher router, Func<TRequest, Task<IResponse>> handler)
            where TRequest : IRequest
        {
            Task<IResponse> WrappedHandler(IRequest rawRequest)
            {
                if (rawRequest is TRequest request)
                {
                    return handler(request);
                }

                var serializedRequest = Serialize(rawRequest);
                request = Deserialize<TRequest>(serializedRequest);

                return handler(request);
            }

            _router.Add(router, WrappedHandler);
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
        public async Task<TResponse> Act<TResponse>(IRequest request)
            where TResponse : IResponse
        {
            var rawResponse = await Act(request);
            if (rawResponse is TResponse response)
            {
                return response;
            }

            var serializedResponse = Serialize(rawResponse);
            response = Deserialize<TResponse>(serializedResponse);

            return response;
        }

        private string Serialize(object value) => JsonConvert.SerializeObject(value, _serializerSettings);
        private T Deserialize<T>(string serialized) => JsonConvert.DeserializeObject<T>(serialized, _serializerSettings);
    }
}
