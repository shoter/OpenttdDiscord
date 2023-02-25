using LanguageExt;
using OpenttdDiscord.Domain.Chatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Chatting
{
    internal class ChatChannelRepository : IChatChannelRepository
    {
        private OttdContext DB { get; }

        public ChatChannelRepository(OttdContext dB)
        {
            DB = dB;
        }

        public EitherAsyncUnit Delete(Guid serverId, ulong channelId)
        {
            throw new NotImplementedException();
        }

        public EitherAsync<IError, List<ChatChannel>> GetChatChannelsForServer(Guid serverId)
        {
            throw new NotImplementedException();
        }

        public EitherAsyncUnit Insert(ChatChannel chatChannel)
        {
            throw new NotImplementedException();
        }
    }
}
