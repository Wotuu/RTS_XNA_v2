using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SocketLibrary.Packets;
using SocketLibrary.Protocol;

public delegate void OnPacketProcessListener(Packet p);
namespace SocketLibrary.Packets
{
    public class PacketProcessor
    {
        private LinkedList<PacketReceiverPair> receivedPackets = new LinkedList<PacketReceiverPair>();
        private LinkedList<PacketReceiverPair> buggedPackets = new LinkedList<PacketReceiverPair>();
        private LinkedList<PacketSenderPair> sentPackets = new LinkedList<PacketSenderPair>();
        private readonly object syncSentPackets;

        public Boolean isRunning { get; set; }
        public OnPacketProcessListener onProcessPacket { get; set; }

        private const int packetResendTimeoutMS = 1000;


        public PacketProcessor()
        {
            syncSentPackets = new object();
        }

        /// <summary>
        /// Confirms a packet ID.
        /// </summary>
        /// <param name="packetID">The packet ID.</param>
        private void ConfirmPacket(int packetID)
        {
            lock (syncSentPackets)
            {
                PacketSenderPair toRemove = new PacketSenderPair();
                foreach (PacketSenderPair p in this.sentPackets)
                {
                    if (p.packet.GetPacketID() == packetID) { toRemove = p; break; }
                }
                this.sentPackets.Remove(toRemove);
                // Console.Out.WriteLine("Packet " + packetID + " was confirmed. Left to confirm: " + this.sentPackets.Count);
            }
        }

        /// <summary>
        /// Called when the packet is sent.
        /// </summary>
        /// <param name="p">The packet that was sent.</param>
        public void SentPacket(Packet p, SocketClient client)
        {
            lock (syncSentPackets)
            {
                sentPackets.AddLast(new PacketSenderPair(p, client));
            }
        }

        /// <summary>
        /// Spawns a thread to start processing packets
        /// </summary>
        public void StartProcessing()
        {
            if (!isRunning)
            {
                isRunning = true;
                new Thread(ProcessQueue).Start();
            }
        }

