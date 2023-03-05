using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure
{
    internal class UseCaseBase
    {
        protected EitherUnit CheckIfHasCorrectUserLevel(User user, UserLevel level)
        {
            var hasLevel = level switch
            {
                UserLevel.User => true,
                UserLevel.Moderator => user.UserLevel == UserLevel.Moderator || user.UserLevel == UserLevel.Admin,
                UserLevel.Admin => user.UserLevel == UserLevel.Admin,
                _ => false
            };

            if (!hasLevel)
            {
                return new HumanReadableError("You do not have sufficient privileges to run this command!");
            }

            return Unit.Default;
        }
    }
}
