using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.AutoReplies.ModalRunners
{
    internal class SetWelcomeMessageModalRunner : OttdModalRunnerBase
    {
        private readonly IUpsertWelcomeMessageUseCase upsertWelcomeMessageUseCase;

        private readonly IGetServerUseCase getServerUseCase;

        public SetWelcomeMessageModalRunner(
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IUpsertWelcomeMessageUseCase upsertWelcomeMessageUseCase,
            IGetServerUseCase getServerUseCase)
            : base(getRoleLevelUseCase)
        {
            this.upsertWelcomeMessageUseCase = upsertWelcomeMessageUseCase;
            this.getServerUseCase = getServerUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            IModalInteraction modal,
            Dictionary<string, IComponentInteractionData> components,
            User user)
        {
            string serverName = components["server-name"].Value;
            string content = components["content"].Value;
            bool isDeletion = string.IsNullOrWhiteSpace(content);

            return
                from guildId in EnsureItIsGuildModal(modal)
                    .ToAsync()
                from server in getServerUseCase.Execute(
                    serverName,
                    guildId)
                from response in isDeletion
                    ? Delete(
                        guildId,
                        server)
                    : Upsert(
                        guildId,
                        server,
                        content)
                select response;
        }

        private EitherAsync<IError, IInteractionResponse> Upsert(
            ulong guildId,
            OttdServer server,
            string content) => from _1 in upsertWelcomeMessageUseCase.Execute(
                guildId,
                server.Id,
                content)
            select new TextResponse($"Welcome message for {server.Name} was updated!") as IInteractionResponse;

        private EitherAsync<IError, IInteractionResponse> Delete(
            ulong guildId,
            OttdServer server) => new TextResponse("Deletion is not implemneted yet");
    }
}