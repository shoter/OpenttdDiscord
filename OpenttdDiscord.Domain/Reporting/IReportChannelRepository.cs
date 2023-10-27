namespace OpenttdDiscord.Domain.Reporting
{
    public interface IReportChannelRepository
    {
        EitherAsyncUnit Insert(ReportChannel chatChannel);

        EitherAsyncUnit Delete(Guid serverId, ulong channelId);

        EitherAsync<IError, List<ReportChannel>> GetReportChannelsForServer(Guid serverId);

        EitherAsync<IError, Option<ReportChannel>> GetReportChannel(Guid serverId, ulong channelId);

        EitherAsync<IError, List<ReportChannel>> GetReportChannelsForGuild(ulong guildId);
    }
}
