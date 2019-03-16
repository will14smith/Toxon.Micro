namespace Toxon.Micro.Blog.Front.Outbound
{
    internal class SaveEntryToStore : IRequest
    {
        public string Post => "entry";

        public string User { get; set; }
        public string Text { get; set; }
    }
}
