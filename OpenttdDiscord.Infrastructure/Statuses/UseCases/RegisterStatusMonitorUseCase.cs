﻿using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Statuses.Messages;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class RegisterStatusMonitorUseCase : IRegisterStatusMonitorUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        private readonly DiscordSocketClient discord;

        private readonly IAkkaService akkaService;

        public RegisterStatusMonitorUseCase(
            IStatusMonitorRepository statusMonitorRepository,
            IAkkaService akkaService,
            DiscordSocketClient discord)
        {
            this.statusMonitorRepository = statusMonitorRepository;
            this.akkaService = akkaService;
            this.discord = discord;
        }

        public EitherAsync<IError, StatusMonitor> Execute(OttdServer server, ulong guildId, ulong channelId)
        {
            var embedMessageIdResult = CreateEmbedMessage(channelId, server);

            return
            (from messageId in embedMessageIdResult
             from statusMonitor in statusMonitorRepository.Insert(new StatusMonitor(
                 server.Id,
                 guildId,
                 channelId,
                 messageId,
                 DateTime.MinValue.ToUniversalTime()))
             from _1 in InformActor(statusMonitor)
             select statusMonitor)
            .MapLeft(err =>
            {
                if (embedMessageIdResult.IsRight.Result)
                {
                    DeleteEmbedMessage(channelId, embedMessageIdResult.Right());
                }

                return err;
            });
        }

        private EitherAsync<IError, ulong> CreateEmbedMessage(ulong channelId, OttdServer server)
            => TryAsync<Either<IError, ulong>>(async () =>
            {
                IChannel channel = await discord.GetChannelAsync(channelId);

                if (!(channel is IMessageChannel msgChannel))
                {
                    return new HumanReadableError("Wrong channel!");
                }

                IUserMessage msg = await msgChannel.SendMessageAsync(embed: CreateEmptyEmbed(server));

                return msg.Id;
            }).ToEitherAsyncErrorFlat();

        private EitherAsyncUnit DeleteEmbedMessage(ulong channelId, ulong messageId)
           => TryAsync<EitherUnit>(async () =>
           {
               IChannel channel = await discord.GetChannelAsync(channelId);

               if (!(channel is IMessageChannel msgChannel))
               {
                   return new HumanReadableError("Wrong channel!");
               }

               await msgChannel.DeleteMessageAsync(messageId);
               return Unit.Default;
           }).ToEitherAsyncErrorFlat();

        private Embed CreateEmptyEmbed(OttdServer server)
        {
            return new EmbedBuilder()
                .WithTitle($"{server.Name} Status")
                .AddField("Please Wait", "Status message creation in progress")
                .Build();
        }

        private EitherAsyncUnit InformActor(StatusMonitor statusMonitor)
            => TryAsync(async () =>
            {
                var msg = new RegisterStatusMonitor(statusMonitor);
                (await akkaService.SelectActor(MainActors.Paths.Guilds)).Tell(msg);
                return Unit.Default;
            }).ToEitherAsyncError();
    }
}
