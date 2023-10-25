using Discord;
using LanguageExt;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.UseCases
{
    public class RegisterRoleUseCase : IRegisterRoleUseCase
    {
        private readonly IAkkaService akkaService;

        public RegisterRoleUseCase(IAkkaService akkaService)
        {
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(
            ulong guildId,
            IRole role,
            UserLevel userLevel)
        {
            UpsertRole msg = new(
                guildId,
                role.Id,
                userLevel);

            if (!Enum.IsDefined(
                    typeof(UserLevel),
                    userLevel))
            {
                return new UndefinedUserLevelError();
            }

            return
                from _1 in akkaService.SelectAndAsk<object>(MainActors.Paths.Guilds, msg)
                select Unit.Default;
        }
    }
}