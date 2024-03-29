﻿namespace Toxon.Micro.Blog.Post.Outbound
{
    internal class SaveEntryRequest : IRequest
    {
        public string Store => "save";
        public string Kind => "entry";

        public string User { get; set; }
        public string Text { get; set; }
    }
}
