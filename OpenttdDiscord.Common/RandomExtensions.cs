using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public static class RandomExtensions
    {
        public static string RandomString(this Random rand, int minChar, int maxChar)
        {
            int charCount = rand.Next(minChar, maxChar);
            var ss = new StringBuilder();
            for(int i = 0;i < charCount; ++i)
            {
                ss.Append(rand.RandomChar());
            }
            return ss.ToString();
        }

        public static char RandomChar(this Random rand)
        {
            return (char)rand.Next((int)'a', ((int)'z') + 1);
        }
        
    }
}
