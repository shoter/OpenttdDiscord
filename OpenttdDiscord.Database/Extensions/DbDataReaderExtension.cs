using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Extensions
{
    public static class DbDataReaderExtension
    {
        public static string ReadString(this DbDataReader r, string columnName, string tableName = null) => r[r.GetOrdinal(columnName, tableName)].ToString();


        public static int ReadInt(this DbDataReader r, string columnName, string tableName = null) => int.Parse(r.ReadString(columnName, tableName));


        public static bool ReadBool(this DbDataReader r, string columnName, string tableName = null) => int.Parse(r.ReadString(columnName, tableName)) != 0;


        public static ulong ReadU64(this DbDataReader r, string columnName, string tableName = null) => ulong.Parse(r.ReadString(columnName, tableName));


        public static T Read<T>(this DbDataReader r, string columnName, string tableName = null) => r.GetFieldValue<T>(r.GetOrdinal(columnName, tableName));



        public static int GetOrdinal(this DbDataReader r, string columnName, string tableName)
        {
            if (tableName == null)
                return r.GetOrdinal(columnName);

            var schema = r.GetColumnSchema();
            for(int i = 0;i < schema.Count; ++i)
            {
                var col = schema[i];
                if (col.ColumnName.ToLower() == columnName.ToLower() && col.BaseTableName.ToLower() == tableName.ToLower())
                    return col.ColumnOrdinal.Value;

            }
            throw new KeyNotFoundException($"{tableName}.{columnName}");
        }

        public static T ReadNullable<T>(this DbDataReader r, string columnName, string tableName = null)
        {
            int ordinal = r.GetOrdinal(columnName, tableName);
            if (r.IsDBNull(ordinal))
                return default(T);
            return r.Read<T>(columnName);
        }
    }
}
