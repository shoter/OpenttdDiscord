using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.Errors;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.UseCases
{
    public class RemoveAutoReplyUseCase : IRemoveAutoReplyUseCase
    {
        private readonly IGetServerUseCase getServerUseCase;
        private readonly IGetAutoReplyUseCase getAutoReplyUseCase;
        private readonly IAutoReplyRepository autoReplyRepository;
        private readonly IAkkaService akkaService;

        public RemoveAutoReplyUseCase(
            IGetServerUseCase getServerUseCase,
            IAkkaService akkaService,
            IGetAutoReplyUseCase getAutoReplyUseCase,
            IAutoReplyRepository autoReplyRepository)
        {
            this.getServerUseCase = getServerUseCase;
            this.akkaService = akkaService;
            this.getAutoReplyUseCase = getAutoReplyUseCase;
            this.autoReplyRepository = autoReplyRepository;
        }

        public EitherAsyncUnit Execute(
            ulong guildId,
            string serverName,
            string triggerMessage)
        {
            var ret =
                from server in getServerUseCase.Execute(
                    serverName,
                    guildId)
                from autoReply in getAutoReplyUseCase.Execute(
                    guildId, server.Id, triggerMessage)
                from _1 in autoReply.ErrorWhenNone(() => AutoReplyNotFound.Instance)
                from _2 in autoReplyRepository.RemoveAutoReply(
                    guildId,
                    server.Id,
                    triggerMessage)
                from _3 in akkaService.SelectAndAsk<object>(
                    MainActors.Paths.Guilds,
                    new RemoveAutoReply(
                        guildId,
                        server.Id,
                        triggerMessage))
                select Unit.Default;
            return ret;
        }
    }
}