using System.Text;
using Discord;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners
{
    internal class GetAutoRepliesCommandRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetAutoReplyUseCase getAutoReplyUseCase;
        private readonly IGetServerUseCase getServerUseCase;

        public const string NoRepliesResponse = "No auto-replies defined for this server";

        public GetAutoRepliesCommandRunner(
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
            OptionsDictionary options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            return
                from guildId in EnsureItIsGuildCommand(command)
                    .ToAsync()
                from server in getServerUseCase.Execute(
                    serverName,
                    guildId)
                from autoReplies in getAutoReplyUseCase.Execute(
                    guildId,
                    server.Id)
                select CreateResponse(autoReplies);
        }

        private IInteractionResponse CreateResponse(IReadOnlyCollection<AutoReply> autoReplies)
        {
            if (autoReplies.Count == 0)
            {
                return new TextResponse(NoRepliesResponse);
            }

            StringBuilder sb = new();
            sb.Append("Auto replies defined for this server:");

            foreach (var ar in autoReplies)
            {
                sb.AppendLine();
                sb.Append(ar.TriggerMessage);
                sb.Append(" - ");
                sb.Append(ar.AdditionalAction.ToString());
            }

            return new TextResponse(sb);
        }
    }
}