using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal interface IRegisterStatusMonitorUseCase
    {
        EitherAsync<IError, StatusMonitor> Execute(User user, OttdServer server, ulong guildId, ulong channelId);
    }
}
