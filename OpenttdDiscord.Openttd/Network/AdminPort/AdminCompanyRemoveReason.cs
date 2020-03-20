using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public enum AdminCompanyRemoveReason
    {
        /// <summary>
        /// The company is manually removed.
        /// </summary>
        ADMIN_CRR_MANUAL,
        /// <summary>
        /// The company is removed due to autoclean.
        /// </summary>
        ADMIN_CRR_AUTOCLEAN,
        /// <summary>
        /// The company went belly-up.
        /// </summary>
        ADMIN_CRR_BANKRUPT,
        /// <summary>
        /// Sentinel for end.
        /// </summary>
        ADMIN_CRR_END,
    }
}
