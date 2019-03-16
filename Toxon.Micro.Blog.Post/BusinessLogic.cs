using System.Threading.Tasks;
using Toxon.Micro.Blog.Post.Inbound;
using Toxon.Micro.Blog.Post.Outbound;

namespace Toxon.Micro.Blog.Post
{
    public class BusinessLogic : HostPlugin
    {
        public override void Init(Host host)
        {
            host.Add<PostEntryRequest>("post:entry", message => HandlePostEntryAsync(host, message));
        }

        private static async Task<object> HandlePostEntryAsync(Host host, PostEntryRequest message)
        {
            var saveResponse = await host.Act<SaveEntryResponse>(new SaveEntryRequest
            {
                User = message.User,
                Text = message.Text,
            });

            await host.Broadcast(new InfoEntryBroadcast
            {
                Id = saveResponse.Id,

                User = message.User,
                Text = message.Text,
            });

            return new PostEntryResponse
            {
                Id = saveResponse.Id,

                User = message.User,
                Text = message.Text,
            };
        }
    }
}
