using OpenttdDiscord.Infrastructure.AutoReplies.Modals;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.AutoReplies.ModalRunners
{
    public class AutoReplyAssociatedModalRunners : IAssociatedModalRunners
    {
        public Dictionary<string, Type> AssociatedModalRunners { get; } = new()
        {
            { AutoReplyModals.SetWelcomeMessage, typeof(SetWelcomeMessageModalRunner) },
        };
    }
}