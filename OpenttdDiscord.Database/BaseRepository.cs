using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend
{
    public class BaseRepository
    {
        public async Task<ulong> GetLastId(MySqlConnection conn)
        {
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT LAST_INSERT_ID()";
                using (var reader = await cmd.ExecuteReaderAsync())
                    if (await reader.ReadAsync())
                        return ulong.Parse(reader[0].ToString());
            }

            throw new Exception();
        }
        
    }
}
