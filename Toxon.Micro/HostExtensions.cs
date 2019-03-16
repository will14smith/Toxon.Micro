using System;
using System.Threading.Tasks;
using Toxon.Micro.Routing;

namespace Toxon.Micro
{
    public static class HostExtensions
    {
        public static void Add(this Host host, string route, Func<IRequest, Task<IResponse>> handler)
        {
            host.Add(RouterPatternParser.Parse(route), handler);
        }

        public static void Add<TRequest>(this Host host, string route, Func<TRequest, Task<IResponse>> handler)
            where TRequest : IRequest
        {
            host.Add(RouterPatternParser.Parse(route), handler);
        }

        public static void Add<TRequest>(this Host host, IRequestMatcher route, Func<TRequest, Task<IResponse>> handler)
            where TRequest : IRequest
        {
            Task<IResponse> WrappedHandler(IRequest rawRequest)
            {
                if (rawRequest is TRequest request)
                {
                    return handler(request);
                }

                var serializedRequest = host.Serialize(rawRequest);
                request = host.Deserialize<TRequest>(serializedRequest);

                return handler(request);
            }

            host.Add(route, WrappedHandler);
        }

        public static async Task<TResponse> Act<TResponse>(this Host host, IRequest request)
            where TResponse : IResponse
        {
            var rawResponse = await host.Act(request);
            if (rawResponse is TResponse response)
            {
                return response;
            }

            var serializedResponse = host.Serialize(rawResponse);
            response = host.Deserialize<TResponse>(serializedResponse);

            return response;
        }
    }
}
