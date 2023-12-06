using Discord;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Options;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Servers.Options;

namespace OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners
{
    internal class RemoveAutoReplyCommandRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRemoveAutoReplyUseCase removeAutoReplyUseCase;

        public RemoveAutoReplyCommandRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IRemoveAutoReplyUseCase removeAutoReplyUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.removeAutoReplyUseCase = removeAutoReplyUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            OptionsDictionary options)
        {
            var serverName = ServerNameOption.GetValue(options);
            var trigger = AutoReplyTriggerOption.GetValue(options);

            return
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Admin)
                    .ToAsync()
                from _2 in removeAutoReplyUseCase.Execute(
                    guildId,
                    serverName,
                    trigger)
                select new TextResponse("Auto reply has been removed") as IInteractionResponse;
        }
    }
}