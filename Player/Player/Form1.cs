﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Player
{
    public partial class Form1 : Form
    {
        private int seq = 0;
        private Socket sock = new Socket(
        AddressFamily.InterNetwork,
        SocketType.Dgram,
        ProtocolType.Udp
        );
        private List<Players> players = new List<Players>();
        public Form1()
        {
            InitializeComponent();
        }

        static void Send(Socket sock, ref int seq, int X, int Y)
        {
            MemoryStream stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(seq);
                writer.Write(X);
                writer.Write(Y);
            }
            byte[] dataToSend = stream.ToArray();
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint addr = new IPEndPoint(ip, 1337);

            sock.SendTo(dataToSend, addr);

            seq += 1;
        }
        static void Receive(Socket sock, ref int seq, ref List<Players> players)
        {
            byte[] data = new byte[92];
            EndPoint addr = new IPEndPoint(0, 0);
            sock.ReceiveFrom(data, ref addr);
            MemoryStream stream = new MemoryStream(data);
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int seq2;
                seq2 = reader.ReadInt32();
                if (seq2 > seq)
                {
                    players[0].X = reader.ReadInt32();
                    players[0].Y = reader.ReadInt32();
                    
                    for (int i = 1; i < 10; i++)
                    {

                        players[i].X = reader.ReadInt32();
                        players[i].Y = reader.ReadInt32();
                    }
                        seq = seq2;
                }
            }
        }
      
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                players[0].X += 50;
            }
            else if (e.KeyCode == Keys.A)
            {
                players[0].X -= 50;
            }
            else if (e.KeyCode == Keys.W)
            {
                players[0].Y -= 50;
            }
            else if (e.KeyCode == Keys.S)
            {
                players[0].Y += 50;
            }
            Send(sock, ref seq, players[0].X,players[0].Y);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            players.Add(new Players(10, 10, this.Size.Width, this.Size.Height));

            players.Add(new Players(100, 100, this.Size.Width, this.Size.Height));
            Send(sock, ref seq, players[0].X, players[0].Y);
        }
        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < players.Count; i++)
            {
                var rect = new Rectangle(players[i].X, players[i].Y, 100, 100);
                g.DrawEllipse(Pens.Red, rect);
                g.FillEllipse(Brushes.Blue, rect);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Receive(sock, ref seq, ref players);
            Refresh();
        }
    }
}
