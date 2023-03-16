using Akka.Actor;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Ottd.Messages
{
    public record QueryDebugInfo(Guid ServerId, ulong GuildId, ulong ChannelId) : ExecuteServerAction(ServerId, GuildId)
    {
        public override Props CreateCommandActorProps(IServiceProvider serviceProvider, OttdServer server, AdminPortClient client)
        {
            throw new NotImplementedException();
        }
    }
}
