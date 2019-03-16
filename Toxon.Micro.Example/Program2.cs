using System;
using System.Threading.Tasks;
using Toxon.Micro.Transport;

namespace Toxon.Micro.Example
{
    class Program2
    {
        public static async Task Main(string[] args)
        {
            Task<object> Rejector(Request request) =>
                Task.FromResult<object>(new Response { Tag = "rejector" });
            Task<object> Approver(Request request) =>
                Task.FromResult<object>(new Response { Tag = "approver" });

            async Task<object> Local(Request request, RequestMeta meta)
            {
                if (meta.TryPrior(request, out var priorResult))
                {
                    return await priorResult;
                }

                return new Response { Tag = "local" };
            }

            var host1 = new Host()
                .Add<Request>("cmd:run", Approver)
                .WithTransport()
                .Listen(new HttpListenConfig(8260) { Pin = "cmd:*" });
            var host2 = new Host()
                .Add<Request>("cmd:run", Rejector)
                .WithTransport()
                .Listen(new HttpListenConfig(8270));

            await PrintAsync(new Host()
                .WithTransport()
                .Add<Request>("cmd:run", Local)
            );

            await PrintAsync(new Host()
                .WithTransport()
                .Client(new HttpClientConfig(new Uri("http://localhost:8260")) { Pin = "cmd:run" })
                .Client(new HttpClientConfig(new Uri("http://localhost:8270")) { Pin = "cmd:run" })
                .Add<Request>("cmd:run", Local)
            );

            await PrintAsync(new Host()
                .WithTransport()
                .Client(new HttpClientConfig(new Uri("http://localhost:8270")) { Pin = "cmd:run" })
                .Client(new HttpClientConfig(new Uri("http://localhost:8260")) { Pin = "cmd:run" })
                .Add<Request>("cmd:run", Local)
            );

            Console.ReadLine();
        }

        private static async Task PrintAsync(Host host)
        {
            var response = await host.Act<Response>(new Request { Cmd = "run" });

            Console.WriteLine($"Tag = {response.Tag}");
        }

        public class Request : IRequest
        {
            public string Cmd { get; set; }
        }

        public class Response
        {
            public string Tag { get; set; }
        }
    }
}
