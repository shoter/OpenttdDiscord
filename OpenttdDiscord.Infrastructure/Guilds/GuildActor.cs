using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    public class GuildActor : ReceiveActorBase
    {
        public GuildActor(IServiceProvider serviceProvider, ulong guildId) : base(serviceProvider)
        {
        }

        public static Props Create(IServiceProvider sp, ulong guildId)
            => Props.Create(() => new GuildActor(sp, guildId));
    }
}
