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
    internal class RegisterReportChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        private readonly IRegisterReportChannelUseCase registerReportChannelUseCase;

        public RegisterReportChannelRunner(
            IGetServerUseCase getServerUseCase,
            IRegisterReportChannelUseCase registerReportChannelUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(akkaService, getRoleLevelUseCase)
        {
            this.getServerUseCase = getServerUseCase;
            this.registerReportChannelUseCase = registerReportChannelUseCase;
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
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from server in getServerUseCase.Execute(
                    user,
                    serverName,
                    guildId)
                from _1 in registerReportChannelUseCase.Execute(
                    user,
                    new ReportChannel(
                        server.Id,
                        guildId,
                        channelId))
                select (IInteractionResponse) new TextResponse("Report channel registered");
        }
    }
}