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

        protected override async Task<Either<IError, ISlashCommandResponse>> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            return (await this.removeOttdServerUseCase.Execute(new User(command.User), command.GuildId!.Value, serverName))
                .Map<ISlashCommandResponse>(_ => new TextCommandResponse($"{serverName} successfully deleted"));
        }
    }
}
