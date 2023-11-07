using Discord;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Modals;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners
{
    internal class SetAutoReplyCommandRunner : OttdSlashCommandRunnerBase
    {
        public SetAutoReplyCommandRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
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
                from guild in EnsureItIsGuildCommand(command)
                    .ToAsync()
                select new ModalResponse(
                        new SetAutoReplyModal(
                            welcomeMessage.Map(x => x.Content),
                            serverName)) as
                    IInteractionResponse;        }
    }
}