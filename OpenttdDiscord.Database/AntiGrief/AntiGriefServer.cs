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

        public ulong GuildId { get; }

        public AntiGriefServer(Server server, ulong guildId)
        {
            this.Server = server;
            this.GuildId = guildId;
        }

        public AntiGriefServer(DbDataReader reader, string prefix = null)
        {
            this.Server = new Server(reader);
            this.GuildId = reader.ReadU64("guild_id", prefix);
        }
    }
}
