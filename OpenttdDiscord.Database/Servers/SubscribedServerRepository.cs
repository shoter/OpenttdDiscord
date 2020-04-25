using MySql.Data.MySqlClient;
using OpenttdDiscord.Backend;
using OpenttdDiscord.Database.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public class SubscribedServerRepository : BaseRepository, ISubscribedServerRepository
    {
        private readonly string connectionString;

        public SubscribedServerRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<SubscribedServer> Add(Server server, int port, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO subscribed_servers" +
                        "(server_id, last_update, channel_id, subscribe_port) " +
                        "VALUES " +
                        "(@server_id, now(), @channel_id, @sub_port)";
                    cmd.Parameters.AddWithValue("server_id", server.Id);
                    cmd.Parameters.AddWithValue("sub_port", port);
                    cmd.Parameters.AddWithValue("channel_id", channelId);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand($@"SELECT * FROM subscribed_servers ss
                                                    join servers s on ss.server_id = s.id
                                                    where ss.server_id = {server.Id}", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    return ReadFromReader(reader);
                }
            }
        }

        public async Task<IEnumerable<SubscribedServer>> GetAll(ulong guildId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<SubscribedServer>();

                using (var cmd = new MySqlCommand($@"SELECT * FROM subscribed_servers ss
                                                    JOIN servers s on ss.server_id = s.id
                                                    WHERE s.guild_id = @gid", conn))
                {
                    cmd.Parameters.AddWithValue("gid", guildId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            _list.Add(ReadFromReader(reader));
                    }
                }

                return _list;
            }
        }
        public async Task<IEnumerable<SubscribedServer>> GetAll()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<SubscribedServer>();

                using (var cmd = new MySqlCommand($@"SELECT * FROM subscribed_servers ss
                                                    JOIN servers s on ss.server_id = s.id", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            _list.Add(ReadFromReader(reader));
                    }
                }

                return _list;
            }
        }


        public async Task<bool> Exists(Server server, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"SELECT COUNT(*) FROM subscribed_servers ss
                                      WHERE ss.server_id = @server_id AND ss.channel_id = @cid";
                    cmd.Parameters.AddWithValue("server_id", server.Id);
                    cmd.Parameters.AddWithValue("cid", channelId);

                    return (await cmd.GetCount()) == 1;
                }
            }
        }

        public async Task UpdateServer(ulong serverId, ulong channelId, ulong messageId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE subscribed_servers " +
                        "SET last_update = now(), message_id = @message_id" +
                        " WHERE server_id = @id AND channel_id = @cid";
                    cmd.Parameters.AddWithValue("id", serverId);
                    cmd.Parameters.AddWithValue("cid", channelId);
                    cmd.Parameters.AddWithValue("message_id", messageId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public SubscribedServer ReadFromReader(DbDataReader reader)
        {
            return new SubscribedServer(new Server(reader),
                reader.Read<DateTimeOffset>("last_update"), reader.ReadU64("channel_id"),
                reader.ReadNullable<ulong?>("message_id"), reader.ReadInt("subscribe_port"));
        }

        public async Task Remove(Server server, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"DELETE FROM subscribed_servers
                                       WHERE server_id = @id AND channel_id = @cid";
                    cmd.Parameters.AddWithValue("id", server.Id);
                    cmd.Parameters.AddWithValue("cid", channelId);

                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new Exception($"{nameof(Remove)} for {nameof(SubscribedServerRepository)} did not remove record {server.Id} for channel {channelId}");
                }
            }
        }

        public async Task<SubscribedServer> Get(Server server, ulong channelId)
        {
            using var conn = new MySqlConnection(this.connectionString);
            await conn.OpenAsync();
            using var cmd = new MySqlCommand($@"
                    SELECT * FROM subscribed_servers ss
                    join servers s on ss.server_id = s.id
                    WHERE s.id = @server_id AND ss.channel_id = @cid ", conn);

            cmd.Parameters.AddWithValue("server_id", server.Id);
            cmd.Parameters.AddWithValue("cid", channelId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadFromReader(reader);
            }
            return null;
        }

    }
}
