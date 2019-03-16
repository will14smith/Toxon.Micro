using System;
using System.Threading.Tasks;
using Toxon.Micro.Transport;

namespace Toxon.Micro.Blog.Post
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new Host();

            host.WithTransport()
                .Listen(new HttpListenConfig(8602) { Pin = "post:*" })
                .Client(new HttpClientConfig(new Uri("http://localhost:8601")) { Pin = "store:*,kind:entry" })

                .Add<PostEntryRequest>("post:entry", message => HandlePostEntryAsync(host, message))
            ;

            Console.WriteLine("Running Post... press enter to exit!");
            Console.ReadLine();
        }

        private static async Task<object> HandlePostEntryAsync(Host host, PostEntryRequest message)
        {
            var saveResponse = await host.Act<SaveEntryResponse>(new SaveEntryRequest
            {
                User = message.User,
                Text = message.Text,
            });

            await host.Broadcast(new InfoEntryBroadcast
            {
                Id = saveResponse.Id,

                User = message.User,
                Text = message.Text,
            });

            return new PostEntryResponse
            {
                Id = saveResponse.Id,

                User = message.User,
                Text = message.Text,
            };
        }
    }

    internal class InfoEntryBroadcast : IRequest
    {
        public string Info => "entry";

        public int Id { get; set; }

        public string User { get; set; }
        public string Text { get; set; }
    }

    internal class PostEntryRequest : IRequest
    {
        public string Post => "entry";

        public string User { get; set; }
        public string Text { get; set; }
    }

    internal class PostEntryResponse
    {
        public int Id { get; set; }

        public string User { get; set; }
        public string Text { get; set; }
    }

    internal class SaveEntryRequest : IRequest
    {
        public string Store => "save";
        public string Kind => "entry";

        public string User { get; set; }
        public string Text { get; set; }
    }

    internal class SaveEntryResponse
    {
        public int Id { get; set; }
    }
}
