﻿using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Statuses.Messages;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class RegisterStatusMonitorUseCase : UseCaseBase, IRegisterStatusMonitorUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        private readonly IDiscordClient discord;

        private readonly IAkkaService akkaService;

        public RegisterStatusMonitorUseCase(
            IStatusMonitorRepository statusMonitorRepository,
            IDiscordClient discord,
            IAkkaService akkaService)
        {
            this.statusMonitorRepository = statusMonitorRepository;
            this.discord = discord;
            this.akkaService = akkaService;
        }

        public EitherAsync<IError, StatusMonitor> Execute(OttdServer server, ulong guildId, ulong channelId)
        {
            var transactionLog = new TransactionLog();
            return
            (
             from messageId in CreateEmbedMessage(channelId, server)
             from _2 in transactionLog.AddTransactionRollback(() => DeleteEmbedMessage(channelId, messageId)).ToAsync()
             from statusMonitor in statusMonitorRepository.Insert(new StatusMonitor(
                 server.Id,
                 guildId,
                 channelId,
                 messageId,
                 DateTime.MinValue.ToUniversalTime()))
             from _3 in akkaService.SelectAndAsk<object>(MainActors.Paths.Guilds, new RegisterStatusMonitor(statusMonitor), TimeSpan.FromSeconds(1))
             select statusMonitor)
             .LeftRollback(transactionLog);
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
    }
}
