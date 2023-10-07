using LanguageExt;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Messages;

namespace OpenttdDiscord.Infrastructure.Roles.UseCases
{
    public class DeleteRoleLevelUseCase : IDeleteRoleLevelUseCase
    {
        private readonly IRolesRepository rolesRepository;

        private readonly IAkkaService akkaService;

        public DeleteRoleLevelUseCase(
            IRolesRepository rolesRepository,
            IAkkaService akkaService)
        {
            this.rolesRepository = rolesRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(
            ulong guildId,
            ulong roleId)
        {
            var @return =
                from _1 in rolesRepository.DeleteRole(
                    guildId,
                    roleId)
                from selection in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in selection.TellExt(
                        new DeleteRole(
                            guildId,
                            roleId))
                    .ToAsync()
                select Unit.Default;

            return @return;
        }
    }
}