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
        private string PlayerName = "";
        private Socket sock = new Socket(
        AddressFamily.InterNetwork,
        SocketType.Dgram,
        ProtocolType.Udp
        );
        private Thread thread;
        private object mon = new object();

        private Dictionary<string, Player> players = new Dictionary<string, Player>();
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
            int seq2 = seq + 1;
            seq = Interlocked.Increment(ref seq2);
        }
        void Receive(Socket sock, ref int seq, Dictionary<string, Player> players)
        {
            byte[] data = new byte[154];
            EndPoint addr = new IPEndPoint(0, 0);
            sock.ReceiveFrom(data, ref addr);
            MemoryStream stream = new MemoryStream(data);
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int seq2;
                seq2 = reader.ReadInt32();
                string name = reader.ReadString();
                if (seq2 > seq)
                {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    lock (mon) 
                    {
                    if (players.ContainsKey(name))
                        {
                            players[name].X = Interlocked.Increment(ref x);
                            players[name].Y = Interlocked.Increment(ref y);
                        }
                        else
                        {
                            players.Add(name, new Player(x, y, this.Size.Width, this.Size.Height));
                        }
                    }
                    seq = Interlocked.Increment(ref seq2);
                }
            }
        }
      
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int x; int y;
            lock (mon)
            {
                x = players[PlayerName].X;
                y = players[PlayerName].Y;
            
                if (e.KeyCode == Keys.D)
                {
                    x += 10;
                }
                else if (e.KeyCode == Keys.A)
                {
                    x -= 10;
                }
                else if (e.KeyCode == Keys.W)
                {
                    y -= 10;
                }
                else if (e.KeyCode == Keys.S)
                {
                    y += 10;
                }

                players[PlayerName].X = x;
                players[PlayerName].Y = y;
            }
            Send(sock, ref seq, x, y);
        }
        public void UpdateData()
        {
            while (true)
            {
                Receive(this.sock,ref this.seq, players);
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            players.Add("",new Player(10, 10, this.Size.Width, this.Size.Height));
            Send(sock, ref seq, players[PlayerName].X, players[PlayerName].Y);
            thread = new Thread(new ThreadStart(UpdateData));
            thread.Start();
        }
        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            foreach (KeyValuePair<string, Player> item in players)
            {
                var rect = new Rectangle(players[item.Key].X, players[item.Key].Y, 10, 10);
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
