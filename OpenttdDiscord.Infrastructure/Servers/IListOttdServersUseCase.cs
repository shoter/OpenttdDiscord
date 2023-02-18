using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Servers
{
    public interface IListOttdServersUseCase
    {
        Task<Either<IError, List<OttdServer>>> Execute(UserRights rights, ulong guildId);
    }
}
