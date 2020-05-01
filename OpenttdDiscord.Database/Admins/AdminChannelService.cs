using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public class AdminChannelService : IAdminChannelService
    {
        public event EventHandler<AdminChannel> Added;
        public event EventHandler<AdminChannel> Removed;

        private readonly IAdminChannelRepository adminChannelRepository;

        public AdminChannelService(IAdminChannelRepository adminChannelRepository)
        {
            this.adminChannelRepository = adminChannelRepository;
        }

        public async Task<AdminChannel> Add(Server server, ulong channelId, string prefix)
        {
            AdminChannel channel = await this.adminChannelRepository.Insert(server, channelId, prefix);
            this.Added?.Invoke(this, channel);
            return channel;
        }

        public Task<List<AdminChannel>> GetAll(ulong guildId) => this.adminChannelRepository.GetAdminChannels(guildId);
        public Task<AdminChannel> Get(ulong channelId) => this.adminChannelRepository.GetAdminChannelsForChannel(channelId);

        public Task<List<AdminChannel>> GetAll(Server server) => this.adminChannelRepository.GetAdminChannels(server);

        public async Task Remove(AdminChannel adminChannel)
        {
            await this.adminChannelRepository.RemoveChannel(adminChannel.Server.Id, adminChannel.ChannelId);
            this.Removed?.Invoke(this, adminChannel);
        }

        public Task<List<AdminChannel>> GetAll() => this.adminChannelRepository.GetAll();
    }
}
