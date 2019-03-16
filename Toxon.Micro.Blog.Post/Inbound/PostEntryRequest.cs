namespace Toxon.Micro.Blog.Post.Inbound
{
    internal class PostEntryRequest : IRequest
    {
        public string Post => "entry";

        public string User { get; set; }
        public string Text { get; set; }
    }
}
