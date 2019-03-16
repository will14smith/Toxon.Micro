using System;
using System.Collections.Generic;
using Toxon.Micro.Blog.Search.Inbound;
using Toxon.Micro.Transport;

namespace Toxon.Micro.Blog.Search
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new Host();

            host.WithTransport()
                .Listen(new HttpListenConfig(8603) { Pin = "info:entry" })
                .Add<InfoEntryRequest>("info:entry", request =>
                {
                    return host.Act(new SearchInsertRequest
                    {
                        Kind = "entry",
                        Id = request.Id,

                        Fields = new Dictionary<string, string>
                        {
                            {"User", request.User},
                            {"Text", request.Text},
                        }
                    });
                }, RouteMode.Observe)
                .Use<BusinessLogic>();

            Console.WriteLine("Running Search... press enter to exit!");
            Console.ReadLine();
        }
    }
}
