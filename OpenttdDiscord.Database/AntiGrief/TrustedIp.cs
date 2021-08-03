using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenttdDiscord.Database.Extensions;

namespace OpenttdDiscord.Database.AntiGrief
{
    public class TrustedIp
    {
        public string IpAddress { get; }

        public TimeSpan PlayingTime { get; }


        public TrustedIp(string ipAddress, TimeSpan playingTime)
        {
            this.IpAddress = ipAddress;
            this.PlayingTime = playingTime;
        }

        public TrustedIp(DbDataReader reader, string prefix = null)
        {
            this.IpAddress = reader.ReadString("ip_address", prefix);
            this.PlayingTime = TimeSpan.FromMinutes(reader.ReadInt("playing_time_minutes", prefix));

        }

    }
}
