using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public class SubscribedServer
    {
        public Server Server { get; }
        public DateTimeOffset LastUpdate { get; }
        public ulong ChannelId { get; }
        public ulong? MessageId { get; }
        public int Port { get; }

        public SubscribedServer(Server server, DateTimeOffset lastUpdate, ulong channelId, ulong? messageId, int port)
        {
            this.Server = server;
            this.LastUpdate = lastUpdate;
            this.ChannelId = channelId;
            this.MessageId = messageId;
            this.Port = port;
        }
    }
}
