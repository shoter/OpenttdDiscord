using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Servers
{
    public interface IRegisterOttdServerUseCase : IUseCase
    {
        EitherAsyncUnit Execute(User userRights, OttdServer server);
    }
}
