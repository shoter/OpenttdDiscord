using Discord.WebSocket;
using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord;

namespace OpenttdDiscord.Infrastructure.Servers.Runners
{
    internal class RegisterServerRunner : OttdSlashCommandRunnerBase, IOttdSlashCommandRunner
    {
        private readonly IRegisterOttdServerUseCase useCase;

        public RegisterServerRunner(IRegisterOttdServerUseCase useCase)
        {
            this.useCase = useCase;

        }
        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            if (command.GuildId == null)
            {
                return new HumanReadableError("GuildId is Null - wtf?");
            }

            string name = options.GetValueAs<string>("name");
            string password = options.GetValueAs<string>("password");
            string ip = options.GetValueAs<string>("ip");
            int port = (int)options.GetValueAs<long>("port");

            var rights = new User(command.User);

            var server = new OttdServer(
                Guid.NewGuid(),
                command.GuildId!.Value,
                ip,
                name,
                port,
                password
                );

            return
            from _1 in useCase.Execute(rights, server)
            select (ISlashCommandResponse)new TextCommandResponse($"Created Server {name} - {ip}:{port}");
        }
    }
}
