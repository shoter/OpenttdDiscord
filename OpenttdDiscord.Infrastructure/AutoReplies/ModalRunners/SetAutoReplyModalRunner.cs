using Discord;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.AutoReplies.ModalRunners
{
    public class SetAutoReplyModalRunner : OttdModalRunnerBase
    {
        private readonly IUpsertAutoReplyUseCase upsertAutoReplyUseCase;
        private readonly IGetServerUseCase getServerUseCase;

        public SetAutoReplyModalRunner(
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IUpsertAutoReplyUseCase upsertAutoReplyUseCase,
            IGetServerUseCase getServerUseCase)
            : base(getRoleLevelUseCase)
        {
            this.upsertAutoReplyUseCase = upsertAutoReplyUseCase;
            this.getServerUseCase = getServerUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            IModalInteraction modal,
            Dictionary<string, IComponentInteractionData> components,
            User user)
        {
            string serverName = components["server-name"].Value;
            string triggerMessage = components["trigger"].Value;
            var action = Enum.Parse<AutoReplyAction>(
                components["action"].Value,
                ignoreCase: true);
            string content = components["content"].Value;
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from guildId in EnsureItIsGuildModal(modal)
                    .ToAsync()
                from server in getServerUseCase.Execute(
                    serverName,
                    guildId)
                from _4 in upsertAutoReplyUseCase.Execute(
                    guildId,
                    server.Id,
                    new AutoReply(
                        triggerMessage,
                        content,
                        action))
                select new TextResponse("Auto reply set!") as IInteractionResponse;
        }
    }
}