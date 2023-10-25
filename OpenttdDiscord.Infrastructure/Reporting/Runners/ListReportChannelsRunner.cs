﻿using System.Text;
using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Reporting.Runners
{
    internal class ListReportChannelsRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        private readonly IListReportChannelsUseCase listReportChannelsUseCase;

        public ListReportChannelsRunner(
            IGetServerUseCase getServerUseCase,
            IListReportChannelsUseCase listReportChannelsUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(akkaService, getRoleLevelUseCase)
        {
            this.getServerUseCase = getServerUseCase;
            this.listReportChannelsUseCase = listReportChannelsUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;

            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from reportServers in listReportChannelsUseCase.Execute(
                    user,
                    guildId)
                from response in GenerateResponse(reportServers)
                select (IInteractionResponse) new TextResponse(response);
        }

        private EitherAsync<IError, string> GenerateResponse(List<ReportChannel> channels) => TryAsync(
                async () =>
                {
                    StringBuilder sb = new();

                    foreach (var reportChannel in channels)
                    {
                        var server = (await getServerUseCase.Execute(
                                User.Master,
                                reportChannel.ServerId))
                            .ThrowIfError()
                            .Right();
                        sb.AppendLine($"{server.Name} - {MentionUtils.MentionChannel(reportChannel.ChannelId)}");
                    }

                    return sb.ToString();
                })
            .ToEitherAsyncError();
    }
}