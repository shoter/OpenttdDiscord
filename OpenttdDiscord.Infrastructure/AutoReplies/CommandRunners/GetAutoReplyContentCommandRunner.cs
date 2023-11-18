using Discord;
using LanguageExt.SomeHelp;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.Errors;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.AutoReplies.Options;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Servers.Options;

namespace OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners
{
    internal class GetAutoReplyContentCommandRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;
        private readonly IGetAutoReplyUseCase getAutoReplyUseCase;

        public GetAutoReplyContentCommandRunner(
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase,
            IGetServerUseCase getServerUseCase,
            IGetAutoReplyUseCase getAutoReplyUseCase)
            : base(
                akkaService,
                getRoleLevelUseCase)
        {
            this.getServerUseCase = getServerUseCase;
            this.getAutoReplyUseCase = getAutoReplyUseCase;
        }

        protected override EitherAsync<IError, IInteractionResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            var serverName = options.GetValueAs<string>(ServerNameOption.OptionName);
            var trigger = options.GetValueAs<string>(AutoReplyTriggerOption.OptionName);

            return
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from server in getServerUseCase.Execute(
                    serverName,
                    guildId)
                from autoReplyOption in getAutoReplyUseCase.Execute(
                    guildId,
                    server.Id,
                    trigger)
                from response in GenerateResponse(autoReplyOption)
                select response;
        }

        private EitherAsync<IError, IInteractionResponse> GenerateResponse(Option<AutoReply> autoReplyOption)
        {
            if (autoReplyOption.IsNone)
            {
                return new AutoReplyNotFound();
            }

            var autoReply = (AutoReply) autoReplyOption.Case!;
            return new TextResponse(autoReply.ResponseMessage);
        }
    }
}