using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Extensions
{
    public static class MySqlCommandExtensions
    {
        public static async Task<ulong> GetCount(this MySqlCommand cmd) => ulong.Parse((await cmd.ExecuteScalarAsync()).ToString());
        
    }
}
