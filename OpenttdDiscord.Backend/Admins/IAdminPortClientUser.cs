using OpenTTDAdminPort.Events;
using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Backend.Admins
{
    public interface IAdminPortClientUser
    {
        void ParseServerEvent(Server server, IAdminEvent adminEvent);
    }
}
