using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Chatting;
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
        public event EventHandler<NewServerPassword> NewServerPasswordRequestAdded;

        private Dictionary<ulong, NewServerPassword> NewServerPasswordRequests { get; } = new Dictionary<ulong, NewServerPassword>();


        private readonly IServerRepository serverRepository;
        private readonly ITimeProvider timeProvider;

        public ServerService(IServerRepository serverRepository, ITimeProvider timeProvider)
        {
            this.serverRepository = serverRepository;
            this.timeProvider = timeProvider;
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

        public void InformAboutNewPasswordRequest(NewServerPassword newPassword)
        {
            this.NewServerPasswordRequests.Add(newPassword.UserId, newPassword);
            this.NewServerPasswordRequestAdded?.Invoke(this, newPassword);
        }

        public NewServerPassword RemoveNewPasswordRequest(ulong userId)
        {
            var nsp = this.NewServerPasswordRequests[userId];
            this.NewServerPasswordRequests.Remove(userId);
            return nsp;
        }

        public bool IsPasswordRequestInProgress(ulong userId)
        {
            if (!NewServerPasswordRequests.ContainsKey(userId))
                return false;
            var inReg = NewServerPasswordRequests[userId];

            if (timeProvider.Now > inReg.ExpiryTime)
            {
                NewServerPasswordRequests.Remove(userId);
                return false;
            }
            return true;

        }

        public NewServerPassword GetNewPasswordProcess(ulong userId)
        {
            if (!NewServerPasswordRequests.ContainsKey(userId))
                return null;

            var nsp = NewServerPasswordRequests[userId];
            return nsp;
        }

        public Task ChangePassword(ulong serverId, string newPassword) => this.serverRepository.UpdatePassword(serverId, newPassword);
    }
}
