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

        public async Task<bool> Exists(string ip, int port)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT COUNT(*) FROM subscribed_servers ss" +
                        " JOIN servers s on ss.server_id = s.id" +
                        " WHERE s.server_ip = @ip AND s.server_port = @port ";
                    cmd.Parameters.AddWithValue("ip", ip);
                    cmd.Parameters.AddWithValue("port", port);
                    return (await cmd.GetCount()) == 1;
                }
            }
        }

        public async Task UpdateServer(ulong serverId, ulong messageId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE subscribed_servers " +
                        "SET last_update = now(), message_id = @message_id" +
                        " WHERE server_id = @id";
                    cmd.Parameters.AddWithValue("id", serverId);
                    cmd.Parameters.AddWithValue("message_id", messageId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public SubscribedServer ReadFromReader(DbDataReader reader)
        {
            return new SubscribedServer(new Server(
                reader.ReadU64("id"), reader.ReadString("server_ip"), reader.ReadInt("server_port")),
                reader.Read<DateTimeOffset>("last_update"), reader.ReadU64("channel_id"));
        }
    }
}
