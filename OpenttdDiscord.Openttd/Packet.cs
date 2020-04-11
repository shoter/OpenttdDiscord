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

        public void SendU64(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            this.Buffer[this.Size] = bytes[0];
            this.Buffer[this.Size + 1] = bytes[1];
            this.Buffer[this.Size + 2] = bytes[2];
            this.Buffer[this.Size + 3] = bytes[3];
            this.Buffer[this.Size + 4] = bytes[4];
            this.Buffer[this.Size + 5] = bytes[5];
            this.Buffer[this.Size + 6] = bytes[6];
            this.Buffer[this.Size + 7] = bytes[7];

            this.Size += 8;
        }


        public void SendString(string str)
        {
            var bytes = Encoding.Default.GetBytes(str);
            foreach (byte b in bytes)
                SendByte(b);

            SendByte(0);
        }

        public void SendString(string str, int size)
        {
            var bytes = Encoding.Default.GetBytes(str);

            for (int i = 0;i < size; ++ i)
            {
                if (i < bytes.Length)
                {
                    SendByte(bytes[i]);
                }
                else
                {
                    SendByte(0);
                    break;
                }
            }
        }

        public byte ReadByte() => this.Buffer[this.Position++];

        public bool ReadBool() => ReadByte() != 0;

        public ushort ReadU16() => BitConverter.ToUInt16(this.Buffer, (this.Position += 2) - 2);

        public uint ReadU32() => BitConverter.ToUInt32(this.Buffer, (this.Position += 4) - 4);

        public ulong ReadU64() => BitConverter.ToUInt64(this.Buffer, (this.Position += 8) - 8);

        public long ReadI64() => BitConverter.ToInt64(this.Buffer, (this.Position += 8) - 8);



        public string ReadString()
        {
            List<byte> bytes = new List<byte>();

            while (this.Buffer[this.Position] != 0)
            {
                bytes.Add(this.Buffer[this.Position++]);
            }

            this.Position++;

            return Encoding.Default.GetString(bytes.ToArray());
        }

        public string ReadString(int size)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0;i < size; ++i)
            {
                bytes.Add(this.Buffer[this.Position]);
            }

            return Encoding.Default.GetString(bytes.ToArray());
        }
    }
}
