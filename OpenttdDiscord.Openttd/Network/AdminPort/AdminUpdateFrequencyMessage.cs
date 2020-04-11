using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminUpdateFrequencyMessage : IAdminMessage
    {
        public AdminMessageType MessageType => AdminMessageType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY;

        public AdminUpdateType UpdateType { get; }

        public UpdateFrequency UpdateFrequency { get; }

        public AdminUpdateFrequencyMessage(AdminUpdateType updateType, UpdateFrequency updateFrequency)
        {
            this.UpdateType = updateType;
            this.UpdateFrequency = updateFrequency;
        }

        public override string ToString() => $"Update FREQ {UpdateType} to {UpdateFrequency}";
    }
}
