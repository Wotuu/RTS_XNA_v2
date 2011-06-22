using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SocketLibrary;
using SocketLibrary.Packets;

public delegate void OnClientConnected(SocketClient client);

namespace SocketLibrary
{
    public class SocketServer
    {
        public OnClientConnected onClientConnectedListeners { get; set; }
        private readonly Socket Sock;
        private readonly int Port;
        private readonly string SocketName;
        private bool Closing;

        public SocketServer(int _port, bool UDP, string _socketname)
        {
            Closing = false;
            Port = _port;
            SocketName = _socketname;
            Sock = !UDP ? new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) : new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);
            var IPEndP = new IPEndPoint(IPAddress.Any, Port);
            try
            {
                Sock.Bind(IPEndP);
            }
            catch (Exception ex)
            {
                Show("Unable to bind port " + Port);
                Show(ex);
            }
        }
        
        public void Enable()
        {
            Sock.Listen(100);
            while (!Closing)
            {
                Socket Connection;
                try
                {
                    Connection = Sock.Accept();
                }
                catch (Exception ex)
                {
                    Show("Unable to create a new connection with a newly created connection.");
                    Show(ex);
                    Sock.Close();
                    return;
                }
                if (Connection.Connected)
                {
                    if (onClientConnectedListeners != null)
                    {
                        SocketClient sock = new SocketClient(Connection, SocketName);
                        new Thread(sock.Enable).Start();
                        onClientConnectedListeners(sock);
                    }
                }
                else
                    Show("The Connection wasn't connected. Weird o_o.");
            }
        }

        public void Disable()
        {
            Sock.Close();
            Closing = true;
            Show("Closing.");
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
