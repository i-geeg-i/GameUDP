using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Send(ref Player player, Socket sock, ref List<Player> players)
        {
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(player.Seq);
                writer.Write(player.X);
                writer.Write(player.Y);
                for (int i = 0; i < 10; i++)
                {
                    writer.Write(players[i].X);
                    writer.Write(players[i].Y);
                }
            }
            byte[] dataToSend = stream.ToArray();

            sock.SendTo(dataToSend, player.Addr);
            Console.WriteLine($"Sent to {player.Addr}");
            player.Seq += 1;
        }
        static void Recive(Socket sock, ref List<Player> players)
        {
            bool ready = false;
            byte[] data = new byte[12];
            EndPoint addr = new IPEndPoint(0, 0);
            sock.ReceiveFrom(data, ref addr);
            for (int i = 0; i < players.Count; i++)
            {
                if(addr == players[i].Addr && !ready)
                {
                    MemoryStream stream = new MemoryStream(data);
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        int seq;
                         seq = reader.ReadInt32();
                        if (seq > players[i].Seq)
                        {
                            players[i].Seq = seq;
                            players[i].X = reader.ReadInt32();
                            players[i].Y = reader.ReadInt32();
                        }
                    }
                    ready = true;
                    break;
                }
                
            }
            if (!ready && players.Count <= 10)
            {
                MemoryStream stream = new MemoryStream(data);
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int seq;
                    seq = reader.ReadInt32();
                    players.Add(new Player(reader.ReadInt32(), reader.ReadInt32(), addr,seq));
                }
            }

            Console.WriteLine($"Recived from {addr}");

            return;

        }
        public void sendAll(ref List<Player> players, Socket sock)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                Send(ref player, sock, ref players);
            }
        }
        public void Time(ref object obj)
        {
            object[] objects = (object[])obj;

            List<Player> players = (List<Player>)objects[0];
            Socket socket = (Socket)objects[1];

            while (1 != 2)
            {
                sendAll(ref players, socket);
                Thread.Sleep(10);
            }

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Socket sock = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
                );
            List<Player> players = new List<Player>();
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint addr = new IPEndPoint(ip, 1337);
            sock.Bind(addr);
            
            while (1 != 2)
            {
                Recive(sock, ref players);
                
            }
        }
        // Recive = seq + x + y
        
    }
}
