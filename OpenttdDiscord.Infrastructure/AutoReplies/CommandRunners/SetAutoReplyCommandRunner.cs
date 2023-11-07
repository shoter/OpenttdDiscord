using Discord;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Modals;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners
{
    internal class SetAutoReplyCommandRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetAutoReplyUseCase getAutoReplyUseCase;
        private readonly IGetServerUseCase getServerUseCase;

        public SetAutoReplyCommandRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IGetAutoReplyUseCase getAutoReplyUseCase,
            IGetServerUseCase getServerUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.getAutoReplyUseCase = getAutoReplyUseCase;
            this.getServerUseCase = getServerUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            string action = options.GetValueAs<string>("action");
            string trigger = options.GetValueAs<string>("trigger");

            return from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from server in getServerUseCase.Execute(
                    serverName,
                    guildId)
                from autoReply in getAutoReplyUseCase.Execute(
                    guildId,
                    server.Id,
                    trigger)
                select new ModalResponse(
                    new SetAutoReplyModal(
                        serverName,
                        action,
                        trigger,
                        autoReply.Map(x => x.ResponseMessage))) as IInteractionResponse;
        }
    }
}