using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Domain.Statuses.UseCases
{
    public interface IGetStatusMonitorsForServerUseCase
    {
        EitherAsync<IError, List<StatusMonitor>> Execute(User user, Guid serverId);
    }
}
