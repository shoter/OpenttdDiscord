using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Testing;
using OpenttdDiscord.Testing.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Reporting
{
    public class ReportServerRepositoryTests : MysqlTest
    {
        public ReportServerRepositoryTests() : base(new ContainerizedMysqlDatabase(), nameof(ReportServerRepositoryTests))
        { }

        [Fact]
        public async Task Add_ShouldAddServer()
        {
            IReportServerRepository repo = new ReportServerRepository(GetMysql());

            var report = await repo.Add(DefaultTestData.DefaultServer, 11u);
            var secondReport = await repo.GetAll(11u);

            secondReport.Single(x => x.ChannelId == 11u && x.Server.Id == report.Server.Id);
        }

        [Fact]
        public async Task GetAllForGuild_ShouldGetServersForTheSameGuild_EvenIfOnDifferentChannels()
        {
            IReportServerRepository repo = new ReportServerRepository(GetMysql());

            List<ReportServer> repoServers = new List<ReportServer>();

            repoServers.Add(await repo.Add(DefaultTestData.SameGuildServers.ElementAt(0), 11u));
            repoServers.Add(await repo.Add(DefaultTestData.SameGuildServers.ElementAt(1), 11u));
            repoServers.Add(await repo.Add(DefaultTestData.SameGuildServers.ElementAt(2), 22u));

            ReportServer differentGuildRepo = await repo.Add(DefaultTestData.DefaultServer, 11u);

            List<ReportServer> fromRepo = await repo.GetAllForGuild(DefaultTestData.SameGuildServers.First().GuildId);

            foreach(var r in repoServers)
            {
                fromRepo.Single(x => x.ChannelId == r.ChannelId && x.Server.Id == r.Server.Id);
            }

            Assert.DoesNotContain(fromRepo, x => x.ChannelId == differentGuildRepo.ChannelId && x.Server.Id == differentGuildRepo.Server.Id);
        }

        [Fact]
        public async Task Remove_ShouldRemoveServer()
        {
            IReportServerRepository repo = new ReportServerRepository(GetMysql());

            var report = await repo.Add(DefaultTestData.DefaultServer, 11u);
            await repo.Remove(report);
            var secondReport = await repo.GetAll(11u);

            Assert.DoesNotContain(secondReport, x => x.ChannelId == 11u && x.Server.Id == report.Server.Id);
        }

    }
}
