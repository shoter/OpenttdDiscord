using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Statuses.UseCases
{
    public interface IUpdateStatusMonitorUseCase
    {
        EitherAsync<IError, StatusMonitor> Execute(User user, StatusMonitor monitor);
    }
}
