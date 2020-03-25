using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public static class Enums
    {
        public static T[] ToArray<T>()
            where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToArray();

    }
}
