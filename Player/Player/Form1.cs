using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
        private Thread thread;
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
        void Receive(Socket sock, ref int seq, ref List<Players> players)
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
                    int x = reader.ReadInt32();
                    players[0].X = Interlocked.Increment(ref x);
                    int y = reader.ReadInt32();
                    players[0].Y = Interlocked.Increment(ref y);

                    for (int i = 1; i < 10; i++)
                    {
                        int j = reader.ReadInt32();
                        players[i].X = Interlocked.Increment(ref j);
                        int k = reader.ReadInt32();
                        players[i].Y = Interlocked.Increment(ref k);
                    }
                        seq = Interlocked.Increment(ref seq2);
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
        public void UpdateData()
        {
            while (1 == 1)
            {
                Socket sock = new Socket(
        AddressFamily.InterNetwork,
        SocketType.Dgram,
        ProtocolType.Udp
        );
                int seq = 0;
                List<Players> players = new List<Players>();
                this.Invoke((MethodInvoker)delegate
                {
                    sock = this.sock;
                    seq = this.seq;
                    players = this.players;
                    
                });
                Receive(sock, ref seq, ref players);
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            players.Add(new Players(10, 10, this.Size.Width, this.Size.Height));

            players.Add(new Players(100, 100, this.Size.Width, this.Size.Height));
            Send(sock, ref seq, players[0].X, players[0].Y);
            thread = new Thread(new ThreadStart(UpdateData));
            thread.Start();
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
            Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thread.Join();
        }
    }
}
