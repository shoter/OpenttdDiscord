using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public interface IAdminPortClientFactory
    {
        IAdminPortClient Create(ServerInfo serverInfo);
        
    }
}
