using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SocketLibrary.Packets;

public delegate void OnPacketProcessListener(Packet p);
namespace SocketLibrary.Packets
{
    public class PacketProcessor
    {
        private LinkedList<Packet> packets = new LinkedList<Packet>();
        public Boolean isRunning { get; set; }
        public OnPacketProcessListener onProcessPacket { get; set; }


        public PacketProcessor()
        {

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
        /// Queues a packet for processing.
        /// </summary>
        /// <param name="p">The packet</param>
        public void QueuePacket(Packet p)
        {
            this.packets.AddLast(p);
        }

        private void ProcessQueue()
        {
            Console.Out.WriteLine("Starting processing thread!");
            while (isRunning)
            {
                lock (packets)
                {
                    while (packets.Count > 0)
                    {
                        if (onProcessPacket == null)
                        {
                            Console.Out.WriteLine("There are packets in the queue, but noone is listening to them!!");
                        }
                        else
                        {
                            // Console.Out.WriteLine("Processing packet " + packets.First.Value.GetHeader());
                            onProcessPacket(packets.First.Value);
                            packets.Remove(packets.First.Value);
                        }
                    }
                }
                Thread.Sleep(10);
            }
            Console.Out.WriteLine("Ending processing thread!");
            isRunning = false;
        }
    }
}
