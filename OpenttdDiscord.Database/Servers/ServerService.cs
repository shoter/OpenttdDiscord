using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Chatting;
using System;
using System.Collections.Concurrent;
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
        public event EventHandler<Server> PasswordChanged;

        private ConcurrentDictionary<ulong, NewServerPassword> NewServerPasswordRequests { get; } = new ConcurrentDictionary<ulong, NewServerPassword>();

        private readonly IServerRepository serverRepository;
        private readonly ITimeProvider timeProvider;

        public ServerService(IServerRepository serverRepository, ITimeProvider timeProvider)
        {
            this.serverRepository = serverRepository;
            this.timeProvider = timeProvider;
        }

        public async Task<Server> Getsert(ulong guildId, string ip, int port, string serverName)
        {
            Server server = await this.serverRepository.GetServer(guildId, ip, port);

            if (server != null)
                return server;

            server = await this.serverRepository.AddServer(guildId, ip, port, serverName);
            Added?.Invoke(this, server);
            return server;
        }

        public async Task<bool> Exists(ulong guildId, string ip, int port) => await this.serverRepository.GetServer(guildId, ip, port) != null;

        public async Task<bool> Exists(ulong guildId, string serverName) => await this.serverRepository.GetServer(guildId, serverName) != null;

        public Task<Server> Get(ulong guildId, string ip, int port) => this.serverRepository.GetServer(guildId, ip, port);

        public Task<List<Server>> GetAll(ulong guildId) => this.serverRepository.GetAll(guildId);

        public Task<Server> Get(ulong guildId, string serverName) => this.serverRepository.GetServer(guildId, serverName);

        public void InformAboutNewPasswordRequest(NewServerPassword newPassword)
        {
            this.NewServerPasswordRequests.TryAdd(newPassword.UserId, newPassword);
            this.NewServerPasswordRequestAdded?.Invoke(this, newPassword);
        }

        public NewServerPassword RemoveNewPasswordRequest(ulong userId)
        {
            var nsp = this.NewServerPasswordRequests[userId];
            this.NewServerPasswordRequests.TryRemove( userId,out var _);
            return nsp;
        }

        public bool IsPasswordRequestInProgress(ulong userId)
        {
            if (!NewServerPasswordRequests.ContainsKey(userId))
                return false;
            var inReg = NewServerPasswordRequests[userId];

            if (timeProvider.Now > inReg.ExpiryTime)
            {
                NewServerPasswordRequests.Remove(userId, out var _);
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

        public async Task ChangePassword(ulong serverId, string newPassword)
        {
            await this.serverRepository.UpdatePassword(serverId, newPassword);
            Server server = await this.serverRepository.GetServer(serverId);
            PasswordChanged?.Invoke(this, server);
        }

        public Task<List<Server>> Get(string ip, int port) => this.serverRepository.GetServers(ip, port);
    }
}
