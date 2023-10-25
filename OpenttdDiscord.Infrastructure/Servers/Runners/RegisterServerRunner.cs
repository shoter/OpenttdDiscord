using Discord;
using Discord.WebSocket;
using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.Servers.Runners
{
    internal class RegisterServerRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRegisterOttdServerUseCase useCase;

        public RegisterServerRunner(
            IRegisterOttdServerUseCase useCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.useCase = useCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            if (command.GuildId == null)
            {
                return new HumanReadableError("GuildId is Null - wtf?");
            }

            string name = options.GetValueAs<string>("name");
            string password = options.GetValueAs<string>("password");
            string ip = options.GetValueAs<string>("ip");
            int port = (int) options.GetValueAs<long>("port");

            var server = new OttdServer(
                Guid.NewGuid(),
                command.GuildId!.Value,
                ip,
                name,
                port,
                password
            );

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _1 in useCase.Execute(
                    user,
                    server)
                select (ISlashCommandResponse) new TextCommandResponse($"Created Server {name} - {ip}:{port}");
        }
    }
}