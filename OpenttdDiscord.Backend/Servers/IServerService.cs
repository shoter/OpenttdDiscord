using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Servers
{
    public interface IServerService
    {
        event EventHandler<Server> Added;
        Task<Server> Getsert(string ip, int port, string serverName);
        Task<bool> Exists(string ip, int port);
        Task<Server> Get(string ip, int port);

        Task<List<Server>> GetAll();
        
    }
}
