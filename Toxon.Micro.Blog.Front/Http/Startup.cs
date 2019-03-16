using System;
using System.Collections.Generic;
using System.IO;
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
            app.Post("/new", context => HandleNewAsync(host, context));
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
}
