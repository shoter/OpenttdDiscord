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
        public event EventHandler<InRegisterChatChannelServer> NewChannelInRegistered;


        private Dictionary<ulong, InRegisterChatChannelServer> NewChannelsInRegisterProcess { get; } = new Dictionary<ulong, InRegisterChatChannelServer>();

        private readonly IChatChannelServerRepository chatChannelServerRepository;
        private readonly IServerService serverService;
        private readonly ITimeProvider timeProvider;

        public ChatChannelServerService(IChatChannelServerRepository chatChannelServerRepository, IServerService serverService, ITimeProvider timeProvider)
        {
            this.chatChannelServerRepository = chatChannelServerRepository;
            this.serverService = serverService;
            this.timeProvider = timeProvider;
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

        public void InformAboutNewChannelInRegisterProcess(InRegisterChatChannelServer inRegister)
        {
            this.NewChannelsInRegisterProcess.Add(inRegister.UserId, inRegister);
            this.NewChannelInRegistered?.Invoke(this, inRegister);
        }

        public bool IsServerInRegisterProcess(ulong userId, string serverName, ulong channelId)
        {
            if (!NewChannelsInRegisterProcess.ContainsKey(userId))
                return false;
            var inReg = NewChannelsInRegisterProcess[userId];

            if(timeProvider.Now > inReg.ExpiryTime)
            {
                NewChannelsInRegisterProcess.Remove(userId);
                return false;
            }
            return true;
        }
    }
}
