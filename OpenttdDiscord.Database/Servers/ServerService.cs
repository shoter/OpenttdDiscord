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

        private Dictionary<(ulong, ulong), NewServerPassword> NewServerPasswordRequests { get; } = new Dictionary<(ulong, ulong), NewServerPassword>();

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
            this.NewServerPasswordRequests.Add((newPassword.GuildId, newPassword.UserId), newPassword);
            this.NewServerPasswordRequestAdded?.Invoke(this, newPassword);
        }

        public NewServerPassword RemoveNewPasswordRequest(ulong guildId, ulong userId)
        {
            var nsp = this.NewServerPasswordRequests[(guildId, userId)];
            this.NewServerPasswordRequests.Remove((guildId, userId));
            return nsp;
        }

        public bool IsPasswordRequestInProgress(ulong guildId, ulong userId)
        {
            if (!NewServerPasswordRequests.ContainsKey((guildId, userId)))
                return false;
            var inReg = NewServerPasswordRequests[(guildId, userId)];

            if (timeProvider.Now > inReg.ExpiryTime)
            {
                NewServerPasswordRequests.Remove((guildId, userId));
                return false;
            }
            return true;

        }

        public NewServerPassword GetNewPasswordProcess(ulong guildId, ulong userId)
        {
            if (!NewServerPasswordRequests.ContainsKey((guildId, userId)))
                return null;

            var nsp = NewServerPasswordRequests[(guildId, userId)];
            return nsp;
        }

        public Task ChangePassword(ulong serverId, string newPassword) => this.serverRepository.UpdatePassword(serverId, newPassword);
    }
}
