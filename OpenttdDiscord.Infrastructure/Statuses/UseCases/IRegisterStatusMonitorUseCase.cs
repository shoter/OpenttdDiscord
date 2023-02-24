using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal interface IRegisterStatusMonitorUseCase
    {
        EitherAsync<IError, StatusMonitor> Execute(OttdServer server, ulong guildId, ulong channelId);
    }
}
