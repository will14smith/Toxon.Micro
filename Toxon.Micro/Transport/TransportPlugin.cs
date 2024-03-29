﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Toxon.Micro.Transport
{
    public class TransportPlugin
    {
        public void Init(Host host)
        {
            host.Add<TransportListenCommand>("role:transport,cmd:listen", message => HandleListen(host, message));
            host.Add<TransportClientCommand>("role:transport,cmd:client", message => HandleClient(host, message));
        }

        private Task<object> HandleListen(Host host, TransportListenCommand message)
        {
            if (message.Config.Type != "http" || !(message.Config is HttpListenConfig httpConfig))
            {
                throw new NotImplementedException("only http is supported for now");
            }

            new WebHostBuilder()
                .UseKestrel(k => k.ListenLocalhost(httpConfig.Port))
                .Configure(app =>
                {
                    app.Run(async context =>
                    {
                        var body = context.Request.Body;
                        var bodyString = await new StreamReader(body).ReadToEndAsync();

                        Console.WriteLine($"Received message: {bodyString}");

                        var req = host.Deserialize<IReadOnlyDictionary<string, object>>(bodyString);

                        object resp = null;
                        try
                        {
                            resp = await host.Act(new DictionaryRequest(req));
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error handling request: ${ex.Message}");
                            Console.ResetColor();
                        }

                        context.Response.Headers.Add("Content-Type", "application/json");
                        var responseString = host.Serialize(resp);
                        await context.Response.WriteAsync(responseString);

                        Console.WriteLine($"Response message: {responseString}");
                    });
                })
                .Start();

            return Task.FromResult<object>(null);
        }

        private Task<object> HandleClient(Host host, TransportClientCommand message)
        {
            if (message.Config.Type != "http" || !(message.Config is HttpClientConfig httpConfig))
            {
                throw new NotImplementedException("only http is supported for now");
            }

            var client = new HttpClient { BaseAddress = httpConfig.BaseUri };

            host.Add(message.Config.Pin, async request =>
            {
                var serializedRequest = host.Serialize(request);

                Console.WriteLine($"Sending message: {serializedRequest}");

                var content = Encoding.UTF8.GetBytes(serializedRequest);

                var httpResponse = await client.PostAsync("/", new ByteArrayContent(content));
                var serializedResponse = await httpResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"Received response message: {serializedResponse}");

                var response = host.Deserialize<object>(serializedResponse);

                return response;
            }, message.Config.Mode);

            return Task.FromResult<object>(null);
        }
    }

    public static class TransportExtensions
    {
        public static Host WithTransport(this Host host)
        {
            // TODO mark as inited

            var plugin = new TransportPlugin();
            plugin.Init(host);

            return host;
        }

        public static Host Listen(this Host host, TransportListenConfig config)
        {
            // TODO await
            host.Act(new TransportListenCommand { Config = config });

            return host;
        }
        public static Host Client(this Host host, TransportClientConfig config)
        {
            // TODO await
            host.Act(new TransportClientCommand { Config = config });

            return host;
        }
    }

    public abstract class TransportCommand : IRequest
    {
        public string Role => "transport";
        public abstract string Cmd { get; }
    }
    public class TransportListenCommand : TransportCommand
    {
        public override string Cmd => "listen";
        public TransportListenConfig Config { get; set; }
    }
    public class TransportClientCommand : TransportCommand
    {
        public override string Cmd => "client";
        public TransportClientConfig Config { get; set; }
    }

    public abstract class TransportListenConfig
    {
        public abstract string Type { get; }
        public string Pin { get; set; }
    }

    public abstract class TransportClientConfig
    {
        public abstract string Type { get; }
        public string Pin { get; set; }
        public RouteMode Mode { get; set; } = RouteMode.Consume;
    }
}
