using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Guilds.UseCases
{
    public interface IGetAllGuildsUseCase
    {
        EitherAsync<IError, List<ulong>> Execute();
    }
}
