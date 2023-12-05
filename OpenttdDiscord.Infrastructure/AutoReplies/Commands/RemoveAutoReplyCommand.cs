using Discord;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.AutoReplies.Options;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Servers.Options;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Commands
{
    internal class RemoveAutoReplyCommand : OttdSlashCommandBase<RemoveAutoReplyCommandRunner>
    {
        public RemoveAutoReplyCommand()
            : base("remove-auto-reply")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Removes auto reply from given ottd server")
                .AddOption(
                    new ServerNameOption()
                        .WithRequired(true))
                .AddOption(
                    new AutoReplyTriggerOption()
                        .WithRequired(true));
        }
    }
}