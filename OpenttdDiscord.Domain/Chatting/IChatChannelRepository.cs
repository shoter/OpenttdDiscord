using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Chatting
{
    public interface IChatChannelRepository
    {
        EitherAsyncUnit Insert(ChatChannel chatChannel);

        EitherAsyncUnit Delete(Guid serverId, ulong channelId);

        EitherAsync<IError, List<ChatChannel>> GetChatChannelsForServer(Guid serverId);

        EitherAsync<IError, Option<ChatChannel>> GetChatChannelForServer(Guid serverId, ulong channelId);
    }
}
