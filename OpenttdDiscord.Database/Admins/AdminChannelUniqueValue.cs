using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public class AdminChannelUniqueValue : IEquatable<AdminChannelUniqueValue>
        {
       public ulong GuildId { get; }
        
        public ulong ServerId { get; }

        public ulong ChannelId { get; }

        public AdminChannelUniqueValue(ulong guildId, ulong serverId, ulong channelId)
        {
            this.GuildId = guildId;
            this.ServerId = serverId;
            this.ChannelId = channelId;
        }

        public bool Equals(AdminChannelUniqueValue o) => GuildId == o.GuildId && ServerId == o.ServerId && ChannelId == o.ChannelId;

        public override int GetHashCode()
        {
            return (GuildId + (ServerId * ChannelId)).GetHashCode();
        }
    }
}
