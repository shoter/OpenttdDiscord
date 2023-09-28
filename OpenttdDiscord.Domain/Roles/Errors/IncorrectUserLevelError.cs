using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Roles.Errors
{
    public class IncorrectUserLevelError : HumanReadableError
    {
        public IncorrectUserLevelError(string reason)
            : base(reason)
        {
        }
    }
}