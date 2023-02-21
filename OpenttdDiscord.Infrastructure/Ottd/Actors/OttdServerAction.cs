using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
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
