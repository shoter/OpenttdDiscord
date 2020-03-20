using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public enum UpdateFrequency
    {
		ADMIN_FREQUENCY_POLL      = 0x01, /// The admin can poll this.
		ADMIN_FREQUENCY_DAILY     = 0x02, /// The admin gets information about this on a daily basis.
		ADMIN_FREQUENCY_WEEKLY    = 0x04, /// The admin gets information about this on a weekly basis.
		ADMIN_FREQUENCY_MONTHLY   = 0x08, /// The admin gets information about this on a monthly basis.
		ADMIN_FREQUENCY_QUARTERLY = 0x10, /// The admin gets information about this on a quarterly basis.
		ADMIN_FREQUENCY_ANUALLY   = 0x20, /// The admin gets information about this on a yearly basis.
		ADMIN_FREQUENCY_AUTOMATIC = 0x40, /// The admin gets information about this when it changes.
    }
}
