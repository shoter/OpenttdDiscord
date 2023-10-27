using Discord;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Discord.Modals
{
    public interface IOttdModal
    {
        string Id { get; }
        Modal Build();
        IOttdModalRunner CreateRunner(IServiceProvider sp);
    }
}