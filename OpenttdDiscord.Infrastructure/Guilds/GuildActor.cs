﻿using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Guilds.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Actors;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Servers;
using System.Linq;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    public class GuildActor : ReceiveActorBase
    {
        private readonly IListOttdServersUseCase listOttdServersUseCase;

        private readonly ulong guildId;

        private readonly Dictionary<Guid, IActorRef> serverActors = new();

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
            Receive<ExecuteServerAction>(ExecuteServerAction);
            Receive<InformAboutServerRegistration>(InformAboutServerRegistration);
            ReceiveAsync<InformAboutServerDeletion>(InformAboutServerDeletion);
        }

        public static Props Create(IServiceProvider sp, ulong guildId)
            => Props.Create(() => new GuildActor(sp, guildId));

        private async Task InitGuildsActorMessage(InitGuildsActorMessage _)
        {
            (await listOttdServersUseCase.Execute(User.Master, guildId))
                .ThrowIfError()
                .Map(servers => servers.Select(CreateServerActor).ToList());
        }

        private EitherUnit CreateServerActor(OttdServer s)
        {
            if(serverActors.ContainsKey(s.Id))
            {
                return new HumanReadableError("Server is already registered");
            }

            Props props = GuildServerActor.Create(SP, s);
            IActorRef actor = Context.ActorOf(props);
            serverActors.Add(s.Id, actor);
            return Unit.Default;
        }

        private void InformAboutServerRegistration(InformAboutServerRegistration msg)
        {
            CreateServerActor(msg.server);
        }

        private void ExecuteServerAction(ExecuteServerAction msg)
        {
            if(!serverActors.TryGetValue(msg.ServerId, out var server))
            {
                return;
            }

            server.Tell(msg);
        }

        private async Task InformAboutServerDeletion(InformAboutServerDeletion msg)
        {
            if (!serverActors.TryGetValue(msg.server.Id, out var server))
            {
                return;
            }

            await server.GracefulStop(TimeSpan.FromSeconds(1));
            serverActors.Remove(msg.server.Id);
        }
    }
}
