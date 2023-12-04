using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Domain.Statuses.UseCases
{
    public interface IRegisterStatusMonitorUseCase
    {
        EitherAsync<IError, StatusMonitor> Execute(OttdServer server, ulong guildId, ulong channelId);
    }
}
