using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;

namespace OpenttdDiscord.Infrastructure
{
    internal class UseCaseBase
    {
        protected EitherUnit CheckIfHasCorrectUserLevel(User user, UserLevel level)
        {
            var hasLevel = user.CheckIfHasCorrectUserLevel(level);

            if (!hasLevel)
            {
                return new HumanReadableError("You do not have sufficient privileges to run this use case!");
            }

            return Unit.Default;
        }
    }
}
