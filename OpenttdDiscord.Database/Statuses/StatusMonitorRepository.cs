using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Database.Statuses
{
    internal class StatusMonitorRepository : IStatusMonitorRepository
    {
        private OttdContext DB { get; }

        public StatusMonitorRepository(OttdContext dB)
        {
            DB = dB;
        }

        public async Task<Either<IError, List<StatusMonitor>>> GetStatusMonitors(Guid serverId)
        {
            try
            {
                return (await DB
                    .Monitors
                    .AsNoTracking()
                    .Where(m => m.ServerId == serverId)
                    .ToListAsync())
                    .Select(m => m.ToDomain())
                    .ToList();
            }
            catch(Exception ex)
            {
                return new ExceptionError(ex);
            }
        }

        public async Task<EitherUnit> Insert(StatusMonitor entity)
        {
            try
            {
                await DB
                    .Monitors
                    .AddAsync(new StatusMonitorEntity(entity));
                await DB.SaveChangesAsync();

                return Unit.Default;

            }
            catch (Exception ex)
            {
                return new ExceptionError(ex);
            }

        }

        public async Task<EitherUnit> RemoveStatusMonitor(Guid serverId, ulong channelId)
        {
            try
            {
                int modifiedRows = await DB
                    .Monitors
                    .Where(m => m.ServerId == serverId && m.ChannelId == channelId)
                    .DeleteFromQueryAsync();

                if(modifiedRows == 0)
                {
                    return new HumanReadableError("No status monitors were removed");
                }

                return Unit.Default;

            }
            catch (Exception ex)
            {
                return new ExceptionError(ex);
            }
        }

        public async Task<Either<IError, StatusMonitor>> UpdateStatusMonitor(StatusMonitor entity)
        {
            try
            {
                var monitor = await DB
                    .Monitors
                    .FirstOrDefaultAsync(monitor => monitor.ServerId == entity.ServerId && monitor.ChannelId == entity.ChannelId);

                if(monitor == null)
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
