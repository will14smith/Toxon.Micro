using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toxon.Micro.Transport;

namespace Toxon.Micro.Blog.EntryStore
{
    class Program
    {
        private static readonly List<EntryResponse> Entries = new List<EntryResponse>();

        static void Main(string[] args)
        {
            var host = new Host();

            host.WithTransport()
                .Listen(new HttpListenConfig(8601) { Pin = "store:*,kind:entry" })

                .Add<StoreRequest>("store:*,kind:entry", message => HandleStoreAsync(host, message))
                ;

            Console.WriteLine("Running EntryStore... press enter to exit!");
            Console.ReadLine();
        }

        private async static Task<object> HandleStoreAsync(Host host, StoreRequest message)
        {
            switch (message.Store)
            {
                case "list":
                    IEnumerable<EntryResponse> entries = Entries;
                    if (message.User != null)
                    {
                        entries = entries.Where(x => x.User == message.User);
                    }

                    return entries;

                case "save":
                    var entry = new EntryResponse
                    {
                        Id = Entries.Count + 1,

                        User = message.User,
                        Text = message.Text,
                    };
                    Entries.Add(entry);

                    return entry;

                default: throw new NotImplementedException();
            }
        }
    }

    internal class StoreRequest : IRequest
    {
        public string Store { get; set; }
        public string Kind => "entry";

        public string User { get; set; }
        public string Text { get; set; }
    }

    internal class EntryResponse
    {
        public int Id { get; set; }

        public string User { get; set; }
        public string Text { get; set; }
    }
}
