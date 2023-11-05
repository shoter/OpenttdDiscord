using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.AutoReplies.Actors;
using OpenttdDiscord.Infrastructure.Ottd.Actors;

namespace OpenttdDiscord.Infrastructure.Ottd.Actions
{
    /// <summary>
    /// Action is a temporary actor created as a child of <see cref="AutoReplies.Actors.GuildServerActor"/>.
    /// Its role is to execute single operation (aka. action) and terminate afterwards.
    /// </summary>
    /// <typeparam name="TCommand">Type of message that this actor is going to receive jsut after it has been created.</typeparam>
    public abstract class OttdServerAction<TCommand> : ReceiveActorBase
    {
        protected readonly IAdminPortClient client;
        protected readonly OttdServer server;

        protected OttdServerAction(
            IServiceProvider serviceProvider,
            OttdServer server,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.client = client;
            this.server = server;

            Ready();
        }

        protected virtual void Ready()
        {
            ReceiveAsync<TCommand>(HandleCommand);
        }

        protected abstract Task HandleCommand(TCommand command);
    }
}
