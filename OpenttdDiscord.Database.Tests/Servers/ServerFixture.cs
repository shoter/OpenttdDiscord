using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class ServerFixture
    {
        private byte lastIp = 0;

        private ulong id = 0;
        private string ip;
        private string serverName;
        private int serverPort = 1;
        private ulong guildId = 11u;

        public ServerFixture()
        {
            this.ip = NewIp();
            this.serverName = Guid.NewGuid().ToString();
            }

        public ServerFixture WithServerName(string serverName)
        {
            this.serverName = serverName;
            return this;
        }

        public ServerFixture WithPort(int port)
        {
            this.serverPort = port;
            return this;
        }

        public Server Build()
        {
            var s = new Server(
                id++,
                guildId,
                ip,
                serverPort++,
                serverName
                );
            ip = NewIp();
            serverName = Guid.NewGuid().ToString();

            return s;
        }

        private string NewIp() => $"192.168.0.{lastIp++}";

        public static implicit operator Server(ServerFixture fix) => fix.Build();

        
    }
}
