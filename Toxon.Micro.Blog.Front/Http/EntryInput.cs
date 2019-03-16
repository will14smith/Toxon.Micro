namespace Toxon.Micro.Blog.Front.Http
{
    internal class EntryInput
    {
        public string User { get; set; }
        public string Text { get; set; }
    }

    internal class Entry
    {
        public int Id { get; set; }

        public string User { get; set; }
        public string Text { get; set; }
    }
}
