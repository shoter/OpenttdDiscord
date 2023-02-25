using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Domain.Statuses.UseCases
{
    public interface IGetStatusMonitorUseCase
    {
        EitherAsync<IError, StatusMonitor> Execute(User user, string serverName, ulong channelId, ulong guildId);

        EitherAsync<IError, StatusMonitor> Execute(User user, Guid serverId, ulong channelId);
    }
}
