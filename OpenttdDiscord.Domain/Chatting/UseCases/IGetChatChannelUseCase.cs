using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Chatting.UseCases
{
    public interface IGetChatChannelUseCase
    {
        EitherAsync<IError, Option<ChatChannel>> Execute(User user, Guid serverId, ulong channelId);

        EitherAsync<IError, List<ChatChannel>> Execute(User user, Guid serverId);
    }
}
