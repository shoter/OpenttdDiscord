using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public class RevisionTranslator : IRevisionTranslator
    {
        public NewGrfRevision TranslateToNewGrfRevision(string revision)
        {
            byte major, minor, build, release;
            //1.10.0-RC1
            var splitted = revision.Split('.');
            major = byte.Parse(splitted[0]);
            minor = byte.Parse(splitted[1]);

            splitted = splitted[2].Split('-');

            build = byte.Parse(splitted[0]);

            release = (byte)(splitted.Length > 1 ? 0 : 1);

            return new NewGrfRevision(major, minor, build, release);
        }
    }
}
