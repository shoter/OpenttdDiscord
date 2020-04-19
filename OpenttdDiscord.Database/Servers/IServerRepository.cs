using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public interface IServerRepository
    {
        Task<Server> GetServer(string ip, int port);
        Task<Server> AddServer(string ip, int port, string name);

        Task<Server> GetServer(string serverName);
        Task<List<Server>> GetAll();
        
    }
}
