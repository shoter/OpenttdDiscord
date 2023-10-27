using Discord;
using OpenttdDiscord.Infrastructure.Discord.Commands;
using OpenttdDiscord.Infrastructure.Testing.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Testing.Commands
{
    internal class TestCommand : OttdSlashCommandBase<TestCommandRunner>
    {
        public TestCommand()
            : base("ottd-test-command")
        {
        }

        protected override void Configure(SlashCommandBuilder builder)
        {
            builder.WithDescription("Test command used by admin. DO not touch :)");
        }
    }
}