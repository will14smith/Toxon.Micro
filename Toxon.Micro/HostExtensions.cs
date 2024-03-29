﻿using System;
using System.Threading.Tasks;
using Toxon.Micro.Routing;

namespace Toxon.Micro
{
    public static class HostExtensions
    {
        public static Host Add(this Host host, IRequestMatcher route, Func<IRequest, Task<object>> handler, RouteMode mode = RouteMode.Consume)
        {
            return host.Add(route, new RouteHandler((message, _) => handler(message), mode));
        }

        public static Host Add(this Host host, string route, Func<IRequest, Task<object>> handler, RouteMode mode = RouteMode.Consume)
        {
            return host.Add(RouterPatternParser.Parse(route), handler, mode);
        }

        public static Host Add(this Host host, string route, Func<IRequest, RequestMeta, Task<object>> handler, RouteMode mode = RouteMode.Consume)
        {
            return host.Add(RouterPatternParser.Parse(route), handler, mode);
        }

        public static Host Add<TRequest>(this Host host, string route, Func<TRequest, Task<object>> handler, RouteMode mode = RouteMode.Consume)
            where TRequest : IRequest
        {
            return host.Add(RouterPatternParser.Parse(route), handler, mode);
        }

        public static Host Add<TRequest>(this Host host, string route, Func<TRequest, RequestMeta, Task<object>> handler, RouteMode mode = RouteMode.Consume)
            where TRequest : IRequest
        {
            return host.Add(RouterPatternParser.Parse(route), handler, mode);
        }

        public static Host Add<TRequest>(this Host host, IRequestMatcher route, Func<TRequest, Task<object>> handler, RouteMode mode = RouteMode.Consume)
            where TRequest : IRequest
        {
            return host.Add<TRequest>(route, (message, _) => handler(message), mode);
        }

        public static Host Add<TRequest>(this Host host, IRequestMatcher route, Func<TRequest, RequestMeta, Task<object>> handler, RouteMode mode = RouteMode.Consume)
            where TRequest : IRequest
        {
            Task<object> WrappedHandler(IRequest rawRequest, RequestMeta meta)
            {
                if (rawRequest is TRequest request)
                {
                    return handler(request, meta);
                }

                var serializedRequest = host.Serialize(rawRequest);
                request = host.Deserialize<TRequest>(serializedRequest);

                return handler(request, meta);
            }

            return host.Add(route, new RouteHandler(WrappedHandler, mode));
        }

        public static async Task<TResponse> Act<TResponse>(this Host host, IRequest request)
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
