﻿using LanguageExt;
using OpenttdDiscord.Domain.Statuses;

namespace OpenttdDiscord.Database.Statuses
{
    internal interface IStatusMonitorRepository
    {
        EitherAsync<IError, StatusMonitor> Insert(StatusMonitor entity);

        EitherAsync<IError, List<StatusMonitor>> GetStatusMonitors(Guid serverId);

        EitherAsync<IError, Option<StatusMonitor>> GetStatusMonitor(Guid serverId, ulong channelId);

        EitherAsyncUnit RemoveStatusMonitor(Guid serverId, ulong channelId);

        EitherAsync<IError, StatusMonitor> UpdateStatusMonitor(StatusMonitor entity);
    }
}
