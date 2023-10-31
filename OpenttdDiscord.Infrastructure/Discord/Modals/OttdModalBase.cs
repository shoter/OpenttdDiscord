using System.Diagnostics.CodeAnalysis;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Discord.Modals
{
    [ExcludeFromCodeCoverage]
    public abstract class OttdModalBase : IOttdModal
    {
        public string Id { get; }

        public OttdModalBase(string id)
        {
            this.Id = id;
        }

        public Modal Build()
        {
            ModalBuilder builder = new();
            builder.WithCustomId(Id);
            Configure(builder);
            return builder.Build();
        }

        protected abstract void Configure(ModalBuilder builder);
    }
}