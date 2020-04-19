using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public class ServerService : IServerService
    {
        public event EventHandler<Server> Added;

        private readonly IServerRepository serverRepository;

        public ServerService(IServerRepository serverRepository)
        {
            this.serverRepository = serverRepository;
        }

        public async Task<Server> Getsert(string ip, int port, string serverName)
        {
            Server server = await this.serverRepository.GetServer(ip, port);

            if (server != null)
                return server;

            server = await this.serverRepository.AddServer(ip, port, serverName);
            Added?.Invoke(this, server);
            return server;
        }

        public async Task<bool> Exists(string ip, int port) => await this.serverRepository.GetServer(ip, port) != null;

        public async Task<bool> Exists(string serverName) => await this.serverRepository.GetServer(serverName) != null;

        public Task<Server> Get(string ip, int port) => this.serverRepository.GetServer(ip, port);

        public Task<List<Server>> GetAll() => this.serverRepository.GetAll();

        public Task<Server> Get(string serverName) => this.serverRepository.GetServer(serverName);
    }
}
