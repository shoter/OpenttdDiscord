using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Servers
{
    public class Server
    {
        public ulong Id { get; }
        public string ServerIp { get; }
        public string ServerName { get; }
        public int ServerPort { get; }

        public Server(ulong id, string serverIp, int serverPort, string serverName)
        {
            this.Id = id;
            this.ServerIp = serverIp;
            this.ServerPort = serverPort;
            this.ServerName = serverName;
        }

    }
}
