using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Servers
{
    public class SubscribedServer
    {
        public Server Server { get; }
        public DateTimeOffset LastUpdate { get; }
        public ulong ChannelId { get; }

        public SubscribedServer(Server server, DateTimeOffset lastUpdate, ulong channelId)
        {
            this.Server = server;
            this.LastUpdate = lastUpdate;
            this.ChannelId = channelId;
        }
    }
}
