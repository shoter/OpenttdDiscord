using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Chatting.UseCases
{
    public interface IUnregisterChatChannelUseCase
    {
        EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong chatChannelId);
    }
}
