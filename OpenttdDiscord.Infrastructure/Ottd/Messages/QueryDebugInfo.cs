using Akka.Actor;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Actions;

namespace OpenttdDiscord.Infrastructure.Ottd.Messages
{
    public record QueryDebugInfo(Guid ServerId, ulong GuildId, ulong ChannelId) : ExecuteServerAction(ServerId, GuildId)
    {
        public override Props CreateCommandActorProps(IServiceProvider serviceProvider, OttdServer server, AdminPortClient client)
        {
            return QueryDebugInfoAction.Create(
                serviceProvider,
                server,
                client);
        }
    }
}
