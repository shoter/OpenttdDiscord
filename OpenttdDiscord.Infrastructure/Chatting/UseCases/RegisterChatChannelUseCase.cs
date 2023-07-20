using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.UseCases
{
    internal class RegisterChatChannelUseCase : UseCaseBase, IRegisterChatChannelUseCase
    {
        private readonly IChatChannelRepository chatChannelRepository;

        public RegisterChatChannelUseCase(
            IChatChannelRepository chatChannelRepository,
            IAkkaService akkaService)
        : base(akkaService)
        {
            this.chatChannelRepository = chatChannelRepository;
        }

        public EitherAsyncUnit Execute(User user, ChatChannel chatChannel)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _2 in chatChannelRepository.Insert(chatChannel)
                from actor in AkkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in actor.TellExt(new RegisterChatChannel(chatChannel)).ToAsync()
                select _3;
        }
    }
}
