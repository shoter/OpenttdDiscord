using LanguageExt;
using LanguageExt.Common;

namespace OpenttdDiscord.Domain.Servers
{
    public interface IRegisterOttdServerUseCase : IUseCase
    {
        Task<Result<Unit>> Execute(OttdServer server);
    }
}
