using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public enum Landscape
    {
        [Description("Temperate")]
        LT_TEMPERATE = 0,
        [Description("Arctic")]
        LT_ARCTIC = 1,
        [Description("Tropic")]
        LT_TROPIC = 2,
        [Description("Toyland")]
        LT_TOYLAND = 3,
    }
}
