using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public interface IRevisionTranslator
    {
        NewGrfRevision TranslateToNewGrfRevision(string revision);
    }
}
