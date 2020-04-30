using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public interface IAdminPortClientProvider
    {
        event EventHandler<IAdminPortClient> NewClientCreated;

        Task<IAdminPortClient> GetClient(ServerInfo info);


        
    }
}
