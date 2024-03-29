﻿using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Domain.Guilds.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.EventLogs.Messages;
using OpenttdDiscord.Infrastructure.Guilds.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Rcon.Messages;
using OpenttdDiscord.Infrastructure.Reporting.Messages;
using OpenttdDiscord.Infrastructure.Servers.Messages;
using OpenttdDiscord.Infrastructure.Statuses.Messages;

namespace OpenttdDiscord.Infrastructure.Guilds.Actors
{
    public class GuildsActor : ReceiveActorBase
    {
        private readonly IGetAllGuildsUseCase getAllGuildsUseCase;
        private readonly Dictionary<ulong, IActorRef> guildActors = new();
        private readonly DiscordSocketClient discordSocketClient;

        public GuildsActor(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.discordSocketClient = SP.GetRequiredService<DiscordSocketClient>();
            getAllGuildsUseCase = SP.GetRequiredService<IGetAllGuildsUseCase>();
            Ready();

            Self.Tell(new InitGuildActorMessage());
        }

        public static Props Create(IServiceProvider sp) =>
            Props.Create(() => new GuildsActor(sp));

        public void Ready()
        {
            ReceiveAsync<InitGuildActorMessage>(InitGuildActorMessage);
            Receive<AddNewGuildActorMessage>(AddNewGuildActorMessage);
            Receive<InformAboutServerRegistration>(InformAboutServerRegistration);

            ReceiveRedirectMsg<ExecuteServerAction>(msg => msg.GuildId);
            ReceiveRedirectMsg<RegisterStatusMonitor>(msg => msg.StatusMonitor.GuildId);
            ReceiveRedirectMsg<InformAboutServerDeletion>(msg => msg.server.GuildId);
            ReceiveRedirectMsg<RemoveStatusMonitor>(msg => msg.GuildId);
            ReceiveRedirectMsg<RegisterChatChannel>(msg => msg.chatChannel.GuildId);
            ReceiveRedirectMsg<UnregisterChatChannel>(msg => msg.GuildId);
            ReceiveRedirectMsg<RegisterNewRconChannel>(msg => msg.RconChannel.GuildId);
            ReceiveRedirectMsg<UnregisterRconChannel>(msg => msg.guildId);
            ReceiveRedirectMsg<RetrieveEventLog>(msg => msg.GuildId);
            ReceiveRedirectMsg<RegisterReportChannel>(msg => msg.ReportChannel.GuildId);
            ReceiveRedirectMsg<UnregisterReportChannel>(msg => msg.GuildId);
            ReceiveRedirectMsg<IGuildMessage>(msg => msg.GuildId);
        }

        private async Task InitGuildActorMessage(InitGuildActorMessage _)
        {
            while (discordSocketClient.ConnectionState != ConnectionState.Connected)
            {
                await Task.Delay(1);
            }

            foreach (var guild in discordSocketClient.Guilds)
            {
                var message = new AddNewGuildActorMessage(guild.Id);
                Self.Tell(message);
            }
        }

        private void AddNewGuildActorMessage(AddNewGuildActorMessage msg)
        {
            logger.LogInformation($"Creating GuildActor for {msg.GuildId}");
            IActorRef actor = Context.ActorOf(GuildActor.Create(SP, msg.GuildId), MainActors.Names.Guild(msg.GuildId));
            guildActors.Add(msg.GuildId, actor);
        }

        private void InformAboutServerRegistration(InformAboutServerRegistration msg)
        {
            if (guildActors.TryGetValue(msg.server.GuildId, out IActorRef? actor))
            {
                actor.Tell(msg);
                return;
            }

            actor = Context.ActorOf(GuildActor.Create(SP, msg.server.GuildId), MainActors.Names.Guild(msg.server.GuildId));
            guildActors.Add(msg.server.GuildId, actor);
        }

        private void ReceiveRedirectMsg<TMsg>(Func<TMsg, ulong> guildSelector)
            => Receive((TMsg msg) =>
            {
                if (!guildActors.TryGetValue(guildSelector(msg), out IActorRef? actor))
                {
                    actor = Context.ActorOf(
                        GuildActor.Create(
                            SP,
                            guildSelector(msg)));

                    guildActors.Add(guildSelector(msg), actor);
                }

                actor.Forward(msg);
            });
    }
}
