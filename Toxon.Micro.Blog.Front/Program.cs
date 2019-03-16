using Microsoft.AspNetCore.Hosting;
using System;
using Toxon.Micro.Transport;
using Microsoft.Extensions.DependencyInjection;
using Toxon.Micro.Blog.Front.Http;

namespace Toxon.Micro.Blog.Front
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new Host()
                .WithTransport()
                .Client(new HttpClientConfig(new Uri("http://localhost:8601")) { Pin = "store:*,kind:entry" })
                .Client(new HttpClientConfig(new Uri("http://localhost:8602")) { Pin = "post:*" })
            ;

            new WebHostBuilder()
                .UseKestrel(k => k.ListenLocalhost(8500))
                .ConfigureServices(services => services.AddSingleton(host))
                .UseStartup<Startup>()
                .Start();

            Console.WriteLine("Running Front... press enter to exit!");
            Console.ReadLine();
        }
    }
}
