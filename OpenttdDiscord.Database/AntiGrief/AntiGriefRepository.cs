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

        public async Task<AntiGriefServer> Add(Server server, TimeSpan requiredTimeToPlay, string reason)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"INSERT INTO antigrief_servers(server_id, required_mins_to_play, reason) 
                                                     VALUES (@sid, @req, @reason)", conn))
                {
                    cmd.Parameters.AddWithValue("sid", server.Id);
                    cmd.Parameters.AddWithValue("req", (int)requiredTimeToPlay.TotalMinutes);
                    cmd.Parameters.AddWithValue("reason", reason);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand($@"SELECT * FROM antigrief_servers ag
                                                    JOIN servers s on ag.server_id = s.id
                                                    WHERE s.id = @sid", conn))
                {
                    cmd.Parameters.AddWithValue("sid", server.Id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        return new AntiGriefServer(reader);
                    }
                }
            }
        }

        public async Task<AntiGriefServer> Get(ulong serverId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM antigrief_servers ag
                                                    JOIN servers s on ag.server_id = s.id
                                                    WHERE s.id = @sid", conn))
                {
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

        public async Task<List<AntiGriefServer>> GetAllForGuild(ulong guildId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM antigrief_servers r
                                                    JOIN servers s ON ag.server_id = s.id
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

        public async Task<List<AntiGriefServer>> GetAll()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM antigrief_servers ag
                                                    JOIN servers s ON ag.server_id = s.id", conn))
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

        public async Task Remove(AntiGriefServer reportServer)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"DELETE FROM antigrief_servers 
                                                     WHERE server_id = @sid", conn))
                {
                    cmd.Parameters.AddWithValue("sid", reportServer.Server.Id);

                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new Exception("No rows deleted");
                }
            }
        }
    }
}
