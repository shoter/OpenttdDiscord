using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.EventLogs.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.EventLogs.Messages;

namespace OpenttdDiscord.Infrastructure.EventLogs.UseCases
{
    internal class QueryEventLogUseCase : UseCaseBase, IQueryEventLogUseCase
    {
        private readonly IAkkaService akkaService;

        public QueryEventLogUseCase(IAkkaService akkaService)
        {
            this.akkaService = akkaService;
        }

        public EitherAsync<IError, IReadOnlyList<string>> Execute(User user, Guid serverId, ulong guildId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from response in actor.TryAsk<RetrievedEventLog>(new RetrieveEventLog(serverId, guildId))
                select response.Messages;
        }
    }
}
