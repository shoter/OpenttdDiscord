using MySql.Data.MySqlClient;
using OpenttdDiscord.Backend.Chatting;
using OpenttdDiscord.Backend.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Chatting
{
    public class ChatChannelServerRepository : IChatChannelServerRepository
    {
        private readonly string connectionString;

        public ChatChannelServerRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<List<ChatChannelServer>> GetAllAsync()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<ChatChannelServer>();

                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_chat_channel_servers", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        _list.Add(ReadFromReader(reader));
                }

                return _list;
            }
        }

        public async Task<ChatChannelServer> Insert(ChatChannelServer server)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO discord_chat_channel_servers" +
                        "(server_id, channel_id, server_name) " +
                        "VALUES " +
                        "(@server_id, @channel_id, @server_name)";
                    cmd.Parameters.AddWithValue("server_id", server.ServerId);
                    cmd.Parameters.AddWithValue("channel_id", server.ChannelId);
                    cmd.Parameters.AddWithValue("server_name", server.ServerName);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_chat_channel_servers
                                                    where server_id = {server.ServerId} AND channel_id = {server.ChannelId}", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    return ReadFromReader(reader);
                }
            }
        }

        public async Task<bool> Exists(ulong serverId, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_chat_channel_servers
                                                    where server_id = {serverId} AND channel_id = {channelId}", conn))
                {
                    return (await cmd.GetCount()) == 1;
                }
            }
        }

        public ChatChannelServer ReadFromReader(DbDataReader reader)
        {
            return new ChatChannelServer(
                reader.ReadU64("server_id"),
                reader.ReadU64("channel_id"),
                reader.ReadString("server_name")
                );
        }

        public async Task<ChatChannelServer> Get(ulong serverId, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<ChatChannelServer>();

                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_chat_channel_servers
                                                     WHERE server_id={serverId} AND channel_id={channelId}", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return this.ReadFromReader(reader);
                    else
                        return null;
                }
            }
        }
    }
}
