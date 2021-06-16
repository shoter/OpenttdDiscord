using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;

namespace OpenttdDiscord.Backend.Admins
{
    public class AdminPortClientFactory : IAdminPortClientFactory
    {
        private readonly ILogger<AdminPortClient> clientLogger;

        private readonly ILogger<AdminPortClientFactory> logger;

        public AdminPortClientFactory(ILogger<AdminPortClientFactory> logger, ILogger<AdminPortClient> clientLogger)
        {
            this.clientLogger = clientLogger;
            this.logger = logger;
        }

        public virtual IAdminPortClient Create(ServerInfo info)
        {
            logger.LogInformation($"Creating admin port client for {info}");
            var client = new AdminPortClient(info, clientLogger);
            return client;
        }
    }
}
