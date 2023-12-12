﻿using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Guilds.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Actors;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Roles.Actors;
using OpenttdDiscord.Infrastructure.Roles.Messages;
using OpenttdDiscord.Infrastructure.Servers.Messages;

namespace OpenttdDiscord.Infrastructure.Guilds.Actors
{
    public class GuildActor : ReceiveActorBase
    {
        private readonly ulong guildId;

        private readonly IActorRef guildRoleActor;
        private readonly IOttdServerRepository ottdServerRepository;

        private readonly Dictionary<Guid, IActorRef> serverActors = new();

        public GuildActor(
            IServiceProvider serviceProvider,
            ulong guildId)
            : base(serviceProvider)
        {
            ottdServerRepository = SP.GetRequiredService<IOttdServerRepository>();
            this.guildId = guildId;
            guildRoleActor = Context.ActorOf(
                GuildRoleActor
                    .Create(serviceProvider, guildId),
                "GuildRoleActor");

            Ready();
            Self.Tell(new InitGuildsActorMessage());
        }

        private void Ready()
        {
            ReceiveAsync<InitGuildsActorMessage>(InitGuildsActorMessage);
            Receive<InformAboutServerRegistration>(InformAboutServerRegistration);
            ReceiveAsync<InformAboutServerDeletion>(InformAboutServerDeletion);

            ReceiveForward<GetRoleLevel>(() => guildRoleActor);
            ReceiveForward<DeleteRole>(() => guildRoleActor);
            ReceiveForward<UpsertRole>(() => guildRoleActor);

            ReceiveRedirectToServer<IOttdServerMessage>(msg => msg.ServerId);
        }

        public static Props Create(
            IServiceProvider sp,
            ulong guildId)
        {
            return Props.Create(
                () => new GuildActor(
                    sp,
                    guildId));
        }

        private async Task InitGuildsActorMessage(InitGuildsActorMessage _)
        {
            (await ottdServerRepository.GetServersForGuild(guildId))
                .ThrowIfError()
                .Map(
                    servers => servers.Select(CreateServerActor)
                        .ToList());
        }

        private EitherUnit CreateServerActor(OttdServer s)
        {
            if (serverActors.ContainsKey(s.Id))
            {
                return new HumanReadableError("Server is already registered");
            }

            Props props = GuildServerActor.Create(
                SP,
                s);
            IActorRef actor = Context.ActorOf(
                props,
                $"server-{s.Id}");
            serverActors.Add(
                s.Id,
                actor);
            return Unit.Default;
        }

        private void InformAboutServerRegistration(InformAboutServerRegistration msg)
        {
            CreateServerActor(msg.server);
        }

        private async Task InformAboutServerDeletion(InformAboutServerDeletion msg)
        {
            if (!serverActors.TryGetValue(
                    msg.server.Id,
                    out IActorRef? server))
            {
                return;
            }

            await server.GracefulStop(TimeSpan.FromSeconds(1));
            serverActors.Remove(msg.server.Id);
        }

        private void ReceiveRedirectToServer<TMsg>(Func<TMsg, Guid> serverSelector)
        {
            Receive(
                (TMsg msg) =>
                {
                    if (!serverActors.TryGetValue(
                            serverSelector(msg),
                            out IActorRef? actor))
                    {
                        return;
                    }

                    actor.Forward(msg);
                });
        }
    }
}