        /// <summary>
        /// Kills the thread, making it stop processing packets!
        /// </summary>
        public void StopProcessing()
        {
            isRunning = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullData"></param>
        public void QueuePacket(byte[] fullData, SocketClient receiver)
        {
            PacketReceiverPair pair = PacketReceiverPair.empty;
            try
            {
                this.receivedPackets.AddLast((pair = new PacketReceiverPair(fullData, receiver)));
            }
            catch (NullReferenceException e)
            {
                Console.Error.WriteLine("QueuePacket got a nullpointer!");
                // if (pair.Equals( null )) Console.Error.WriteLine("Pair was null");
                if (fullData == null) Console.Error.WriteLine("fullData was null!");
                if (receiver == null) Console.Error.WriteLine("receiver was null!");
            }
        }

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <returns>The string</returns>
        private String BytesToString(byte[] data)
        {
            char[] chars = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                chars[i] = (char)data[i];
            }
            return new String(chars);
        }
        /// <summary>
        /// Converts a string to bytes.
        /// </summary>
        /// <param name="str">The String to convert.</param>
        /// <returns>The byte array.</returns>
        private byte[] StringToBytes(String str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Constructs a packet from a full byte array.
        /// </summary>
        /// <param name="fullData">The data as received by the socket.</param>
        /// <returns>The packet, including header, packet ID, and the data.</returns>
        private Packet[] ConstructPacket(byte[] fullData)
        {
            String stringData = this.BytesToString(fullData);
            String[] split = stringData.Split((char)SocketClient.END_OF_PACKET);
            Packet[] packets = new Packet[split.Length - 1];

            if (split.Length > 2)
                Console.Out.WriteLine("Found a double packet! (full data: \n'" + stringData + "')");

            for (int i = 0; i < split.Length - 1; i++)
            {
                if (split[i].Contains((char)SocketClient.END_OF_PACKET))
                {
                    Console.Out.WriteLine("Packet contained end of packet char!");
                }
                // Extra catch for empty packets
                if (split[i].Length < 2) continue;
                byte[] currentPacketData = this.StringToBytes(split[i]);
                byte[] packetID = new byte[4];
                for (int j = 1; j < 5; j++)
                {
                    packetID[j - 1] = currentPacketData[j];
                }

                byte[] headerlessData = new byte[currentPacketData.Length - 5];
                for (int j = 5; j < currentPacketData.Length; j++)
                {
                    headerlessData[j - 5] = currentPacketData[j];
                }

                Packet p = new Packet(currentPacketData[0], headerlessData);
                p.SetPacketID(packetID);
                packets[i] = p;
            }
            return packets;
        }

        private void ProcessQueue()
        {
            Console.Out.WriteLine("Starting processing thread!");
            while (isRunning)
            {
                if (buggedPackets.Count != 0 && buggedPackets.Count % 100 == 0)
                {
                    Console.Out.WriteLine("Bugged packets is now " + buggedPackets.Count + ". Enjoy your exception");
                    int crash = Int32.Parse("LOL");
                }
                while (receivedPackets.Count > 0 && receivedPackets.First != null)
                {
                    if (onProcessPacket == null)
                    {
                        Console.Out.WriteLine("There are packets in the queue, but noone is listening to them!!");
                    }
                    else
                    {
                        //if (receivedPackets.First == null) continue;
                        try
                        {
                            // Process the packets
                            Packet[] receivedPacketArray = this.ConstructPacket(receivedPackets.First.Value.packet);
                            for (int i = 0; i < receivedPacketArray.Length; i++)
                            {
                                Packet receivedPacket = receivedPacketArray[i];
                                // Console.Out.WriteLine("Processing packet " + receivedPacket.GetHeader());
                                onProcessPacket(receivedPacket);

                                if (receivedPackets.First.Value.client.confirmPackets)
                                {
                                    if (receivedPacket.GetHeader() != Headers.PACKET_RECEIVED)
                                    {
                                        //Console.Out.WriteLine("Confirming packet with header " + receivedPacket.GetHeader() +
                                        //    " ID " + receivedPacket.GetPacketID());
                                        // Send a confirmation packet, that the packet is processed and received
                                        Packet confirmPacket = new Packet(Headers.PACKET_RECEIVED);
                                        confirmPacket.SetPacketID(receivedPacket.GetPacketID());
                                        receivedPackets.First.Value.client.SendPacket(confirmPacket);
                                    }
                                    else
                                    {
                                        ConfirmPacket(receivedPacket.GetPacketID());
                                    }
                                }
                            }
                            receivedPackets.RemoveFirst();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Nullpointer in PacketProcessor D=. Removing first to buggedPackets");
                            if (receivedPackets.First == null)
                            {
                                Console.WriteLine("First is null");
                            }
                            else
                            {
                                buggedPackets.AddLast(receivedPackets.First.Value);
                                receivedPackets.RemoveFirst();
                            }
                            // Stop for now, try again later
                            break;
                        }
                    }
                }
                lock (syncSentPackets)
                {
                    for (int i = 0; i < this.sentPackets.Count; i++)
                    {
                        PacketSenderPair pair = this.sentPackets.ElementAt(i);
                        if (pair.client.confirmPackets)
                        {
                            double now = (new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds);
                            if (now - pair.packet.timeSent > packetResendTimeoutMS)
                            {
                                Console.Out.WriteLine("Resending packet with ID " + pair.packet.GetPacketID() + " and header " +
                                    ((int)pair.packet.GetHeader()).ToString("x") /* +
                            //   ".. ( " + now + " - " + pair.packet.timeSent + " )"*/);
                                this.sentPackets.Remove(pair);
                                pair.client.SendPacket(pair.packet);
                                i--;
                            }
                        }
                    }
                }
                Thread.Sleep(10);
            }
            Console.Out.WriteLine("Ending processing thread!");
            isRunning = false;
        }

        struct PacketReceiverPair
        {
            public byte[] packet;
            public SocketClient client;
            public static PacketReceiverPair empty = new PacketReceiverPair(null, null);

            public PacketReceiverPair(byte[] packet, SocketClient client)
            {
                // if (packet == null) Console.WriteLine("PacketReceiverPair with null as packet incoming!");
                this.packet = packet;
                this.client = client;
            }
        }

        struct PacketSenderPair
        {
            public Packet packet;
            public SocketClient client;

            public PacketSenderPair(Packet packet, SocketClient client)
            {
                this.packet = packet;
                this.client = client;
            }
        }
    }
}
