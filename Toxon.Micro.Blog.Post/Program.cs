using System;
using Toxon.Micro.Transport;

namespace Toxon.Micro.Blog.Post
{
    class Program
    {
        static void Main(string[] args)
        {
            new Host()
                .WithTransport()
                .Listen(new HttpListenConfig(8602) { Pin = "post:*" })
                .Client(new HttpClientConfig(new Uri("http://localhost:8601")) { Pin = "store:*,kind:entry" })
                .Client(new HttpClientConfig(new Uri("http://localhost:8603")) { Pin = "info:entry", Mode = RouteMode.Observe })
                .Use<BusinessLogic>();

            Console.WriteLine("Running Post... press enter to exit!");
            Console.ReadLine();
        }
    }
}
