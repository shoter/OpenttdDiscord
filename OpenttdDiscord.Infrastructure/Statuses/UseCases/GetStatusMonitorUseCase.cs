using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Database.Statuses;
using OpenttdDiscord.Domain.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Infrastructure.Statuses.UseCases
{
    internal class GetStatusMonitorUseCase : IGetStatusMonitorUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        private readonly IStatusMonitorRepository statusMonitorRepository;

        public GetStatusMonitorUseCase(IOttdServerRepository ottdServerRepository, IStatusMonitorRepository statusMonitorRepository)
        {
            this.ottdServerRepository = ottdServerRepository;
            this.statusMonitorRepository = statusMonitorRepository;
        }

        public EitherAsync<IError, StatusMonitor> Execute(string serverName, ulong channelId, ulong guildId)
        {
            return
            from server in ottdServerRepository.GetServerByName(guildId, serverName).ToAsync()
            from monitor in Execute(server.Id, channelId)
            select monitor;
        }

        public EitherAsync<IError, StatusMonitor> Execute(Guid serverId, ulong channelId)
        {
            //var statusMonitor = statusMonitorRepository
            throw new Exception();
        }
    }
}
