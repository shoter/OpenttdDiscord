using Akka.Actor;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Ottd.Messages
{
    public abstract record ExecuteServerAction(
        Guid ServerId,
        ulong GuildId) : IOttdServerMessage;

    {
    public virtual TimeSpan TimeOut { get; } = TimeSpan.FromSeconds(30);

    public abstract Props CreateCommandActorProps(
        IServiceProvider serviceProvider,
        OttdServer server,
        AdminPortClient client);
    }
}