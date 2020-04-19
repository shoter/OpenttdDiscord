using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Chatting
{
    public class ChatChannelServerService : IChatChannelServerService
    {
        public event EventHandler<ChatChannelServer> Added;
        private readonly IChatChannelServerRepository chatChannelServerRepository;
        private readonly IServerService serverService;

        public ChatChannelServerService(IChatChannelServerRepository chatChannelServerRepository, IServerService serverService)
        {
            this.chatChannelServerRepository = chatChannelServerRepository;
            this.serverService = serverService;
        }

        public async Task<ChatChannelServer> Insert(string serverName, ulong channelId)
        {
            Server server = await this.serverService.Get(serverName);
            var chatChannelServer = await this.chatChannelServerRepository.Insert(server, channelId);
            this.Added?.Invoke(this, chatChannelServer);
            return chatChannelServer;
        }

        public async Task<bool> Exists(string serverName, ulong channelId)
        {
            Server server = await this.serverService.Get(serverName);

            if (server == null) return false;

            ChatChannelServer chatChannelServer = await this.chatChannelServerRepository.Get(server.Id, channelId);

            return chatChannelServer != null;
        }

        public Task<List<ChatChannelServer>> GetAll() => this.chatChannelServerRepository.GetAll();
    }
}
