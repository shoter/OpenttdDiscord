using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public class AdminChannel
    {
        public Server Server { get;  }

        public ulong ChannelId { get;  }

        public AdminChannel(Server server, ulong channelId)
        {
            this.Server = server;
            this.ChannelId = channelId;
        }
    }
}
