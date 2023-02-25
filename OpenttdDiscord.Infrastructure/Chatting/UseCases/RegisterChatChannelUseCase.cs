using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Chatting.UseCases
{
    internal class RegisterChatChannelUseCase : IRegisterChatChannelUseCase
    {
        private readonly IChatChannelRepository chatChannelRepository;

        public RegisterChatChannelUseCase(IChatChannelRepository chatChannelRepository)
        {
            this.chatChannelRepository = chatChannelRepository;
        }

        public EitherAsyncUnit Execute(User user, ChatChannel chatChannel)
            => TryAsync(async () =>
            {
                await chatChannelRepository.Insert(chatChannel);
                return Unit.Default;
            }).ToEitherAsyncError();
    }
}
