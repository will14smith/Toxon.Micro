namespace Toxon.Micro.Blog.Front.Outbound
{
    public class SearchResult
    {
        public decimal Score { get; set; }
        public SearchDocument Document { get; set; }
    }
}