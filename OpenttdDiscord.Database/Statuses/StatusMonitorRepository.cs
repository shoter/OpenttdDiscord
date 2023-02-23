using Discord;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Statuses;
using static LanguageExt.Prelude;

namespace OpenttdDiscord.Database.Statuses
{
    internal class StatusMonitorRepository : IStatusMonitorRepository
    {
        private OttdContext DB { get; }

        public StatusMonitorRepository(OttdContext dB)
        {
            DB = dB;
        }

        public EitherAsync<IError, List<StatusMonitor>> GetStatusMonitors(Guid serverId)
        {
            return
                TryAsync(async () =>
                {
                    return await DB
                    .Monitors
                    .AsNoTracking()
                    .Where(m => m.ServerId == serverId)
                    .ToListAsync();
                })
                .ToEitherAsyncError()
                .Select(list => list.Select(m => m.ToDomain()).ToList());
        }

        public EitherAsyncUnit Insert(StatusMonitor entity)
        {
            return TryAsync(async () =>
            {
                await DB
                    .Monitors
                    .AddAsync(new StatusMonitorEntity(entity));
                await DB.SaveChangesAsync();
                return Unit.Default;
            })
            .ToEitherAsyncError();
        }

        public EitherAsyncUnit RemoveStatusMonitor(Guid serverId, ulong channelId)
        {
            return TryAsync<EitherUnit>(async () =>
            {
                int modifiedRows = await DB
                    .Monitors
                    .Where(m => m.ServerId == serverId && m.ChannelId == channelId)
                    .DeleteFromQueryAsync();

                if (modifiedRows == 0)
                {
                    return new HumanReadableError("No status monitors were removed");
                }

                return Unit.Default;
            })
            .ToEitherAsyncErrorFlat();
        }

        public async EitherAsync<IError, StatusMonitor> UpdateStatusMonitor(StatusMonitor entity)
        {
            try
            {
                var monitor = await DB
                    .Monitors
                    .FirstOrDefaultAsync(monitor => monitor.ServerId == entity.ServerId && monitor.ChannelId == entity.ChannelId);

                if (monitor == null)
                {
                    return new HumanReadableError("Monitor not found!");
                }

                monitor.MessageId = entity.MessageId;
                monitor.LastUpdateTime = entity.LastUpdateTime.ToUniversalTime();

                await DB.SaveChangesAsync();
                return monitor.ToDomain();
            }
            catch (Exception ex)
            {
                return new ExceptionError(ex);
            }
        }
    }
}
