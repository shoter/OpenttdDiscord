using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPortClientFactory : IAdminPortClientFactory
    {
        private readonly IAdminPacketService adminPacketService;
        private readonly ILogger<IAdminPortClient> logger;

        public AdminPortClientFactory(IAdminPacketService adminPacketService, ILogger<IAdminPortClient> logger)
        {
            this.adminPacketService = adminPacketService;
            this.logger = logger;
        }

        public virtual IAdminPortClient Create(ServerInfo info)
        {
            return new AdminPortClient(info, adminPacketService, logger);
        }
    }
}
