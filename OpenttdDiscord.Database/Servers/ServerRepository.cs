using MySql.Data.MySqlClient;
using OpenttdDiscord.Backend;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public class ServerRepository : BaseRepository, IServerRepository
    {
        private readonly string connectionString;
        public ServerRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<Server> AddServer(ulong guildId, string ip, int port, string name)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO servers" +
                        "(server_ip, server_port, server_name, guild_id) " +
                        "VALUES " +
                        "(@ip, @port, @name, @gid)";
                    cmd.Parameters.AddWithValue("ip", ip);
                    cmd.Parameters.AddWithValue("port", port);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("gid", guildId);
                    await cmd.ExecuteNonQueryAsync();
                }

                ulong id = await this.GetLastId(conn);

                using (var cmd = new MySqlCommand($"SELECT * FROM servers where id = {id}", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    Server server = ReadFromReader(reader);
                    return server;
                }
            }
        }

        public async Task<List<Server>> GetAll(ulong guildId)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($"SELECT * FROM servers WHERE guild_id = @gid", conn))
                {
                    cmd.Parameters.AddWithValue("gid", guildId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        List<Server> servers = new List<Server>();
                        while (await reader.ReadAsync())
                            servers.Add(ReadFromReader(reader));
                        return servers;
                    }
                }
            }
        }

        public async Task<Server> GetServer(ulong guildId, string ip, int port)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($"SELECT * FROM servers where server_ip = @ip and server_port = @port AND guild_id = @gid", conn))
                {
                    cmd.Parameters.AddWithValue("ip", ip);
                    cmd.Parameters.AddWithValue("port", port);
                    cmd.Parameters.AddWithValue("gid", guildId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                            return ReadFromReader(reader);
                        else
                            return null;
                    }
                }
            }
        }

        public async Task<Server> GetServer(ulong guildId, string serverName)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($"SELECT * FROM servers where server_name = @name AND guild_id = @gid", conn))
                {
                    cmd.Parameters.AddWithValue("name", serverName);
                    cmd.Parameters.AddWithValue("gid", guildId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return ReadFromReader(reader);
                        else
                            return null;
                    }
                }
            }
        }

        public async Task<List<Server>> GetServers(string ip, int port)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($"SELECT * FROM servers where server_ip = @ip and server_port = @port", conn))
                {
                    cmd.Parameters.AddWithValue("ip", ip);
                    cmd.Parameters.AddWithValue("port", port);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        List<Server> list = new List<Server>();
                        while (await reader.ReadAsync())
                            list.Add(ReadFromReader(reader));
                        return list;
                    }
                }
            }
        }

        public Server ReadFromReader(DbDataReader reader) => new Server(reader, "servers");

        public async Task UpdatePassword(ulong serverId, string password)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"UPDATE servers
                                                      SET server_password = @pwd
                                                      where id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", serverId);
                    cmd.Parameters.AddWithValue("pwd", password);

                    int modifiedRows = await cmd.ExecuteNonQueryAsync();
                    if (modifiedRows != 1)
                        throw new OttdException($"Something went wrong with updating password for {serverId}");
                }
            }
        }
    }
}
