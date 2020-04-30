using MySql.Data.MySqlClient;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public class AdminChannelRepository : IAdminChannelRepository
    {
        private readonly string connectionString;
        public AdminChannelRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;

        }
        public async Task<List<AdminChannel>> GetAdminChannels(Server server)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_admin_channels d
                                                    JOIN servers s on d.server_id = s.id
                                                    WHERE s.id = @sid", conn))
                {
                    cmd.Parameters.AddWithValue("sid", server.Id);
                    List<AdminChannel> list = new List<AdminChannel>();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            list.Add(new AdminChannel(reader));
                    }
                    return list;
                }
            }
        }

        public async Task<List<AdminChannel>> GetAdminChannels(ulong guildId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_admin_channels d
                                                    JOIN servers s on d.server_id = s.id
                                                    WHERE s.guild_id = @gid", conn))
                {
                    cmd.Parameters.AddWithValue("gid", guildId);
                    List<AdminChannel> list = new List<AdminChannel>();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            list.Add(new AdminChannel(reader));
                    }
                    return list;
                }
            }
        }

        public async Task<List<AdminChannel>> GetAdminChannelsForChannel(ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_admin_channels d
                                                    JOIN servers s on d.server_id = s.id
                                                    WHERE d.channel_id = @cid", conn))
                {
                    cmd.Parameters.AddWithValue("cid", channelId);
                    List<AdminChannel> list = new List<AdminChannel>();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            list.Add(new AdminChannel(reader));
                    }
                    return list;
                }
            }
        }

        public async Task<AdminChannel> Insert(Server server, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"INSERT INTO discord_admin_channels(server_id, channel_id) 
                                                     VALUES (@sid, @cid)", conn))
                {
                    cmd.Parameters.AddWithValue("sid", server.Id);
                    cmd.Parameters.AddWithValue("cid", channelId);

                    await cmd.ExecuteNonQueryAsync();

                }

                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_admin_channels d
                                                    JOIN servers s on d.server_id = s.id
                                                    WHERE s.id = @sid AND d.channel_id = @cid", conn))
                {
                    cmd.Parameters.AddWithValue("sid", server.Id);
                    cmd.Parameters.AddWithValue("cid", channelId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        return new AdminChannel(reader);
                    }
                }
            }
        }

        public async Task RemoveChannel(ulong serverId, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"DELETE FROM discord_admin_channels 
                                                     WHERE server_id = @sid AND channel_id = @cid", conn))
                {
                    cmd.Parameters.AddWithValue("sid", serverId);
                    cmd.Parameters.AddWithValue("cid", channelId);

                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new Exception("No rows deleted");
                }
            }
        }
    }
}
