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


        public int RequiredMinsToPlay { get; }

        public string Reason { get;  }

        public AntiGriefServer(Server server,  int requiredMinsToPlay, string reason)
        {
            this.Server = server;
            this.RequiredMinsToPlay = requiredMinsToPlay;
            this.Reason = reason;
        }

        public AntiGriefServer(DbDataReader reader, string prefix = null)
        {
            this.Server = new Server(reader);
            this.RequiredMinsToPlay = reader.ReadInt("required_mins_to_play", prefix);
            this.Reason = reader.ReadString("reason", prefix);
        }
    }
}
