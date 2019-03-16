using System;
using Toxon.Micro.Transport;

namespace Toxon.Micro.Blog.EntryStore
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new Host();

            host.WithTransport()
                .Listen(new HttpListenConfig(8601) { Pin = "store:*,kind:entry" })
                .Use<BusinessLogic>();

            Console.WriteLine("Running EntryStore... press enter to exit!");
            Console.ReadLine();
        }
    }
}
