﻿using System;
using System.Net.Sockets;
using SocketLibrary.Packets;
using System.Collections;
using SocketLibrary.Protocol;
using System.Collections.Generic;


public delegate void OnDisconnectListeners();
public delegate void OnPacketSend(Packet p);
namespace SocketLibrary
{
    public class SocketClient
    {
        public Socket Sock;
        public string SocketName;
        private byte[] buff = new byte[131071];
        public bool Receiving = true;
        private readonly object SyncRecv;
        private readonly object SyncSend;
        public OnDisconnectListeners onDisconnectListeners { get; set; }
        public Logger log { get; set; }

        public Boolean confirmPackets = false;

        public PacketProcessor packetProcessor { get; set; }
        public OnPacketSend onPacketSendListeners { get; set; }

        public readonly static byte END_OF_PACKET = (byte)'|';

        public SocketClient(Socket _sock, string _socketname)
        {
            SocketName = _socketname;
            Sock = _sock;
            // Sock.NoDelay = true;
            SyncRecv = new object();
            SyncSend = new object();
            log = new Logger();
        }

        /// <summary>
        /// Enables the socket to accept packets
        /// </summary>
        public void Enable()
        {
            if (this.packetProcessor == null) packetProcessor = new PacketProcessor();
            packetProcessor.onProcessPacket += this.OnProcessPacket;
            packetProcessor.StartProcessing();

            lock (SyncRecv)
            {
                Console.Out.WriteLine("Starting receiving packets..");
                while (Receiving)
                {
                    if (Sock.Connected)
                    {
                        int size = 0;
                        try
                        {
                            size = Sock.Receive(buff);
                        }
                        catch (Exception e)
                        {
                            this.Disable();
                            break;
                        }
                        if (size == 0)
                        {
                            // Disable();
                            Show("Will not accept 'byte[i <= 0]'");
                            // Receiving = false;
                            // break;
                        }
                        else if (size < 131071)
                        {
                            byte[] data = new byte[size];
                            Array.Copy(buff, data, size);
                            this.packetProcessor.QueuePacket(data, this);
                        }
                        else
                        {
                            // Disable();
                            Show("Cannot accept 'bytes[i > 131071]'");
                            // Receiving = false;
                            // break;
                        }
                    }
                }
                if (onDisconnectListeners != null) onDisconnectListeners();
                Console.Out.WriteLine("Ending receiving packets..");
            }
            packetProcessor.StopProcessing();
        }

        /// <summary>
        /// Disables logging for this client, increasing performance.
        /// </summary>
        public void DisableLogging()
        {
            packetProcessor.onProcessPacket -= this.OnProcessPacket;
        }

        /// <summary>
        /// Packet processor has determined that the packet needed processing.
        /// </summary>
        /// <param name="p">The packet that needed processing</param>
        public void OnProcessPacket(Packet p)
        {
            log.Log(p, true);
        }

        /// <summary>
        /// Sends a packet to the Socket we're connected with.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void SendPacket(Packet packet)
        {
            lock (SyncSend)
            {
                if (Sock.Connected)
                {
                    try
                    {
                        byte[] data = packet.GetFullData();
                        Sock.Send(data, data.Length, SocketFlags.None);
                        packet.timeSent = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
                        log.Log(packet, false);
                        if (onPacketSendListeners != null) onPacketSendListeners(packet);

                        if (confirmPackets)
                        {
                            if (packet.GetHeader() != Headers.PACKET_RECEIVED)
                                this.packetProcessor.SentPacket(packet, this);
                        }
                        // Console.Out.WriteLine("Sent a packet with header " + packet.GetHeader() + " and ID " + packet.GetPacketID());
                    }
                    catch (Exception ex)
                    {
                        Show("Unable to sent a packet.");
                        Show(ex);
                    }
                }
                else
                    Show("Fail. You shouldn't be able to make it send a packet, without having a connection.");
            }
        }


        /// <summary>
        /// Gets the IP address of this client in string representation
        /// </summary>
        /// <returns>The String</returns>
        public String GetRemoteHostIP()
        {
            String result = "";
            try
            {
                result = this.Sock.RemoteEndPoint.ToString();
            }
            catch (ObjectDisposedException e)
            {
                result = "SOCKET DISPOSED";
            }
            return result;
        }

        /// <summary>
        /// Disables the socket client.
        /// </summary>
        public void Disable()
        {
            Receiving = false;
            Sock.Close();
        }

        public void Show(string Text)
        {
            Console.Write(SocketName + ": " + Text);
        }

        public void Show(Exception Text)
        {
            Console.Write(SocketName + " ERROR: " + Text);
        }
    }
}
