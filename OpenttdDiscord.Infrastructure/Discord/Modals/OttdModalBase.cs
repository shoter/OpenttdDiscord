using Discord;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Discord.Modals
{
    public abstract class OttdModalBase<TModalRunner> : IOttdModal
        where TModalRunner : IOttdModalRunner
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

        public IOttdModalRunner CreateRunner(IServiceProvider sp)
            => sp.GetRequiredService<TModalRunner>();

        protected abstract void Configure(ModalBuilder builder);
    }
}