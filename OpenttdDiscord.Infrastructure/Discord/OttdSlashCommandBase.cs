using Discord;
using Microsoft.Extensions.DependencyInjection;

namespace OpenttdDiscord.Infrastructure.Discord
{
    internal abstract class OttdSlashCommandBase<TRunner> : IOttdSlashCommand
        where TRunner: IOttdSlashCommandRunner
    {
        public string Name { get; }

        public OttdSlashCommandBase(string name)
        {
            this.Name = name;
        }

        public  SlashCommandProperties Build()
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
