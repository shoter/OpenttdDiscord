using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySqlConnector;

namespace OpenttdDiscord.Database.AntiGrief
{
    public class TrustedIpRepository : ITrustedIpRepository
    {
        private readonly string connectionString;

        public TrustedIpRepository(MySqlConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public async Task<TrustedIp> Add(TrustedIp trustedIp)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"INSERT INTO trusted_ips(ip_address, playing_time_minutes) 
                                                     VALUES (@ip, @pt)", conn))
                {
                    cmd.Parameters.AddWithValue("ip", trustedIp.IpAddress);
                    cmd.Parameters.AddWithValue("pt", (int)trustedIp.PlayingTime.TotalMinutes);


                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new MySqlCommand($@"SELECT * FROM trusted_ips ti
                                                     WHERE ti.ip_address = @ip", conn))
                {
                    cmd.Parameters.AddWithValue("ip", trustedIp.IpAddress);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        return new TrustedIp(reader);
                    }
                }
            }
        }

        public async Task<TrustedIp> Get(string ipAddress)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM trusted_ips ti WHERE ip_address = @ip", conn))
                {
                    cmd.Parameters.AddWithValue("ip", ipAddress);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            return new TrustedIp(reader);
                        return null;
                    }
                }
            }
        }

        public async Task<List<TrustedIp>> GetAll()
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand($@"SELECT * FROM trusted_ips ti", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        List<TrustedIp> servers = new List<TrustedIp>();
                        while (await reader.ReadAsync())
                            servers.Add(new TrustedIp(reader));
                        return servers;
                    }
                }
            }
        }

        public async Task Remove(string ipAddress)
        {
            using (var conn = new MySqlConnection(this.connectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new MySqlCommand($@"DELETE FROM trusted_ips
                                                     WHERE ip_address = @ip", conn))
                {
                    cmd.Parameters.AddWithValue("ip", ipAddress);

                    int rows = await cmd.ExecuteNonQueryAsync();

                    if (rows == 0)
                        throw new Exception("No rows deleted");
                }
            }
        }
    }
}
