using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Roles.Messages
{
    public record GetRoleLevelResponse(
        UserLevel RoleLevel
    );
}