using Discord;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Commands
{
    internal class SetAutoReplyCommand : OttdSlashCommandBase<SetAutoReplyCommandRunner>
    {
        public SetAutoReplyCommand()
            : base("set-auto-reply")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Autoreply lets server automatically reply and execute action upon user message")
                .AddOption(
                    new SlashCommandOptionBuilder()
                        .WithName("server-name")
                        .WithDescription("Server name")
                        .WithType(ApplicationCommandOptionType.String)
                        .WithRequired(true))
                .AddOption(
                    new SlashCommandOptionBuilder()
                        .WithName("additional-action")
                        .WithDescription("Additional action which is going to be executed upon sending response to the player.")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer)
                        .AddChoice(
                            "None",
                            (int) AutoReplyAction.None)
                        .AddChoice(
                            "Reset company",
                            (int) AutoReplyAction.ResetCompany));
        }
    }
}
