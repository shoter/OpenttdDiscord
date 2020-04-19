﻿using OpenttdDiscord.Database.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
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
            Id = reader.ReadU64("id", prefix);
            ServerIp = reader.ReadString("server_ip", prefix);
            ServerPort = reader.ReadInt("server_port", prefix);
            ServerName = reader.ReadString("server_name", prefix);
        }

    }
}