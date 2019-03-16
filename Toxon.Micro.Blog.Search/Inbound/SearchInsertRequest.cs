namespace Toxon.Micro.Blog.Search.Inbound
{
    public class SearchInsertRequest : Document, IRequest
    {
        public string Search => "insert";
    }
}
