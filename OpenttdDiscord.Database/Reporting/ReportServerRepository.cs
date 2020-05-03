using MySql.Data.MySqlClient;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Reporting
{
    public class ReportServerRepository : IReportServerRepository
    {
        private readonly string connectionString;

        public ReportServerRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<ReportServer> Add(Server server, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"INSERT INTO report_servers(server_id, channel_id) 
                                                     VALUES (@sid, @cid)", conn))
                {
                    cmd.Parameters.AddWithValue("sid", server.Id);
                    cmd.Parameters.AddWithValue("cid", channelId);


                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand($@"SELECT * FROM report_servers r
                                                    JOIN servers s on r.server_id = s.id
                                                    WHERE s.id = @sid AND r.channel_id = @cid", conn))
                {
                    cmd.Parameters.AddWithValue("sid", server.Id);
                    cmd.Parameters.AddWithValue("cid", channelId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        return new ReportServer(reader);
                    }
                }
            }
        }

        public async Task<List<ReportServer>> GetAll(ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM report_servers r
                                                    JOIN servers s on r.server_id = s.id
                                                    WHERE r.channel_id = @cid", conn))
                {
                    cmd.Parameters.AddWithValue("cid", channelId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ReportServer> servers = new List<ReportServer>();
                        while (await reader.ReadAsync())
                            servers.Add(new ReportServer(reader));
                        return servers;
                    }
                }
            }
        }

        public async Task<List<ReportServer>> GetAllForGuild(ulong guildId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM report_servers r
                                                    JOIN servers s on r.server_id = s.id
                                                    WHERE s.guild_id = @gid", conn))
                {
                    cmd.Parameters.AddWithValue("gid", guildId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ReportServer> servers = new List<ReportServer>();
                        while (await reader.ReadAsync())
                            servers.Add(new ReportServer(reader));
                        return servers;
                    }
                }
            }
        }

        public async Task Remove(ReportServer reportServer)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"DELETE FROM report_servers 
                                                     WHERE server_id = @sid AND channel_id = @cid", conn))
                {
                    cmd.Parameters.AddWithValue("sid", reportServer.Server.Id);
                    cmd.Parameters.AddWithValue("cid", reportServer.ChannelId);

                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new Exception("No rows deleted");
                }
            }
        }
    }
}
