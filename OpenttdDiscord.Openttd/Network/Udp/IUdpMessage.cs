﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    public interface IUdpMessage
    {
        UdpMessageType MessageType { get; }
    }
}
