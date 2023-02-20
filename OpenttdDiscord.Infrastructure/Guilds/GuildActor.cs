using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Guilds.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Actors;
using OpenttdDiscord.Infrastructure.Servers;
using System.Linq;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    public class GuildActor : ReceiveActorBase
    {
        private readonly IListOttdServersUseCase listOttdServersUseCase;

        private readonly ulong guildId;

        public GuildActor(IServiceProvider serviceProvider, ulong guildId) : base(serviceProvider)
        {
            this.listOttdServersUseCase = SP.GetRequiredService<IListOttdServersUseCase>();
            this.guildId = guildId;
            Ready();
            Self.Tell(new InitGuildsActorMessage());
        }

        private void Ready()
        {
            ReceiveAsync<InitGuildsActorMessage>(InitGuildsActorMessage);
        }

        public static Props Create(IServiceProvider sp, ulong guildId)
            => Props.Create(() => new GuildActor(sp, guildId));

        private async Task InitGuildsActorMessage(InitGuildsActorMessage _)
        {
            (await listOttdServersUseCase.Execute(User.Master, guildId))
                .ThrowIfError()
                .Select(servers => servers.Select(s => GuildServerActor.Create(SP, s)))
                .Select(props => props.Select(p => Context.ActorOf(p)).ToList());
        }
    }
}
