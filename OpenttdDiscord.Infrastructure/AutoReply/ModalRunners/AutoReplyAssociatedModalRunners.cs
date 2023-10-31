using OpenttdDiscord.Infrastructure.AutoReply.Modals;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.AutoReply.ModalRunners
{
    public class AutoReplyAssociatedModalRunners : IAssociatedModalRunners
    {
        public Dictionary<string, Type> AssociatedModalRunners { get; } = new()
        {
            { AutoReplyModals.SetWelcomeMessageModals, typeof(SetWelcomeMessageModalRunner) },
        };
    }
}