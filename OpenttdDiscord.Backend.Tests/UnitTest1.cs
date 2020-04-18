using MySql.Data.MySqlClient;
using OpenttdDiscord.Testing.Database;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var d = new ContainerizedMysqlDatabase();
            await d.Start("test-container");


        }
    }
}
