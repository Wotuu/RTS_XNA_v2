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
        private byte[] packetID { get; set; }
        private LinkedList<byte> data = new LinkedList<byte>();

        /// <summary>
        /// The time in MS that this packet was sent to the receivers
        /// </summary>
        public double timeSent { get; set; }

        public int timesSent { get; set; }

        public static int CURRENT_PACKET_ID { get; set; }

        static Packet()
        {
            // CURRENT_PACKET_ID += new Random(DateTime.UtcNow.Millisecond).Next();
        }

        public Packet()
        {
            Packet.CURRENT_PACKET_ID++;
            this.packetID = BitConverter.GetBytes(CURRENT_PACKET_ID);
        }

        public Packet(byte header)
        {
            this.SetHeader(header);
            Packet.CURRENT_PACKET_ID++;
            this.packetID = BitConverter.GetBytes(CURRENT_PACKET_ID);
        }

        public Packet(byte header, byte[] data)
        {
            this.SetHeader(header);
            for (int i = 0; i < data.Length; i++)
            {
                this.data.AddLast(data[i]);
            }
            Packet.CURRENT_PACKET_ID++;
            this.packetID = BitConverter.GetBytes(CURRENT_PACKET_ID);
        }

        public Packet(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.data.AddLast(data[i]);
            }
            Packet.CURRENT_PACKET_ID++;
            this.packetID = BitConverter.GetBytes(CURRENT_PACKET_ID);
        }

        /// <summary>
        /// Sets the packet ID.
        /// </summary>
        /// <param name="packetID">The packet id.</param>
        public void SetPacketID(int packetID)
        {
            this.packetID = BitConverter.GetBytes(packetID);
        }

        /// <summary>
        /// Sets the packet ID. Must be a length of 4!.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public void SetPacketID(byte[] bytes)
        {
            this.packetID = bytes;
        }

        /// <summary>
        /// Gets the packetID of this packet.
        /// </summary>
        /// <returns>The int containing the ID of the packet.</returns>
        public int GetPacketID()
        {
            return PacketUtil.DecodeInt(this.packetID);
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
        /// <param name="s">The string</param>
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
            LinkedList<byte> fullData = new LinkedList<byte>();
            // Header first
            fullData.AddLast(this.header);
            foreach (byte b in this.packetID)
            {
                // Packet ID next
                fullData.AddLast(b);
            }

            foreach (byte b in this.data)
            {
                // Now the data we wanted to send
                fullData.AddLast(b);
            }

            fullData.AddLast(SocketClient.END_OF_PACKET);

            return fullData.ToArray();
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
