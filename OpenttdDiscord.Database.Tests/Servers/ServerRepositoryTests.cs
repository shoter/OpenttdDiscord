using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Testing;
using OpenttdDiscord.Testing.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class ServerRepositoryTests : MysqlTest
    {
        public ServerRepositoryTests() : base(new ContainerizedMysqlDatabase(), nameof(ServerRepositoryTests))
        {
        }

        [Fact]
        public async Task InsertTest_ShouldBeAbleToGetItAfterThat()
        {
            IServerRepository repo = new ServerRepository(GetMysql());
            await repo.AddServer(11u, "192.168.0.1", 123, "testServerName");
            var server = await repo.GetServer(11u, "192.168.0.1", 123);

            Assert.Equal("192.168.0.1", server.ServerIp);
            Assert.Equal(11u, server.GuildId);
            Assert.Equal(123, server.ServerPort);
            Assert.Equal("testServerName", server.ServerName);
        }

        [Fact]
        public async Task GetServer_ShouldBeAbleToGetServerByName()
        {
            IServerRepository repo = new ServerRepository(GetMysql());
            await repo.AddServer(11u, "192.168.0.1", 123, "testServerName");
            var server = await repo.GetServer(11u, "testServerName");

            Assert.Equal("192.168.0.1", server.ServerIp);
            Assert.Equal(123, server.ServerPort);
            Assert.Equal(11u, server.GuildId);
            Assert.Equal("testServerName", server.ServerName);
        }

        [Fact]
        public async Task GetServer_ShouldBeAbleToGetServerById()
        {
            IServerRepository repo = new ServerRepository(GetMysql());
            Server s = await repo.AddServer(11u, "192.168.0.1", 123, "testServerName");
            var server = await repo.GetServer(s.Id);

            Assert.Equal("192.168.0.1", server.ServerIp);
            Assert.Equal(123, server.ServerPort);
            Assert.Equal(11u, server.GuildId);
            Assert.Equal("testServerName", server.ServerName);
        }

        [Fact]
        public async Task UpdatePassword_ShouldChangePassword()
        {
            IServerRepository repo = new ServerRepository(GetMysql());
            var server = await repo.AddServer(11u, "192.168.0.1", 123, "testServerName");
            await repo.UpdatePassword(server.Id, "pwd");
            server = await repo.GetServer(11u, server.ServerName);

            Assert.Equal("pwd", server.ServerPassword);
        }

        [Fact]
        public async Task UpdatePassword_ShouldThrowException_ForNonExistingServer()
        {
            IServerRepository repo = new ServerRepository(GetMysql());
            await Assert.ThrowsAsync<OttdException>(() => repo.UpdatePassword(69, "pwd"));
        }

        [Fact]
        public async Task GetAllShouldGetAllServers()
        {
            Server[] servers = new Server[]
            {
                new Server(0,11u,  "127.0.0.1", 123, "test"),
                new Server(0,11u,  "24.1.2.1", 123, "test2"),
                new Server(0,11u,  "17.1.0.1", 22, "test3"),
                new Server(0,11u,  "12.0.2.1", 33, "wara4"),
                new Server(0,11u, "12.0.2.1", 11, "wara"),
            };
            IServerRepository repo = new ServerRepository(GetMysql());
            foreach (var s in servers)
                await repo.AddServer(s.GuildId, s.ServerIp, s.ServerPort, s.ServerName);

            var response = await repo.GetAll(11u);

            foreach(var s in servers)
            {
                Assert.NotNull(response.Single(x => x.ServerIp == s.ServerIp && x.ServerName == s.ServerName && x.ServerPort == s.ServerPort));
            }
        }

        [Fact]
        public async Task GetAllShouldGetAllServersFromGivenGuild()
        {
            Server[] servers = new Server[]
            {
                new Server(0,11u,  "127.0.0.1", 123, "test"),
                new Server(0,11u,  "24.1.2.1", 123, "test2"),
                new Server(0,11u,  "17.1.0.1", 22, "test3"),
                new Server(0,11u,  "12.0.2.1", 33, "wara4"),
                new Server(0,11u, "12.0.2.1", 11, "wara"),
            };
            IServerRepository repo = new ServerRepository(GetMysql());
            foreach (var s in servers)
                await repo.AddServer(s.GuildId, s.ServerIp, s.ServerPort, s.ServerName);

            await repo.AddServer(12u, "10.0.0.1", 123, "different guild");
            await repo.AddServer(13u, "10.0.0.1", 123, "different guild");


            var response = await repo.GetAll(11u);

            foreach (var s in servers)
            {
                Assert.NotNull(response.Single(x => x.ServerIp == s.ServerIp && x.ServerName == s.ServerName && x.ServerPort == s.ServerPort));
            }
        }

        [Fact]
        public async Task Insert_ShouldBeAbleToInsertSameServersToDifferentGuilds()
        {
            IServerRepository repo = new ServerRepository(GetMysql());

            // this should not trigger any exception - smoke test
            await repo.AddServer(12u, "10.0.0.1", 123, "same");
            await repo.AddServer(13u, "10.0.0.1", 123, "same");
        }

        [Fact]
        public async Task GetServers_ShouldBeAbletToGetMultipleServersForSingleIpPortAddress()
        {
            IServerRepository repo = new ServerRepository(GetMysql());

            List<Server> added = new List<Server>();

            added.Add(await repo.AddServer(12u, "10.0.0.1", 123, "same"));
            added.Add(await repo.AddServer(13u, "10.0.0.1", 123, "same"));

            var resp = await repo.GetServers("10.0.0.1", 123);
            foreach(var s in added)
            {
                resp.Single(x => x.Id == s.Id);
            }

        }

    }
}
