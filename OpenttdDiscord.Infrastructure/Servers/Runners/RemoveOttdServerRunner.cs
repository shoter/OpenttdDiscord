using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Servers.UseCases;

namespace OpenttdDiscord.Infrastructure.Servers.Runners
{
    internal class RemoveOttdServerRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRemoveOttdServerUseCase removeOttdServerUseCase;

        public RemoveOttdServerRunner(
            IRemoveOttdServerUseCase removeOttdServerUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(akkaService, getRoleLevelUseCase)
        {
            this.removeOttdServerUseCase = removeOttdServerUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _1 in removeOttdServerUseCase.Execute(
                        user,
                        command.GuildId!.Value,
                        serverName)
                    .ToAsync()
                select (ISlashCommandResponse) new TextCommandResponse($"{serverName} successfully deleted");
        }
    }
}