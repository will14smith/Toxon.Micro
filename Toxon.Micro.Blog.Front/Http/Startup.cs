using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Toxon.Micro.Blog.Front.Outbound;

namespace Toxon.Micro.Blog.Front.Http
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, Host host)
        {
            app.Get("/list", context => HandleList(host, context));
            app.Get("/search", context => HandleSearch(host, context));
            app.Post("/new", context => HandleNewAsync(host, context));
        }

        private static async Task HandleList(Host host, HttpContext context)
        {
            var entries = await host.Act<List<EntryResponse>>(new ListEntriesFromStore());
            var response = entries.Select(entry => new Entry
            {
                Id = entry.Id,
                User = entry.User,
                Text = entry.Text,
            });

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private static async Task HandleSearch(Host host, HttpContext context)
        {
            var query = context.Request.Query["q"];
            var searchResponse = await host.Act<SearchResponse>(new SearchRequest { Kind = "entry", Query = query });
            var response = searchResponse.Results.Select(result => new SearchResult
            {
                Score = result.Score,
                Document = new Entry
                {
                    Id = result.Document.Id,
                    User = result.Document.Fields["user"],
                    Text = result.Document.Fields["text"],
                }
            });

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
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
}
