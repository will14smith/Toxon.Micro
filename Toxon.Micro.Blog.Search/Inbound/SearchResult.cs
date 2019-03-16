namespace Toxon.Micro.Blog.Search.Inbound
{
    public class SearchResult
    {
        public decimal Score { get; set; }
        public Document Document { get; set; }
    }
}