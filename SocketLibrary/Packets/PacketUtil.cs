using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Packets
{
    public class PacketUtil
    {

        /// <summary>
        /// Decodes a packet to a string.
        /// </summary>
        /// <param name="p">The packet to decode.</param>
        /// <returns>The string containing the information that was contained in the packet. </returns>
        public static String DecodePacketString(Packet p, int index)
        {
            String result = "";
            byte[] data = p.GetData();
            for( int i = index; i < data.Length; i++){
                result += ((char)data[i]) + "";
            }
            return result;
        }

        /// <summary>
        /// Decodes a packet that should contain an int.
        /// </summary>
        /// <param name="p">The packet</param>
        /// <param name="index">The byte index to start searching for the int</param>
        /// <returns>The integer</returns>
        public static int DecodePacketInt(Packet p, int index)
        {
            return BitConverter.ToInt32(p.GetData(), index);
        }

        /// <summary>
        /// Converts a byte array with length 4 to an int.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>The result of the computation.</returns>
        public static int DecodeInt(byte[] bytes)
        {
            if (bytes.Length != 4) throw new ArgumentException("Ints are 4 bytes, not " + bytes.Length + " bytes.");
            else return BitConverter.ToInt32(bytes, 0);
        }
    }
}
