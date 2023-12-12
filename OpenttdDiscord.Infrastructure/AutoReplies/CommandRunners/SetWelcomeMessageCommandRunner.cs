using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Modals;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners
{
    internal class SetWelcomeMessageCommandRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        private readonly IAutoReplyRepository autoReplyRepository;

        public SetWelcomeMessageCommandRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IAutoReplyRepository autoReplyRepository,
            IGetServerUseCase getServerUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.autoReplyRepository = autoReplyRepository;
            this.getServerUseCase = getServerUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            return
                from _0 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from server in getServerUseCase.Execute(
                    serverName,
                    guildId)
                from welcomeMessage in autoReplyRepository.GetWelcomeMessage(
                    guildId,
                    server.Id)
                select new ModalResponse(
                        new SetWelcomeMessageModal(
                            welcomeMessage.Map(x => x.Content),
                            serverName)) as
                    IInteractionResponse;
        }
    }
}