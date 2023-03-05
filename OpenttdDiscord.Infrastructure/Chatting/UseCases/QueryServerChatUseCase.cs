using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.UseCases
{
    internal class QueryServerChatUseCase : UseCaseBase, IQueryServerChatUseCase
    {
        private readonly IAkkaService akkaService;

        public QueryServerChatUseCase(IAkkaService akkaService)
        {
            this.akkaService = akkaService;
        }

        public EitherAsync<IError, IReadOnlyList<string>> Execute(User user, Guid serverId, ulong guildId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from response in actor.TryAsk<RetrievedChatMessages>(new RetrieveChatMessages(serverId, guildId))
                select response.Messages;
        }
    }
}
