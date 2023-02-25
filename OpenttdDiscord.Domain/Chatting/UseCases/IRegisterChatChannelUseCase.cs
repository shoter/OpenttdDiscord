using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Domain.Chatting.UseCases
{
    public interface IRegisterChatChannelUseCase
    {
        EitherAsyncUnit Execute(User user, ChatChannel chatChannel);
    }
}
