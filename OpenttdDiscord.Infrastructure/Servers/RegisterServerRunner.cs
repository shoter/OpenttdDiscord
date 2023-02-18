using Discord.WebSocket;
using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class RegisterServerRunner : OttdSlashCommandRunnerBase, IOttdSlashCommandRunner
    {
        private readonly IRegisterOttdServerUseCase useCase;

        public RegisterServerRunner(IRegisterOttdServerUseCase useCase)
        {
            this.useCase = useCase;

        }
        protected override async Task<Either<IError,  ISlashCommandResponse>> RunInternal(SocketSlashCommand command, Dictionary<string, object> options)
        {
            return (await new TryAsync<Either<IError, ISlashCommandResponse>> (async () =>
            {
                if (command.GuildId == null)
                {
                    return Either<IError, ISlashCommandResponse>.Left(new HumanReadableError("GuildId is Null - wtf?"));
                }

                string name = (string)options["name"];
                string password = (string)options["password"];
                string ip = (string)options["ip"];
                int port = (int)(long)options["port"];

                var rights = new User(command.User);

                var server = new OttdServer(
                    Guid.NewGuid(),
                    command.GuildId.Value,
                    ip,
                    name,
                    port,
                    password
                    );

                return (await useCase.Execute(rights, server))
                    .Select<ISlashCommandResponse>(x => new TextCommandResponse($"Created Server {name} - {ip}:{port}"));
            })).IfFail(ex => new ExceptionError(ex));
        }
    }
}
