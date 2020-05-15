using OpenttdDiscord.Database.Extensions;
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
        public string ServerPassword { get; }
        public int ServerPort { get; }

        public ulong GuildId { get; }

        public Server(ulong id, ulong guildId, string serverIp, int serverPort, string serverName, string password = "")
        {
            this.Id = id;
            this.ServerIp = serverIp;
            this.ServerPort = serverPort;
            this.ServerName = serverName;
            this.GuildId = guildId;
            this.ServerPassword = password;
        }

        public Server(DbDataReader reader, string prefix = null)
        {
            Id = reader.ReadU64("id", prefix);
            ServerIp = reader.ReadString("server_ip", prefix);
            ServerPort = reader.ReadInt("server_port", prefix);
            ServerName = reader.ReadString("server_name", prefix);
            ServerPassword = reader.ReadString("server_password", prefix);
            GuildId = reader.ReadU64("guild_id", prefix);
        }

        public string GetUniqueKey() => $"{Id}-{ServerIp}-{ServerPort}";

    }
}
