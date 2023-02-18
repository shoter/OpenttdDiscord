using Discord.WebSocket;
using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class RegisterServerRunner : IOttdSlashCommandRunner
    {
        private readonly IRegisterOttdServerUseCase useCase;

        public RegisterServerRunner(IRegisterOttdServerUseCase useCase)
        {
            this.useCase = useCase;

        }
        public async Task<Either<IError,  ISlashCommandResponse>> Run(SocketSlashCommand command)
        {
            return (await new TryAsync<Either<IError, ISlashCommandResponse>> (async () =>
            {
                if (command.GuildId == null)
                {
                    return Either<IError, ISlashCommandResponse>.Left(new HumanReadableError("GuildId is Null - wtf?"));
                }

                var options = command.Data.Options.ToDictionary(o => o.Name, o => o.Value);
                string name = (string)options["name"];
                string password = (string)options["password"];
                string ip = (string)options["ip"];
                int port = (int)(long)options["port"];

                var rights = new UserRights(UserLevel.Admin);

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
