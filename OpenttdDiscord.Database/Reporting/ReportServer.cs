using OpenttdDiscord.Database.Extensions;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Reporting
{
    public class ReportServer
    {
        public Server Server { get; }

        public ulong ChannelId { get; }

        public ReportServer(Server server, ulong channelId)
        {
            this.Server = server;
            this.ChannelId = channelId;
        }

        public ReportServer(DbDataReader reader, string prefix = null)
        {
            this.Server = new Server(reader);
            this.ChannelId = reader.ReadU64("channel_id", prefix);
        }

    }
}
