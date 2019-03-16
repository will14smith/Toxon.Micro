using System.Linq;

namespace Toxon.Micro.Routing
{
    public static class RouterExtensions
    {
        public static void Add<TData>(this Router<TData> router, string route, TData data)
        {
            router.Add(RouterPatternParser.Parse(route), data);
        }

        public static TData Match<TData>(this Router<TData> router, IRequest request)
        {
            return router.Matches(request).First();
        }
    }
}
