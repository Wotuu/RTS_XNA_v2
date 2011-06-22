using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Packets
{
    public class Packet
    {
        private byte header { get; set; }
        private Boolean isHeaderSet { get; set; }
        private LinkedList<byte> data = new LinkedList<byte>();

        public Packet()
        {
        }

        public Packet(byte header)
        {
            this.SetHeader(header);
        }

        public Packet(byte header, byte[] data)
        {
            this.SetHeader(header);
            for (int i = 0; i < data.Length; i++)
            {
                this.data.AddLast(data[i]);
            }
        }

        public Packet(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.data.AddLast(data[i]);
            }
        }

        /// <summary>
        /// Sets the header of this byte. Also see ProtocolHeader.cs
        /// </summary>
        /// <param name="header">The header to give this packet</param>
        public void SetHeader(byte header)
        {
            this.header = header;
            isHeaderSet = true;
        }

        /// <summary>
        /// Gets the header of this packet.
        /// </summary>
        /// <returns>The header</returns>
        public byte GetHeader()
        {
            return this.header;
        }

        /// <summary>
        /// Adds a string to this packet
        /// </summary>
        /// <param name="s">The packet</param>
        public void AddString(String s)
        {
            foreach (Char c in s.ToCharArray())
            {
                this.data.AddLast((byte)c);
            }
        }

        /// <summary>
        /// Adds an int to this packet.
        /// </summary>
        /// <param name="value">The int to add</param>
        public void AddInt(int value)
        {
            foreach (Byte b in BitConverter.GetBytes(value))
            {
                this.data.AddLast(b);
            }
        }

        /// <summary>
        /// Adds a byte of data to the list.
        /// </summary>
        /// <param name="data">The byte to add</param>
        public void AddData(byte data)
        {
            this.data.AddLast(data);
        }

        /// <summary>
        /// Converts the data in this packet to a byte array, this includes the header.
        /// </summary>
        /// <returns>The byte array with the header.</returns>
        public byte[] GetFullData()
        {
            this.data.AddFirst(header);
            byte[] result = this.data.ToArray();
            this.data.RemoveFirst();
            return result;
        }

        /// <summary>
        /// Gets the data in this packet, excluding the header.
        /// </summary>
        /// <returns>The data without the header.</returns>
        public byte[] GetData()
        {
            return this.data.ToArray();
        }
    }
}
