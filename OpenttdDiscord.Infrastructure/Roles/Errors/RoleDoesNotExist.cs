using Discord;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Roles.Errors
{
    public class RoleDoesNotExist : HumanReadableError
    {
        public RoleDoesNotExist(IRole role)
            : base($"Role {role.Name} does not have any permissions set")
        {
        }
    }
}