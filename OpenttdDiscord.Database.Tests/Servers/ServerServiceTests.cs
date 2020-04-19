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
    public class ServerServiceTests : MysqlTest
    {
        private readonly ServerServiceFixture fix = new ServerServiceFixture();
        private readonly ServerFixture serverFix = new ServerFixture();

        public ServerServiceTests() : base(new ContainerizedMysqlDatabase(), nameof(ServerServiceTests))
        {
        }

        [Fact]
        public async Task Getsert_InsertsServerAtFirstRun()
        {
            ServerService service = fix.WithServerRepository(GetMysql());
            Server server = serverFix;

            await service.Getsert(server.ServerIp, server.ServerPort, server.ServerName);
            Server result = await service.Get(server.ServerIp, server.ServerPort);

            Assert.Equal(server.ServerIp, result.ServerIp);
            Assert.Equal(server.ServerName, result.ServerName);
            Assert.Equal(server.ServerPort, result.ServerPort);
        }

        [Fact]
        public async Task Getsert_GetsDataFromServerOnSecondRun()
        {
            ServerService service = fix.WithServerRepository(GetMysql());
            Server server = serverFix;

            await service.Getsert(server.ServerIp, server.ServerPort, server.ServerName);
            Server result = await service.Get(server.ServerIp, server.ServerPort);
            Server getsert = await service.Getsert(server.ServerIp, server.ServerPort, server.ServerName);

            Assert.Equal(result.Id, getsert.Id);
        }


    }
}
