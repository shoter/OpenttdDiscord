using Akka.Actor;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    internal class GuildServerActor : ReceiveActorBase
    {
        private readonly OttdServer server;

        public GuildServerActor(IServiceProvider serviceProvider, OttdServer server) : base(serviceProvider)
        {
            this.server = server;
        }

        public static Props Create(IServiceProvider sp, OttdServer server) 
            => Props.Create(() => new GuildServerActor(sp, server));
    }
}
