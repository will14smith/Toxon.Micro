using Microsoft.Extensions.Hosting;

namespace Toxon.Micro
{
    public abstract class HostPlugin
    {
        public abstract void Init(Host host);
    }

    public static class HostPluginExtensions
    {
        public static Host Use<TPlugin>(this Host host)
            where TPlugin : HostPlugin, new()
        {
            var plugin = new TPlugin();
            plugin.Init(host);

            return host;
        }

        public static Host Use(this Host host, HostPlugin plugin)
        {
            plugin.Init(host);

            return host;
        }
    }
}
