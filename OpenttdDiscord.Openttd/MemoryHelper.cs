using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public class MemoryHelper
    {
        /**
         * Fetch \a n bits from \a x, started at bit \a s.
         *
         * This function can be used to fetch \a n bits from the value \a x. The
         * \a s value set the start position to read. The start position is
         * count from the LSB and starts at \c 0. The result starts at a
         * LSB, as this isn't just an and-bitmask but also some
         * bit-shifting operations. GB(0xFF, 2, 1) will so
         * return 0x01 (0000 0001) instead of
         * 0x04 (0000 0100).
         *
         * @param x The value to read some bits.
         * @param s The start position to read some bits.
         * @param n The number of bits to read.
         * @pre n < sizeof(T) * 8
         * @pre s + n <= sizeof(T) * 8
         * @return The selected bits, aligned to a LSB.
         */

        public static uint GB(long x, ushort s, ushort n)
        {
            return (ushort)((x >> s) & (((long)1U << n) - 1));
        }
    }
}
