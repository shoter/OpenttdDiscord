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

        public AdminPortClientProvider(IAdminPortClientFactory factory)
        {
            this.clientFactory = factory;
        }

        public event EventHandler<IAdminPortClient> NewClientCreated;

        public async Task<IAdminPortClient> GetClient(ServerInfo info)
        {
            return serverInfos.GetOrAdd($"{info}", (_) =>
            {
                var client = clientFactory.Create(info);
                this.NewClientCreated?.Invoke(this, client);
                return client;
                });
            
            // TODO : Code for password change.
        }

    }
}
