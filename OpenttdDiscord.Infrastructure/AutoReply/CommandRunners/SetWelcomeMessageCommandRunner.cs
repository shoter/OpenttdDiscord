using Discord;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReply.Modals;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Testing.Modals;

namespace OpenttdDiscord.Infrastructure.AutoReply.CommandRunners
{
    internal class SetWelcomeMessageCommandRunner : OttdSlashCommandRunnerBase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        private readonly IAutoReplyRepository autoReplyRepository;

        public SetWelcomeMessageCommandRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IOttdServerRepository ottdServerRepository,
            IAutoReplyRepository autoReplyRepository)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.ottdServerRepository = ottdServerRepository;
            this.autoReplyRepository = autoReplyRepository;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");

            return
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from server in ottdServerRepository.GetServerByName(
                    guildId,
                    serverName)
                from welcomeMessage in autoReplyRepository.GetWelcomeMessage(
                    guildId,
                    server.Id)
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                select new ModalResponse(
                        new SetWelcomeMessageModal(
                            welcomeMessage.Map(x => x.Content),
                            serverName)) as
                    IInteractionResponse;
        }
    }
}