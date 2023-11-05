using Discord;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Commands
{
    internal class SetWelcomeMessageCommand : OttdSlashCommandBase<SetWelcomeMessageCommandRunner>
    {
        public SetWelcomeMessageCommand()
            : base("set-welcome-message")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Used to set welcome message for players joining Ottd server")
                .AddOption(
                    new SlashCommandOptionBuilder()
                        .WithName("server-name")
                        .WithRequired(true)
                        .WithDescription("Name of the server")
                        .WithType(ApplicationCommandOptionType.String));
        }
    }
}