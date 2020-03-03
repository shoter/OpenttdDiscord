using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public class Packet
    {
        public ushort Size { get; private set; } = 2; // first 2 bytes are for size.
        public ushort Position { get; private set; } = 0;

        public byte[] Buffer { get; } = new byte[1460];

        public Packet() { }

        public Packet(byte[] buffer)
        {
            this.Buffer = buffer;
            this.Size = ReadU16();
        }

        public void PrepareToSend()
        {
            byte[] bytes = BitConverter.GetBytes(Size);
            this.Buffer[0] = bytes[0];
            this.Buffer[1] = bytes[1];
        }

        public void SendByte(byte value)
        {
            this.Buffer[this.Size] = value;
            this.Size += 1;
        }

        public void SendU16(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.Buffer[this.Size] = bytes[0];
            this.Buffer[this.Size + 1] = bytes[1];

            this.Size += 2;
        }
        public void SendU32(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.Buffer[this.Size] = bytes[0];
            this.Buffer[this.Size + 1] = bytes[1];
            this.Buffer[this.Size + 2] = bytes[2];
            this.Buffer[this.Size + 3] = bytes[3];

            this.Size += 4;
        }

        public void SendString(string str)
        {
            foreach(char c in str)
            {
                this.Buffer[this.Size++] = (byte)c;
            }

            this.Buffer[this.Size++] = 0;
        }

        public byte ReadByte() => this.Buffer[this.Position++];

        public bool ReadBool() => ReadByte() != 0;

        public ushort ReadU16() => BitConverter.ToUInt16(this.Buffer, (this.Position += 2) - 2);

        public uint ReadU32() => BitConverter.ToUInt32(this.Buffer, (this.Position += 4) - 4); 


        public string ReadString()
        {
            StringBuilder str = new StringBuilder();

            while(this.Buffer[this.Position] != 0)
            {
                char c = (char)this.Buffer[this.Position++];
                str.Append(c);
            }

            this.Position++;

            return str.ToString();
        }
    }
}
