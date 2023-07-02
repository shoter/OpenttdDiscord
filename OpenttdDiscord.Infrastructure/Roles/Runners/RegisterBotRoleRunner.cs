using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Roles.Runners
{
    internal class RegisterBotRoleRunner : OttdSlashCommandRunnerBase
    {
        private readonly IAkkaService akkaService;

        public RegisterBotRoleRunner(IAkkaService akkaService)
        {
            this.akkaService = akkaService;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            SocketSlashCommand command,
            User user,
            ExtDictionary<string, object> options)
        {
            ulong guildID = command.GuildId!.Value;
            SocketRole? role = options["role"] as SocketRole;
            UserLevel roleLevel = (UserLevel)options["role-level"];

            if (role == null)
            {
                throw new ArgumentException(
                    options["role"]
                        .GetType()
                        .Name);
            }

            var x =
                from _1 in akkaService.SelectActor(MainActors.Paths.Guilds)

            throw new NotImplementedException();
        }
    }
}