namespace Toxon.Micro.Blog.Post.Outbound
{
    internal class InfoEntryBroadcast : IRequest
    {
        public string Info => "entry";

        public int Id { get; set; }

        public string User { get; set; }
        public string Text { get; set; }
    }
}
