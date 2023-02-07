using LanguageExt;
using LanguageExt.Common;

namespace OpenttdDiscord.Domain.Ottd
{
    public interface IRegisterOttdServerUseCase : IUseCase
    {
        Task<Result<Unit>> Execute(OttdServer server);
    }
}
