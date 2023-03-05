using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Servers.UseCases
{
    public interface IRegisterOttdServerUseCase : IUseCase
    {
        EitherAsyncUnit Execute(User userRights, OttdServer server);
    }
}
