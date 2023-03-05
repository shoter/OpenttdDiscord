using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Domain.Servers.UseCases
{
    public interface IListOttdServersUseCase
    {
        Task<Either<IError, List<OttdServer>>> Execute(User rights, ulong guildId);
    }
}
