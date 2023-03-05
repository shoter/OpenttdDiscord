using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Guilds.UseCases
{
    public interface IGetAllGuildsUseCase
    {
        Task<Either<IError, List<ulong>>> Execute();
    }
}
