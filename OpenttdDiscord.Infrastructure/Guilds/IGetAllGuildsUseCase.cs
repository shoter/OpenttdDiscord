using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    internal interface IGetAllGuildsUseCase
    {
        Task<Either<IError, List<ulong>>> Execute();
    }
}
