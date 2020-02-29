using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Extensions
{
    public static class DbDataReaderExtension
    {
        public static string ReadString(this DbDataReader r, string columnName) => r[r.GetOrdinal(columnName)].ToString();

        public static int ReadInt(this DbDataReader r, string columnName) => int.Parse(r.ReadString(columnName));

        public static ulong ReadU64(this DbDataReader r, string columnName) => ulong.Parse(r.ReadString(columnName));

        public static T Read<T>(this DbDataReader r, string columnName) => r.GetFieldValue<T>(r.GetOrdinal(columnName));
        
    }
}
