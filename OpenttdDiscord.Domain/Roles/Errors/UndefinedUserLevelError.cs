using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Roles.Errors
{
    public class UndefinedUserLevelError : HumanReadableError
    {
        public UndefinedUserLevelError()
            : base("Undefined user level!")
        {
        }
    }
}
