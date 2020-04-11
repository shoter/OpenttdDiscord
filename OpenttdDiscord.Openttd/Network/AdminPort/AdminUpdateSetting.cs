using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminUpdateSetting
    {
        public bool Enabled { get; }

        public AdminUpdateType UpdateType { get; }

        public UpdateFrequency UpdateFrequency { get; }

        public AdminUpdateSetting(bool enabled, AdminUpdateType updateType, UpdateFrequency updateFrequency)
        {
            this.Enabled = enabled;
            this.UpdateType = updateType;
            this.UpdateFrequency = UpdateFrequency;
        }

        public override string ToString() => $"{this.UpdateType} - {this.UpdateFrequency}";

    }
}
