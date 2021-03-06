﻿using OpenttdDiscord.Database.Extensions;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public class AdminChannel
    {
        public Server Server { get;  }

        public ulong ChannelId { get;  }

        public string Prefix { get; }

        public AdminChannel(Server server, ulong channelId, string prefix)
        {
            this.Server = server;
            this.ChannelId = channelId;
            this.Prefix = prefix;
        }

        public AdminChannel(DbDataReader reader, string prefix = null)
        {
            this.Server = new Server(reader);
            this.ChannelId = reader.ReadU64("channel_id");
            this.Prefix = reader.ReadString("prefix");

        }

        public AdminChannelUniqueValue UniqueValue => new AdminChannelUniqueValue(Server.GuildId, Server.Id, ChannelId);
    }
}
