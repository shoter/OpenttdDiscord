using System.Diagnostics.CodeAnalysis;
using Discord;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.Options;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Servers.Options;

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
            builder.WithDescription("Gets message content of given auto reply")
                .AddOption(
                    new ServerNameOption()
                        .WithRequired(true))
                .AddOption(
                    new AutoReplyTriggerOption()
                        .WithRequired(true));
        }
    }
}