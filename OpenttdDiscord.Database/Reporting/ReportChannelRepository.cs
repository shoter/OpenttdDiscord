using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Reporting;

namespace OpenttdDiscord.Database.Reporting
{
    internal class ReportChannelRepository : IReportChannelRepository
    {
        private OttdContext DB { get; }

        public ReportChannelRepository(OttdContext dB)
        {
            DB = dB;
        }

        public EitherAsyncUnit Delete(Guid serverId, ulong channelId)
            => TryAsync<EitherUnit>(async () =>
            {
                int deletedRows = await DB.ReportChannels
                    .Where(rc => rc.ServerId == serverId && rc.ChannelId == channelId)
                    .DeleteFromQueryAsync();

                if (deletedRows == 0)
                {
                    return new HumanReadableError("No report channel was found for deletion");
                }

                return Unit.Default;
            }).ToEitherAsyncErrorFlat();

        public EitherAsync<IError, List<ReportChannel>> GetReportChannel(Guid serverId)
            => TryAsync<Either<IError, List<ReportChannel>>>(async () =>
            {
                return (await DB.ReportChannels
                    .AsNoTracking()
                    .Where(rc => rc.ServerId == serverId)
                    .ToListAsync())
                    .Select(rc => rc.ToDomain())
                    .ToList();
            }).ToEitherAsyncErrorFlat();

        public EitherAsyncUnit Insert(ReportChannel reportChannel)
            => TryAsync<EitherUnit>(async () =>
            {
                await DB.ReportChannels.AddAsync(new(reportChannel));
                await DB.SaveChangesAsync();
                return Unit.Default;
            }).ToEitherAsyncErrorFlat();

        public EitherAsync<IError, Option<ReportChannel>> GetReportChannelsForServer(Guid serverId, ulong channelId)
              => TryAsync<Either<IError, Option<ReportChannel>>>(async () =>
              {
                  var reportChannel = await DB.ReportChannels
                      .AsNoTracking()
                      .FirstOrDefaultAsync(rc => rc.ServerId == serverId && rc.ChannelId == channelId);

                  if (reportChannel == null)
                  {
                      return Option<ReportChannel>.None;
                  }

                  return Option<ReportChannel>.Some(reportChannel.ToDomain());
              }).ToEitherAsyncErrorFlat();
    }
}
