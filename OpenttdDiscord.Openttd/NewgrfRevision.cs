using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public class NewGrfRevision
    {
        // explanation
        // https://github.com/OpenTTD/OpenTTD/blob/master/src/rev.cpp.in
        public byte Major { get; }
        public byte Minor { get; }
        public byte Build { get; }

        /// <summary>
        /// 1 if release - 0 if not
        /// </summary>
        public byte Release { get; }

        public NewGrfRevision(byte major, byte minor, byte build, byte release)
        {
            this.Major = major;
            this.Minor = minor;
            this.Build = build;
            this.Release = release;
        }

        public uint Revision => (uint)(Major << 28 | Minor << 24 | Build << 20 | Release << 19 | 28004);



    }
}
