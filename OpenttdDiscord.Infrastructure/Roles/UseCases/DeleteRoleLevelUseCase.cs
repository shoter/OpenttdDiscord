using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Roles.UseCases
{
    public class DeleteRoleLevelUseCase : IDeleteRoleLevelUseCase
    {
        private readonly IRolesRepository rolesRepository;

        public DeleteRoleLevelUseCase(IRolesRepository rolesRepository)
        {
            this.rolesRepository = rolesRepository;
        }

        public EitherAsyncUnit Execute(
            ulong guildId,
            ulong roleId) => rolesRepository.DeleteRole(
            guildId,
            roleId);
    }
}
