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

            while (true)
            {
                try
                {
                    using var conn = new MySqlConnection(d.GetConnectionString());
                    await conn.OpenAsync();
                    using var cmd = new MySqlCommand("select 1", conn);

                    await cmd.ExecuteNonQueryAsync();
                    break;
                }
                catch (MySqlException e)
                {
                    if (e.ErrorCode != 1042 && e.Message != "Couldn't connect to server")
                    {
                        throw;
                    }
                }
                catch(Exception e)
                {
                    int asa = 123;
                }
            }

            {
                string workingDirectory = Environment.CurrentDirectory;
                var dir = Path.Combine(Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName, "OpenttdDiscord.Database/SQL");
                var files = Directory.GetFiles(dir);

                using var conn = new MySqlConnection(d.GetConnectionString());
                await conn.OpenAsync();

                foreach (var f in files.OrderBy(x => x))
                {
                    var path = Path.Combine(dir, f);
                    string sql = File.ReadAllText(path);

                    using var cmd = new MySqlCommand(sql, conn);

                    await cmd.ExecuteNonQueryAsync();

                }
            }
        }
    }
}
