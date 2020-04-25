using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public class SubscribedServerService : ISubscribedServerService
    {
        private readonly IServerRepository serverRepository;

        private readonly ISubscribedServerRepository subscribedServerRepository;

        public SubscribedServerService(IServerRepository serverRepo, ISubscribedServerRepository subServRepo)
        {
            this.serverRepository = serverRepo;
            this.subscribedServerRepository = subServRepo;
        }

        public event EventHandler<SubscribedServer> ServerAdded;
        public event EventHandler<SubscribedServer> ServerRemoved;

        public async Task<SubscribedServer> AddServer(string name, int port ,ulong channelId)
        {
            var server = await serverRepository.GetServer(name);

            if (await this.subscribedServerRepository.Exists(server, channelId))
                return await this.subscribedServerRepository.Get(server, channelId);

            SubscribedServer ss = await this.subscribedServerRepository.Add(server, port, channelId);
            this.ServerAdded?.Invoke(this, ss);
            return ss;
        }

        public async Task<bool> Exists(string name, ulong channelId)
        {
            var server = await serverRepository.GetServer(name);
            return await this.subscribedServerRepository.Exists(server, channelId);
        }

        public Task<IEnumerable<SubscribedServer>> GetAllServers() => this.subscribedServerRepository.GetAll();

        public async Task RemoveServer(string name, ulong channelId)
        {
            var server = await serverRepository.GetServer(name);
            var subServer = await subscribedServerRepository.Get(server, channelId);
            await this.subscribedServerRepository.Remove(server, channelId);
            this.ServerRemoved?.Invoke(this, subServer);
        }

        public Task UpdateServer(ulong serverId, ulong channelId, ulong messageId) => this.subscribedServerRepository.UpdateServer(serverId, channelId, messageId);
    }
}
