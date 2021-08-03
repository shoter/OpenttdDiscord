using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenttdDiscord.Database.Extensions;
using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Database.AntiGrief
{
    public class AntiGriefServer
    {
        public Server Server { get; }

        public ulong ChannelId { get; }

        public AntiGriefServer(Server server, ulong channelId)
        {
            this.Server = server;
            this.ChannelId = channelId;
        }

        public AntiGriefServer(DbDataReader reader, string prefix = null)
        {
            this.Server = new Server(reader);
            this.ChannelId = reader.ReadU64("channel_id", prefix);
        }
    }
}
