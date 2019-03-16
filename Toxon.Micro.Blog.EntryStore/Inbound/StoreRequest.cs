namespace Toxon.Micro.Blog.EntryStore.Inbound
{
    internal class StoreRequest : IRequest
    {
        public string Store { get; set; }
        public string Kind => "entry";

        public string User { get; set; }
        public string Text { get; set; }
    }
}
