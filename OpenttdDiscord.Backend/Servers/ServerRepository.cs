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
    public class ServerRepository : BaseRepository, IServerRepository
    {
        private readonly string connectionString;
        public ServerRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<Server> AddServer(string ip, int port)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO servers" +
                        "(server_ip, server_port) " +
                        "VALUES " +
                        "(@ip, @port)";
                    cmd.Parameters.AddWithValue("ip", ip);
                    cmd.Parameters.AddWithValue("port", port);
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

        public async Task<List<Server>> GetAll()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($"SELECT * FROM servers", conn))
                {
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

        public async Task<Server> GetServer(string ip, int port)
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
                        if(await reader.ReadAsync())
                            return ReadFromReader(reader);
                        else
                            return null;
                    }
                }
            }
        }

        public Server ReadFromReader(DbDataReader reader)
        {
            return new Server(
                id: reader.ReadU64("id"),
                serverIp: reader.ReadString("server_ip"),
                serverPort: reader.ReadInt("server_port")
                );
        }
    }
}
