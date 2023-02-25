using Discord;
using LanguageExt;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Statuses;
using System.Threading.Channels;
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

        public EitherAsync<IError, StatusMonitor> Insert(StatusMonitor entity)
        {
            return TryAsync(async () =>
            {
                await DB
                    .Monitors
                    .AddAsync(new StatusMonitorEntity(entity));
                await DB.SaveChangesAsync();
                return entity;
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

        public EitherAsync<IError, StatusMonitor> UpdateStatusMonitor(StatusMonitor entity)
        {
            return TryAsync<Either<IError, StatusMonitor>>(async () =>
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
            })
            .ToEitherAsyncErrorFlat();
        }

        public EitherAsync<IError, Option<StatusMonitor>> GetStatusMonitor(Guid serverId, ulong channelId)
            => TryAsync(async () =>
            {
                var monitor = await DB
                .Monitors
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ServerId == serverId);

                if (monitor == null)
                {
                    return Option<StatusMonitor>.None;
                }

                return monitor.ToDomain();
            })
            .ToEitherAsyncError();

    }
}
