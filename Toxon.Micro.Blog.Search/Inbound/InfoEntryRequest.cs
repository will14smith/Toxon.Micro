using System;
using System.Collections.Generic;
using System.Text;

namespace Toxon.Micro.Blog.Search.Inbound
{
    public class InfoEntryRequest : IRequest
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
    }
}
