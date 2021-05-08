using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord
{
    public class ServerEvent 
    {

        public ServerEvent(Server server, IAdminEvent ev)
        {
            this.AdminEvent = ev;
            this.Server = server;
        }

        public Server Server { get; set; }

        public IAdminEvent AdminEvent { get; set; }
        
    }
}
