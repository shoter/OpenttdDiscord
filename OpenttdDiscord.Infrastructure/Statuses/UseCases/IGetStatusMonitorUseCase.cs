using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal interface IGetStatusMonitorUseCase
    {
        EitherAsync<IError, StatusMonitor> Execute(string serverName, ulong channelId, ulong guildId);

        EitherAsync<IError, StatusMonitor> Execute(Guid serverId, ulong channelId);
    }
}
