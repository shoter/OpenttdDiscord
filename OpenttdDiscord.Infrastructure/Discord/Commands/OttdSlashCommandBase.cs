using Discord;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Discord.Commands
{
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

        public abstract void Configure(SlashCommandBuilder builder);

        public IOttdSlashCommandRunner CreateRunner(IServiceProvider sp)
            => sp.GetRequiredService<TRunner>();
    }
}
