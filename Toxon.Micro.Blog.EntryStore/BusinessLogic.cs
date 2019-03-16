using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxon.Micro.Blog.EntryStore.Inbound;

namespace Toxon.Micro.Blog.EntryStore
{
    public class BusinessLogic : HostPlugin
    {
        private readonly List<EntryResponse> _entries = new List<EntryResponse>();

        public override void Init(Host host)
        {
            host.Add<StoreRequest>("store:*,kind:entry", HandleStoreAsync);
        }

        private async Task<object> HandleStoreAsync(StoreRequest message)
        {
            switch (message.Store)
            {
                case "list":
                    IEnumerable<EntryResponse> entries = _entries;
                    if (message.User != null)
                    {
                        entries = entries.Where(x => x.User == message.User);
                    }

                    return entries;

                case "save":
                    var entry = new EntryResponse
                    {
                        Id = _entries.Count + 1,

                        User = message.User,
                        Text = message.Text,
                    };
                    _entries.Add(entry);

                    return entry;

                default: throw new NotImplementedException();
            }
        }
    }
}
