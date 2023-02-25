using LanguageExt;
using OpenttdDiscord.Domain.Chatting;

namespace OpenttdDiscord.Database.Chatting
{
    public interface IChatChannelRepository
    {
        EitherAsyncUnit Insert(ChatChannel chatChannel);

        EitherAsyncUnit Delete(Guid serverId, ulong channelId);

        EitherAsync<IError, List<ChatChannel>> GetChatChannelsForServer(Guid serverId);

        EitherAsync<IError, Option<ChatChannel>> GetChatChannelsForServer(Guid serverId, ulong channelId);
    }
}
