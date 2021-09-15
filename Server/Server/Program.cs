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
        static void Send(Player AboutPlayer, Socket sock, Player ToPlayer, ref int MainSeq) 
        {
            MemoryStream stream = new MemoryStream();
            int us = 0;
            if (AboutPlayer.Addr == ToPlayer.Addr)
            {
                us = 1;
            }
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(MainSeq); 
                writer.Write(us);
                writer.Write(AboutPlayer.Name); 
                writer.Write(AboutPlayer.X); 
                writer.Write(AboutPlayer.Y); 
            }
            byte[] dataToSend = stream.ToArray();
            sock.SendTo(dataToSend, ToPlayer.Addr);
            Console.WriteLine($"Sent to {ToPlayer.Addr}, {AboutPlayer.Name.Length}");
            MainSeq += 1;
        }
        static void Recive(Socket sock, ref Dictionary<EndPoint, Player> players, ref int MainSeq)
        {
            byte[] data = new byte[12];
            EndPoint addr = new IPEndPoint(0, 0);
            sock.ReceiveFrom(data, ref addr);
            Player recivedPlayer;
            MemoryStream stream = new MemoryStream(data);
            if (players.ContainsKey(addr))
            {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        int seq;
                        seq = reader.ReadInt32();
                        recivedPlayer = players[addr];
                        if (seq >= players[addr].Seq)
                        {
                            recivedPlayer.Seq = seq;
                            recivedPlayer.X = reader.ReadInt32();
                            recivedPlayer.Y = reader.ReadInt32();
                            Console.WriteLine($"Recived from {addr}, X = {recivedPlayer.X}");
                            sendAll(players, sock, recivedPlayer,ref MainSeq);
                            players[addr].Seq = seq;
                        }
                    }
            }
            else
            {
                MemoryStream stream2 = new MemoryStream(data);
                using (BinaryReader reader = new BinaryReader(stream2))
                {
                    int seq;
                    seq = reader.ReadInt32()+1;
                    string name = Player.RandomString(64);
                    recivedPlayer = (new Player(reader.ReadInt32(), reader.ReadInt32(), addr, name, seq));
                    players.Add(addr, recivedPlayer);
                }
                Console.WriteLine($"Recived from {addr}");
                Send(players[addr], sock, players[addr], ref MainSeq);
                sendAll(players, sock, recivedPlayer, ref MainSeq);
                

            }
            
        }
        static public void sendAll(Dictionary<EndPoint, Player> players, Socket sock, Player aboutPlayer,ref int MainSeq)
        {
            foreach (KeyValuePair<EndPoint, Player> item in players)
            {
                Send(aboutPlayer, sock, item.Value, ref MainSeq);
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
            int MainSeq = 0;
            Dictionary<EndPoint, Player> players = new Dictionary<EndPoint, Player>();
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint addr = new IPEndPoint(ip, 1337);
            sock.Bind(addr);
            while (1 != 2)
            {
                Recive(sock, ref players, ref MainSeq);
            }
        }
        // Recive = seq + x + y
        
    }
}
