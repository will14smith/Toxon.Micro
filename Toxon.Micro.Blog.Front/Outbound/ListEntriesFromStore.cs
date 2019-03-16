namespace Toxon.Micro.Blog.Front.Outbound
{
    internal class ListEntriesFromStore : IRequest
    {
        public string Store => "list";
        public string Kind => "entry";

        public string User { get; set; }
    }
}
