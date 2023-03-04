using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using OpenttdDiscord.Infrastructure.Servers.UseCases;

namespace OpenttdDiscord.Infrastructure.Servers.Runners
{
    internal class RemoveOttdServerRunner : OttdSlashCommandRunnerBase, IOttdSlashCommandRunner
    {
        private readonly IRemoveOttdServerUseCase removeOttdServerUseCase;

        public RemoveOttdServerRunner(IRemoveOttdServerUseCase removeOttdServerUseCase)
        {
            this.removeOttdServerUseCase = removeOttdServerUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            return
            from _1 in removeOttdServerUseCase.Execute(user, command.GuildId!.Value, serverName).ToAsync()
            select (ISlashCommandResponse)new TextCommandResponse($"{serverName} successfully deleted");
        }
    }
}
