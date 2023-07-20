﻿using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class GetStatusMonitorsForServerUseCase : UseCaseBase, IGetStatusMonitorsForServerUseCase
    {
        private readonly IStatusMonitorRepository statusMonitorRepository;

        public GetStatusMonitorsForServerUseCase(IStatusMonitorRepository statusMonitorRepository,
                                                 IAkkaService akkaService)
        : base(akkaService)
        {
            this.statusMonitorRepository = statusMonitorRepository;
        }

        public EitherAsync<IError, List<StatusMonitor>> Execute(User user, Guid serverId)
        {
            return
            from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
            from servers in statusMonitorRepository.GetStatusMonitors(serverId)
            select servers;
        }
    }
}
