using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    public enum UdpMessageType
    {
		/// <summary>
		/// queriesa game server for game information
		/// </summary>
		PACKET_UDP_CLIENT_FIND_SERVER = 0,
		/// <summary>
		/// Reply of the game server with game information
		/// </summary>
		PACKET_UDP_SERVER_RESPONSE = 1,
		PACKET_UDP_CLIENT_DETAIL_INFO,   ///< Queries a game server about details of the game, such as companies
		PACKET_UDP_SERVER_DETAIL_INFO,   ///< Reply of the game server about details of the game, such as companies
		PACKET_UDP_SERVER_REGISTER,      ///< Packet to register itself to the master server
		PACKET_UDP_MASTER_ACK_REGISTER,  ///< Packet indicating registration has succeeded
		PACKET_UDP_CLIENT_GET_LIST,      ///< Request for serverlist from master server
		PACKET_UDP_MASTER_RESPONSE_LIST, ///< Response from master server with server ip's + port's
		PACKET_UDP_SERVER_UNREGISTER,    ///< Request to be removed from the server-list
		PACKET_UDP_CLIENT_GET_NEWGRFS,   ///< Requests the name for a list of GRFs (GRF_ID and MD5)
		PACKET_UDP_SERVER_NEWGRFS,       ///< Sends the list of NewGRF's requested.
		PACKET_UDP_MASTER_SESSION_KEY,   ///< Sends a fresh session key to the client
    }
}
