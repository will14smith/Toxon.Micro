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
        private readonly List<RequestHandler> _handlers = new List<RequestHandler>();
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

        public void Add(IRequestMatcher requestMatcher, Func<IRequest, Task<IResponse>> handler)
        {
            _handlers.Add(new RequestHandler(requestMatcher, handler));
        }
        public void Add<TRequest>(IRequestMatcher requestMatcher, Func<TRequest, Task<IResponse>> handler)
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

            _handlers.Add(new RequestHandler(requestMatcher, WrappedHandler));
        }

        public Task<IResponse> Act(IRequest request)
        {
            var handler = _handlers.Single(x => x.RequestMatcher.Matches(request));

            return handler.Handler(request);
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

    internal class RequestHandler
    {
        public RequestHandler(IRequestMatcher requestMatcher, Func<IRequest, Task<IResponse>> handler)
        {
            RequestMatcher = requestMatcher;
            Handler = handler;
        }

        public IRequestMatcher RequestMatcher { get; }
        public Func<IRequest, Task<IResponse>> Handler { get; }
    }
}
