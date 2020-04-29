using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public static class EnumExtensions
    {
        private static ThreadLocal<Random> rand = new ThreadLocal<Random>
            (() => new Random());


        public static string Stringify<T>(this T val)
            where T:System.Enum
        {
            // thanks to https://stackoverflow.com/questions/2650080/how-to-get-c-sharp-enum-description-from-value
            // I just copied that - lol
            FieldInfo fi = val.GetType().GetField(val.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return val.ToString();
        }

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            int eln = rand.Value.Next(0, enumerable.Count());
            return enumerable.ElementAt(eln);
        }
        
    }
}
