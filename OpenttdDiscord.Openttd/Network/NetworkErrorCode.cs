using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public enum NetworkErrorCode
    {
        NETWORK_ERROR_GENERAL, // Try to use this one like never

        /* Signals from clients */
        NETWORK_ERROR_DESYNC,
        NETWORK_ERROR_SAVEGAME_FAILED,
        NETWORK_ERROR_CONNECTION_LOST,
        NETWORK_ERROR_ILLEGAL_PACKET,
        NETWORK_ERROR_NEWGRF_MISMATCH,

        /* Signals from servers */
        NETWORK_ERROR_NOT_AUTHORIZED,
        NETWORK_ERROR_NOT_EXPECTED,
        NETWORK_ERROR_WRONG_REVISION,
        NETWORK_ERROR_NAME_IN_USE,
        NETWORK_ERROR_WRONG_PASSWORD,
        NETWORK_ERROR_COMPANY_MISMATCH, // Happens in CLIENT_COMMAND
        NETWORK_ERROR_KICKED,
        NETWORK_ERROR_CHEATER,
        NETWORK_ERROR_FULL,
        NETWORK_ERROR_TOO_MANY_COMMANDS,
        NETWORK_ERROR_TIMEOUT_PASSWORD,
        NETWORK_ERROR_TIMEOUT_COMPUTER,
        NETWORK_ERROR_TIMEOUT_MAP,
        NETWORK_ERROR_TIMEOUT_JOIN,

        NETWORK_ERROR_END,
    }
}
