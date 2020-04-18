using MySql.Data.MySqlClient;
using OpenttdDiscord.Backend.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Servers
{
    public class SubscribedServerRepository : BaseRepository, ISubscribedServerRepository
    {
        private readonly string connectionString;

        public SubscribedServerRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<SubscribedServer> Add(Server server, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO subscribed_servers" +
                        "(server_id, last_update, channel_id) " +
                        "VALUES " +
                        "(@server_id, now(), @channel_id)";
                    cmd.Parameters.AddWithValue("server_id", server.Id);
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

        public async Task<SubscribedServer> Get(string ip, int port, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"
                    SELECT * FROM subscribed_servers ss
                       join servers s on ss.server_id = s.id
                        WHERE s.server_ip = @ip AND s.server_port = @port AND ss.channel_id = @cid ", conn))
                {
                    cmd.Parameters.AddWithValue("ip", ip);
                    cmd.Parameters.AddWithValue("port", port);
                    cmd.Parameters.AddWithValue("cid", channelId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return ReadFromReader(reader);
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<IEnumerable<SubscribedServer>> GetAll()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<SubscribedServer>();

                using (var cmd = new MySqlCommand($@"SELECT * FROM subscribed_servers ss
                                                    join servers s on ss.server_id = s.id", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        _list.Add(ReadFromReader(reader));
                }

                return _list;
            }
        }

        public async Task<bool> Exists(string ip, int port, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT COUNT(*) FROM subscribed_servers ss" +
                        " JOIN servers s on ss.server_id = s.id" +
                        " WHERE s.server_ip = @ip AND s.server_port = @port AND ss.channel_id = @cid ";
                    cmd.Parameters.AddWithValue("ip", ip);
                    cmd.Parameters.AddWithValue("port", port);
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
                reader.ReadNullable<ulong?>("message_id"));
        }
    }
}
