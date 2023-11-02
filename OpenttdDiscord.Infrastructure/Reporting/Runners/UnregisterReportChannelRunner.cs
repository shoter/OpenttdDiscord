using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Reporting.Runners
{
    internal class UnregisterReportChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerByNameUseCase;
        private readonly IUnregisterReportChannelUseCase unregisterReportChannelUseCase;

        public UnregisterReportChannelRunner(
            IGetServerUseCase getServerByNameUseCase,
            IUnregisterReportChannelUseCase unregisterReportChannelUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.getServerByNameUseCase = getServerByNameUseCase;
            this.unregisterReportChannelUseCase = unregisterReportChannelUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            return
                from _0 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from server in getServerByNameUseCase.Execute(
                    serverName,
                    guildId)
                from _1 in unregisterReportChannelUseCase.Execute(
                    user,
                    server.Id,
                    guildId,
                    channelId)
                select (IInteractionResponse) new TextResponse($"Unregistered RCON channel for {serverName}");
        }
    }
}