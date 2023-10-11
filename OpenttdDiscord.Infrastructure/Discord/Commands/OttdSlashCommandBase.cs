using System.Diagnostics.CodeAnalysis;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Discord.Commands
{
    // This class is excluded from code coverage due to it's very declarative nature.
    // There is no build logic here all of the classes that are deriving from this abstract class.
    [ExcludeFromCodeCoverage]
    internal abstract class OttdSlashCommandBase<TRunner> : IOttdSlashCommand
        where TRunner : IOttdSlashCommandRunner
    {
        public string Name { get; }

        public OttdSlashCommandBase(string name)
        {
            Name = name;
        }

        public SlashCommandProperties Build()
        {
            var builder = new SlashCommandBuilder();
            builder.WithName(Name);
            Configure(builder);
            return builder.Build();
        }

        protected abstract void Configure(SlashCommandBuilder builder);

        public IOttdSlashCommandRunner CreateRunner(IServiceProvider sp)
            => sp.GetRequiredService<TRunner>();
    }
}
