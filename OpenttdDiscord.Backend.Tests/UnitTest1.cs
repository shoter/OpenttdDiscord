using MySql.Data.MySqlClient;
using OpenttdDiscord.Testing.Database;
using System;
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

            try
            {
                using var conn = new MySqlConnection(d.GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new MySqlCommand("select 1", conn);

                await cmd.ExecuteNonQueryAsync();
            }
            catch(Exception e )
            {
                int a = 123;
            }



        }
    }
}
