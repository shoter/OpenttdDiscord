using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Reporting.UseCases
{
    public interface IListReportChannelsUseCase
    {
        EitherAsync<IError, List<ReportChannel>> Execute(User user, Guid serverId);

        EitherAsync<IError, List<ReportChannel>> Execute(User user, ulong guildId);
    }
}
