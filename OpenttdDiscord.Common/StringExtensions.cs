using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public static class StringExtensions
    {
        public static string FirstUpper(this string str)
        {
            string remainder = str.Length > 0 ? str.Substring(1).ToLower() : string.Empty;
            return $"{char.ToUpper(str.First())}{remainder}";
        }

    }
}
