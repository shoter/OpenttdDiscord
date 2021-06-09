using OpenTTDAdminPort;

namespace OpenttdDiscord.Backend.Admins
{
    public interface IAdminPortClientFactory
    {
        IAdminPortClient Create(ServerInfo serverInfo);
    }
}
