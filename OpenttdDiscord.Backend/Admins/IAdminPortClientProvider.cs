using OpenTTDAdminPort;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Openttd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Admins
{
    public interface IAdminPortClientProvider
    {
        Task Register(IAdminPortClientUser owner, Server server);

        IAdminPortClient GetClient(IAdminPortClientUser owner, Server server);

        Task Unregister(IAdminPortClientUser owner, Server server);

        bool IsRegistered(IAdminPortClientUser owner, Server server);
        
    }
}
