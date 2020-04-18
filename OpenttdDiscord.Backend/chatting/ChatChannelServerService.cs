using OpenttdDiscord.Backend.Chatting;
using OpenttdDiscord.Backend.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Chatting
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

        public async Task<ChatChannelServer> Getsert(string ip, int port, ulong channelId,  string serverName)
        {
            Server server = await this.serverService.Getsert(ip, port, serverName);

            ChatChannelServer chatChannelServer = await this.chatChannelServerRepository.Get(server.Id, channelId);

            if(chatChannelServer == null)
            {
                chatChannelServer = await this.chatChannelServerRepository.Insert(server, channelId);

                this.Added?.Invoke(this, chatChannelServer);
            }

            return chatChannelServer;
        }

        public async Task<bool> Exists(string ip, int port, ulong channelId)
        {
            Server server = await this.serverService.Get(ip, port);

            if (server == null) return false;

            ChatChannelServer chatChannelServer = await this.chatChannelServerRepository.Get(server.Id, channelId);

            return chatChannelServer != null;
        }

        public Task<List<ChatChannelServer>> GetAll() => this.chatChannelServerRepository.GetAllAsync();
    }
}
