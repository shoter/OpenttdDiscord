using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using OpenttdDiscord.Infrastructure.Roles.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.Runners
{
    internal class RegisterBotRoleRunner : OttdSlashCommandRunnerBase
    {
        public RegisterBotRoleRunner(IAkkaService akkaService,
                                     IGetRoleLevelUseCase getRoleLevelUseCase)
        : base(akkaService, getRoleLevelUseCase)
        {
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            ulong guildID = command.GuildId!.Value;
            SocketRole? role = options["role"] as SocketRole;
            long roleLevel = (long)options["role-level"];

            if (role == null)
            {
                throw new ArgumentException(
                    options["role"]
                        .GetType()
                        .Name);
            }

            RegisterNewRole msg = new(
                guildID,
                role.Id,
                (UserLevel)roleLevel);

            return
                from _0 in CheckIfHasCorrectUserLevel(
                    user,
                    UserLevel.Admin).ToAsync()
                from actor in AkkaService.SelectActor(MainActors.Paths.Guilds)
                from _1 in actor.TryAsk(msg)
                select new TextCommandResponse("Role has been registered") as ISlashCommandResponse;
        }
    }
}