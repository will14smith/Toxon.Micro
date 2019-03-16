using System;
using System.Threading.Tasks;

namespace Toxon.Micro
{
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
}