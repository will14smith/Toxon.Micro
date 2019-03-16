using System;

namespace Toxon.Micro.Transport
{
    public class HttpListenConfig : TransportListenConfig
    {
        public override string Type => "http";

        public ushort Port { get; }

        public HttpListenConfig(ushort port)
        {
            Port = port;
        }
    }

    public class HttpClientConfig : TransportClientConfig
    {
        public override string Type => "http";

        public Uri BaseUri { get; }

        public HttpClientConfig(Uri baseUri)
        {
            BaseUri = baseUri;
        }
    }
}
