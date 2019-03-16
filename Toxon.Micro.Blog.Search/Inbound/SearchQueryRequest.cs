namespace Toxon.Micro.Blog.Search.Inbound
{
    public class SearchQueryRequest : IRequest
    {
        public string Search => "query";

        public string Kind { get; set; }
        public string Query { get; set; }
    }
}
