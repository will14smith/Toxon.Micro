using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Http;
using Toxon.Micro.Transport;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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
                .Configure(app =>
                {
                    app.Get("/list", context => HandleList(host, context));
                    app.Post("/new", context => HandleNewAsync(host, context));
                })
                .Build().Start();

            Console.WriteLine("Running Front... press enter to exit!");
            Console.ReadLine();
        }

        private static async Task HandleList(Host host, HttpContext context)
        {
            var entries = await host.Act<List<EntryResponse>>(new ListEntriesFromStore());

            await context.Response.WriteAsync(JsonConvert.SerializeObject(entries));
        }

        private static async Task HandleNewAsync(Host host, HttpContext context)
        {
            var input = JsonSerializer.Create().Deserialize<EntryInput>(new JsonTextReader(new StreamReader(context.Request.Body)));

            var entry = await host.Act<EntryCreateResponse>(new SaveEntryToStore
            {
                User = input.User,
                Text = input.Text,
            });

            await context.Response.WriteAsync(JsonConvert.SerializeObject(entry));
        }
    }

    internal class EntryInput
    {
        public string User { get; set; }
        public string Text { get; set; }
    }
    internal class EntryCreateResponse
    {
        public int Id { get; set; }
    }

    internal class ListEntriesFromStore : IRequest
    {
        public string Store => "list";
        public string Kind => "entry";

        public string User { get; set; }
    }
    internal class SaveEntryToStore : IRequest
    {
        public string Post => "entry";

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
