using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySqlConnector;

using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Database.AntiGrief
{
    public class AntiGriefRepository : IAntiGriefRepository
    {
        private readonly string connectionString;

        public AntiGriefRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<AntiGriefServer> Add(Server server, ulong channelId)
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
                        return new AntiGriefServer(reader);
                    }
                }
            }
        }

        public async Task<AntiGriefServer> Get(ulong serverId, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM report_servers r
                                                    JOIN servers s on r.server_id = s.id
                                                    WHERE r.channel_id = @cid AND r.server_id = @sid", conn))
                {
                    cmd.Parameters.AddWithValue("cid", channelId);
                    cmd.Parameters.AddWithValue("sid", serverId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return new AntiGriefServer(reader);
                        return null;
                    }
                }
            }
        }

        public async Task<List<AntiGriefServer>> GetAll(ulong channelId)
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
                        List<AntiGriefServer> servers = new List<AntiGriefServer>();
                        while (await reader.ReadAsync())
                            servers.Add(new AntiGriefServer(reader));
                        return servers;
                    }
                }
            }
        }

        public async Task<List<AntiGriefServer>> GetAll()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM report_servers r
                                                    JOIN servers s on r.server_id = s.id", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        List<AntiGriefServer> servers = new List<AntiGriefServer>();
                        while (await reader.ReadAsync())
                            servers.Add(new AntiGriefServer(reader));
                        return servers;
                    }
                }
            }
        }

        public async Task<List<AntiGriefServer>> GetAllForGuild(ulong guildId)
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
                        List<AntiGriefServer> servers = new List<AntiGriefServer>();
                        while (await reader.ReadAsync())
                            servers.Add(new AntiGriefServer(reader));
                        return servers;
                    }
                }
            }
        }

        public async Task Remove(AntiGriefServer reportServer)
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
