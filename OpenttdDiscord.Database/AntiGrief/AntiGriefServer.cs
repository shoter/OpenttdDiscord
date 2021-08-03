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
        public ulong serverId { get; }


        public int RequiredMinsToPlay { get; }

        public string Reason { get;  }

        public AntiGriefServer(ulong serverId,  int requiredMinsToPlay, string reason)
        {
            this.serverId = serverId;
            this.RequiredMinsToPlay = requiredMinsToPlay;
            this.Reason = reason;
        }

        public AntiGriefServer(DbDataReader reader, string prefix = null)
        {
            this.serverId = reader.ReadU64("server_id", prefix);
            this.RequiredMinsToPlay = reader.ReadInt("required_mins_to_play", prefix);
            this.Reason = reader.ReadString("reason", prefix);
        }
    }
}
