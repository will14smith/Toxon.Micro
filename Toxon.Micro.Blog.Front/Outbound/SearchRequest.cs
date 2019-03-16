namespace Toxon.Micro.Blog.Front.Outbound
{
    public class SearchRequest : IRequest
    {
        public string Search => "query";
        public string Kind { get; set; }
        public string Query { get; set; }
    }
}
