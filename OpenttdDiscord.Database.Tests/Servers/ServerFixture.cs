﻿using System;
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
        private readonly Random rand = new Random();
        private byte lastIp = 0;
        private ulong id = 0;
        private string ip;
        private string serverName;
        private int serverPort = 1;
        private ulong guildId = 11u;
        private string password = "";

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

        public ServerFixture BasedOn(Server server)
        {
            this.id = server.Id;
            this.serverPort = server.ServerPort;
            this.password = server.ServerPassword;
            this.ip = server.ServerIp;
            this.guildId = server.GuildId;
            return this;
        }

        public ServerFixture WithPassword(string password)
        {
            this.password = password;
            return this;
        }

        public ServerFixture WithPort(int port)
        {
            this.serverPort = port;
            return this;
        }

        public ServerFixture WithGuild(ulong guild)
        {
            this.guildId = guild;
            return this;
        }

        public ServerFixture WithRandomGuild()
        {
            this.guildId = (ulong)rand.Next(0, int.MaxValue);
            return this;
        }

        public Server Build()
        {
            var s = new Server(
                id++,
                guildId,
                ip,
                serverPort++,
                serverName,
                password
                );
            ip = NewIp();
            serverName = Guid.NewGuid().ToString();

            return s;
        }

        private string NewIp() => $"192.168.0.{lastIp++}";

        public static implicit operator Server(ServerFixture fix) => fix.Build();

        
    }
}
