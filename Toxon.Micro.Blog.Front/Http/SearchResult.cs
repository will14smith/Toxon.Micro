namespace Toxon.Micro.Blog.Front.Http
{
    internal class SearchResult
    {
        public decimal Score { get; set; }
        public Entry Document { get; set; }
    }
}
