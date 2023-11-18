using System.Diagnostics.CodeAnalysis;
using Discord;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Commands
{
    [ExcludeFromCodeCoverage]
    internal class GetAutoReplyContentCommand : OttdSlashCommandBase<GetAutoReplyContentCommandRunner>
    {
        public GetAutoReplyContentCommand()
            : base("get-auto-reply-content")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
