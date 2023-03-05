using LanguageExt;
using OpenttdDiscord.Domain.Reporting;

namespace OpenttdDiscord.Database.Reporting
{
    internal interface IReportChannelRepository
    {
        EitherAsyncUnit Insert(ReportChannel chatChannel);

        EitherAsyncUnit Delete(Guid serverId, ulong channelId);

        EitherAsync<IError, List<ReportChannel>> GetReportChannel(Guid serverId);

        EitherAsync<IError, Option<ReportChannel>> GetReportChannelsForServer(Guid serverId, ulong channelId);
    }
}
