namespace Toxon.Micro.Routing
{
    public static class RouterExtensions
    {
        public static void Add<TData>(this Router<TData> router, string route, TData data)
        {
            router.Add(RouterPatternParser.Parse(route), data);
        }
    }
}
