using Akka.Actor;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Actions;

namespace OpenttdDiscord.Infrastructure.Ottd.Messages;

internal record QueryServer(Guid ServerId, ulong GuildId, ulong ChannelId) : ExecuteServerAction(ServerId, GuildId)
{
    public override Props CreateCommandActorProps(IServiceProvider serviceProvider, OttdServer ottdServer, AdminPortClient client)
    {
        return OttdQueryServerAction.Create(
            serviceProvider,
            ottdServer,
            client);
    }
}
