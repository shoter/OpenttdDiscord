using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Chatting.UseCases
{
    internal class GetChatChannelUseCase : UseCaseBase, IGetChatChannelUseCase
    {
        private readonly IChatChannelRepository chatChannelRepository;

        public GetChatChannelUseCase(IChatChannelRepository chatChannelRepository)
        {
            this.chatChannelRepository = chatChannelRepository;
        }

        public EitherAsync<IError, Option<ChatChannel>> Execute(User user, Guid serverId, ulong channelId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from server in chatChannelRepository.GetChatChannelsForServer(serverId, channelId)
                select server;
        }
    }
}
