using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public class Player
    {
        public uint ClientId { get; }
        public string Name { get; set; }

        public Player(uint clientId) => this.ClientId = clientId;

        public Player(uint clientId, string name)
            :this(clientId) => this.Name = name;

    }
}
