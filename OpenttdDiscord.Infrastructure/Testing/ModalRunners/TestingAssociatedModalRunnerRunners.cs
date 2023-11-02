using OpenttdDiscord.Infrastructure.Discord.ModalRunners;
using OpenttdDiscord.Infrastructure.Testing.Modals;

namespace OpenttdDiscord.Infrastructure.Testing.ModalRunners
{
    public class TestingAssociatedModalRunnerRunners : IAssociatedModalRunners
    {
        public Dictionary<string, Type> AssociatedModalRunners { get; } = new()
        {
            { TestModals.TestModal, typeof(TestModalRunner) },
        };
    }
}
