using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPortClientProvider : IAdminPortClientProvider
    {
        private readonly ConcurrentDictionary<string, IAdminPortClient> serverInfos = new ConcurrentDictionary<string, IAdminPortClient>();
        private readonly IAdminPortClientFactory clientFactory;
        private readonly ILogger<IAdminPortClientProvider> logger;

        public AdminPortClientProvider(IAdminPortClientFactory factory, ILogger<IAdminPortClientProvider> logger)
        {
            this.clientFactory = factory;
            this.logger = logger;
        }

        public event EventHandler<IAdminPortClient> NewClientCreated;

        public async Task<IAdminPortClient> GetClient(ServerInfo info)
        {
            return serverInfos.GetOrAdd($"{info}", (_) =>
            {
                var client = clientFactory.Create(info);
                this.NewClientCreated?.Invoke(this, client);
                logger.LogInformation($"Created admin port client for {info}");
                return client;
                });
            
            // TODO : Code for password change.
        }

    }
}
