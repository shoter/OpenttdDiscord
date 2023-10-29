using Discord;
using OpenttdDiscord.Infrastructure.AutoReply.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.AutoReply.Commands
{
    internal class SetWelcomeMessageCommand : OttdSlashCommandBase<SetWelcomeMessageRunner>
    {
        public SetWelcomeMessageCommand(string name)
            : base(name)
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Used to set welcome message for players joining Ottd server");
        }
    }
}