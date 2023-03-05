using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.EventLogs.UseCases
{
    public interface IQueryEventLogUseCase
    {
        EitherAsync<IError, IReadOnlyList<string>> Execute(User user, Guid serverId, ulong guildId);
    }
}
