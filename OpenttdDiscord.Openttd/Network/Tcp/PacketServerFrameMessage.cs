﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class PacketServerFrameMessage : ITcpMessage
    {
        public TcpMessageType MessageType => TcpMessageType.PACKET_SERVER_FRAME;

        public uint FrameCounter { get; }

        public uint FrameCounterMax { get; }

        public byte Token { get; }

        public PacketServerFrameMessage(uint frameCounter, uint frameCounterMax, byte token)
        {
            this.FrameCounter = frameCounter;
            this.FrameCounterMax = frameCounterMax;
            this.Token = token;
        }
    }
}
