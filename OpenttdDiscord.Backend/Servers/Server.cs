using OpenttdDiscord.Backend.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public Server(DbDataReader reader, string prefix = null)
        {
            if (prefix != null)
                prefix += ".";
            
            Id = reader.ReadU64($"{prefix}id");
            ServerIp = reader.ReadString($"{prefix}server_ip");
            ServerPort = reader.ReadInt($"{prefix}server_port");
            ServerName = reader.ReadString($"{prefix}server_name");
        }

    }
}
