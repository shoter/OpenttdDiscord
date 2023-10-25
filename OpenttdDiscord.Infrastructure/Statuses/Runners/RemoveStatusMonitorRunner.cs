using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Statuses.Runners
{
    internal class RemoveStatusMonitorRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRemoveStatusMonitorUseCase removeStatusMonitorUseCase;

        private readonly IOttdServerRepository ottdServerRepository;

        public RemoveStatusMonitorRunner(
            IRemoveStatusMonitorUseCase removeStatusMonitorUseCase,
            IOttdServerRepository ottdServerRepository,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.removeStatusMonitorUseCase = removeStatusMonitorUseCase;
            this.ottdServerRepository = ottdServerRepository;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            var _ =
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from server in ottdServerRepository.GetServerByName(
                    command.GuildId!.Value,
                    serverName)
                from _2 in removeStatusMonitorUseCase.Execute(
                    user,
                    server.Id,
                    command.GuildId!.Value,
                    command.ChannelId!.Value)
                select (IInteractionResponse) new TextResponse("Status monitor removed!");
            return _;
        }
    }
}