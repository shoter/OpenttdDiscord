using Discord;
using LanguageExt;
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
            RegisterNewRole msg = new(
                guildId,
                role.Id,
                userLevel);

            return
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _1 in actor.TryAsk(msg)
                select Unit.Default;
        }
    }
}