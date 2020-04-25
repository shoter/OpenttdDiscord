using MySql.Data.MySqlClient;
using OpenttdDiscord.Backend;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Extensions;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Chatting
{
    public class ChatChannelServerRepository : IChatChannelServerRepository
    {
        private readonly string connectionString;

        public ChatChannelServerRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<List<ChatChannelServer>> GetAll(ulong guildId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<ChatChannelServer>();

                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_chat_channel_servers d
                                                    JOIN servers s on d.server_id = s.id
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

        public async Task<List<ChatChannelServer>> GetAll()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<ChatChannelServer>();

                using (var cmd = new MySqlCommand($@"SELECT * FROM discord_chat_channel_servers d
                                                    JOIN servers s on d.server_id = s.id", conn))
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

        public async Task<ChatChannelServer> Insert(Server server, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO discord_chat_channel_servers" +
                        "(server_id, channel_id) " +
                        "VALUES " +
                        "(@server_id, @channel_id)";
                    cmd.Parameters.AddWithValue("server_id", server.Id);
                    cmd.Parameters.AddWithValue("channel_id", channelId);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = GetServerCommand(server.Id, channelId, conn))
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
                using (var cmd = new MySqlCommand($@"SELECT count(*) FROM discord_chat_channel_servers d
                                                    JOIN servers s on d.server_id = s.id", conn))
                {
                    return (await cmd.GetCount()) == 1;
                }
            }
        }

        public ChatChannelServer ReadFromReader(DbDataReader reader) => new ChatChannelServer()
        {
            Server = new Server(reader, "servers"),
            ChannelId = reader.ReadU64("channel_id", "discord_chat_channel_servers"),
            JoinMessagesEnabled = reader.ReadBool("connect_message_enabled", "discord_chat_channel_servers")
        };

        public async Task<ChatChannelServer> Get(ulong serverId, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<ChatChannelServer>();

                using var cmd = GetServerCommand(serverId, channelId, conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return this.ReadFromReader(reader);
                    else
                        return null;
                }
            }
        }

        private MySqlCommand GetServerCommand(ulong serverId, ulong channelId, MySqlConnection conn) => new MySqlCommand($@"SELECT * FROM discord_chat_channel_servers d
                                                    JOIN servers s on d.server_id = s.id
                                                    where d.server_id = {serverId} AND d.channel_id = {channelId}", conn);

        public async Task Remove(ulong serverId, ulong channelId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                var _list = new List<ChatChannelServer>();

                var cmd = new MySqlCommand($@"DELETE FROM discord_chat_channel_servers
                                                    WHERE server_id = @server_id AND channel_id = @channel_id", conn);
                cmd.Parameters.AddWithValue("server_id", serverId);
                cmd.Parameters.AddWithValue("channel_id", channelId);

                int rows = await cmd.ExecuteNonQueryAsync();

                if(rows == 0)
                    throw new Exception($"{nameof(Remove)} for {nameof(ServerRepository)} did not remove record {serverId} for channel {channelId}");
            }
        }
    }
}
