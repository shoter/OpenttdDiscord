using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.AdminPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Admins
{
    public interface IAdminPortClientProvider
    {
        Task Register(object owner, Server server);

        IAdminPortClient GetClient(object owner, Server server);

        Task Unregister(object owner, Server server);

        bool IsRegistered(object owner, Server server);
        
    }
}
