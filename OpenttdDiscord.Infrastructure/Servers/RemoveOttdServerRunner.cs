using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class RemoveOttdServerRunner : OttdSlashCommandRunnerBase, IOttdSlashCommandRunner
    {
        private readonly IRemoveOttdServerUseCase removeOttdServerUseCase;

        public RemoveOttdServerRunner(IRemoveOttdServerUseCase removeOttdServerUseCase)
        {
            this.removeOttdServerUseCase = removeOttdServerUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            return
            from _1 in removeOttdServerUseCase.Execute(new User(command.User), command.GuildId!.Value, serverName).ToAsync()
            select (ISlashCommandResponse) new TextCommandResponse($"{serverName} successfully deleted");
        }
    }
}
