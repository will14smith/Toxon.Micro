using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toxon.Micro.Routing;

namespace Toxon.Micro.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new Host();

            var salestax = new FieldMatcher("cmd", new EqualityValueMatcher("salestax"));
            IRequestMatcher Country(string c) => new FieldMatcher("country", new EqualityValueMatcher(c));

            host.Add<TaxRequest>(salestax, request =>
            {
                var rate = 0.23m;
                var total = request.Net * (1 + rate);

                var response = new TaxResponse(total);
                return Task.FromResult<IResponse>(response);
            });

            host.Add<USTaxRequest>(new AndMatcher(salestax, Country("US")), request =>
            {
                var state = new Dictionary<string, decimal>
                {
                    {"NY", 0.04m},
                    {"CA", 0.0625m},
                };

                var rate = state[request.State];
                var total = request.Net * (1 + rate);

                var response = new TaxResponse(total);
                return Task.FromResult<IResponse>(response);
            });

            host.Add<IETaxRequest>(new AndMatcher(salestax, Country("IE")), request =>
            {
                var category = new Dictionary<string, decimal>
                {
                    {"top", 0.23m},
                    {"reduced", 0.135m},
                };

                var rate = category[request.Category];
                var total = request.Net * (1 + rate);

                var response = new TaxResponse(total);
                return Task.FromResult<IResponse>(response);
            });

            var deResponse = await host.Act<TaxResponse>(new TaxRequest { Country = "DE", Net = 100 });
            Console.WriteLine($"DE: {deResponse.Total}");

            var usResponse = await host.Act<TaxResponse>(new USTaxRequest { State = "NY", Net = 100 });
            Console.WriteLine($"US: {usResponse.Total}");

            var ieResponse = await host.Act<TaxResponse>(new IETaxRequest { Category = "reduced", Net = 100 });
            Console.WriteLine($"IE: {ieResponse.Total}");

            Console.ReadLine();
        }
    }

    public class TaxRequest : IRequest
    {
        public string Cmd => "salestax";

        public string Country { get; set; }

        public decimal Net { get; set; }
    }
    public class USTaxRequest : IRequest
    {
        public string Cmd => "salestax";

        public string Country => "US";
        public string State { get; set; }

        public decimal Net { get; set; }
    }
    public class IETaxRequest : IRequest
    {
        public string Cmd => "salestax";

        public string Country => "IE";
        public string Category { get; set; }

        public decimal Net { get; set; }
    }

    public class TaxResponse : IResponse
    {
        public decimal Total { get; }

        public TaxResponse(decimal total)
        {
            Total = total;
        }
    }
}
