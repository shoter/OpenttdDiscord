using System.Diagnostics.CodeAnalysis;
using Discord;
using OpenttdDiscord.Infrastructure.AutoReplies.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.Commands;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Commands
{
    [ExcludeFromCodeCoverage]
    internal class GetAutoRepliesCommand : OttdSlashCommandBase<GetAutoRepliesCommandRunner>
    {
        public GetAutoRepliesCommand()
        : base("get-auto-replies")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder
                .WithDescription("Gets all auto replies trigger messages for the server")
                .AddOption(
                    new SlashCommandOptionBuilder()
                        .WithName("server-name")
                        .WithDescription("Name of the server")
                        .WithRequired(true));
        }
    }
}
