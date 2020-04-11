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

        public async Task<IAdminPortClient> GetClient(ServerInfo info)
        {
            if (serverInfos.TryGetValue(GetKey(info), out IAdminPortClient client))
            {
                if (info.Password != client.ServerInfo.Password)
                {
                    serverInfos.Remove(GetKey(info), out _);
                    if (client.ConnectionState != AdminConnectionState.Idle)
                        await client.Disconnect();
                    client = clientFactory.Create(info);
                    serverInfos.TryAdd(GetKey(info), client);
                }
            }
            else
            {
                client = clientFactory.Create(info);
                serverInfos.TryAdd(GetKey(info), client);
            }

            return client;

        }

        private string GetKey(ServerInfo info) => $"{info.ServerIp}:{info.ServerPort}";
    }
}
