using OpenttdDiscord.Domain.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Statuses.Messages
{
    internal record RegisterStatusMonitor(OttdServer Server, ulong GuildId, ulong ChannelId);
}
