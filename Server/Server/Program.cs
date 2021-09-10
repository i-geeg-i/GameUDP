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
        static void Send(Player player, Socket sock, Player player1) 
        {
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(player.Seq); // int = 4
                writer.Write(player.Name); //string Размер строки = 4 + 4 + 4 + 2 + 2 * length = 14 + 2 * length = 14 + 2*64=14+128=142
                writer.Write(player.X); // int = 4
                writer.Write(player.Y); // int = 4
                                        // sum = 4 + 142 + 4 +4 = 154
            }
            byte[] dataToSend = stream.ToArray();
            sock.SendTo(dataToSend, player1.Addr);
            Console.WriteLine($"Sent to {player1.Addr}, {player.Name.Length}");
            player1.Seq += 1;
        }
        static void Recive(Socket sock, ref Dictionary<EndPoint, Player> players)
        {
            byte[] data = new byte[12];
            EndPoint addr = new IPEndPoint(0, 0);
            sock.ReceiveFrom(data, ref addr);
            Player pl;
            MemoryStream stream = new MemoryStream(data);
            if (players.ContainsKey(addr))
            {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        int seq;
                        seq = reader.ReadInt32();
                        if (seq > players[addr].Seq)
                        {
                            players[addr].Seq = seq;
                            players[addr].X = reader.ReadInt32();
                            players[addr].Y = reader.ReadInt32();
                            pl = players[addr];
                            sendAll(players, sock, pl);
                        }
                    }
            }
            else
            {
                MemoryStream stream2 = new MemoryStream(data);
                using (BinaryReader reader = new BinaryReader(stream2))
                {
                    int seq;
                    seq = reader.ReadInt32();
                    string name = Player.RandomString(64);
                    pl = (new Player(reader.ReadInt32(), reader.ReadInt32(), addr, name, seq));
                    players.Add(addr, pl);
                }
                sendAll(players, sock, pl);
                

            }
            Console.WriteLine($"Recived from {addr}");
        }
        static public void sendAll(Dictionary<EndPoint, Player> players, Socket sock, Player player)
        {
            foreach (KeyValuePair<EndPoint, Player> item in players)
            {
                Player pl = item.Value;
                Send(pl, sock, player);
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
            
            Dictionary<EndPoint, Player> players = new Dictionary<EndPoint, Player>();
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
