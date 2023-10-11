using Discord;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Roles.UseCases
{
    public interface IRegisterRoleUseCase
    {
        EitherAsyncUnit Execute(
            ulong guildId,
            IRole role,
            UserLevel userLevel
        );
    }
}