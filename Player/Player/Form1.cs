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
        private int SeqForReciving = 0;
        private int SeqForSending = 0;
        private string PlayerName = "";
        private Socket sock = new Socket(
        AddressFamily.InterNetwork,
        SocketType.Dgram,
        ProtocolType.Udp
        );
        private Thread thread;
        private object mon = new object();
        private volatile bool running = false;
        private Player we;
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
            byte[] data = new byte[81];
            EndPoint addr = new IPEndPoint(0, 0);
            //sock.ReceiveTimeout = 600;
            /*try
            {*/
                sock.ReceiveFrom(data, ref addr);
            /*}
            catch
            {
                return;
            }
            */
            if (data.Length < 0)
            {
                return;
            }
            MemoryStream stream = new MemoryStream(data);
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int seq2 = reader.ReadInt32();
                int us = reader.ReadInt32();
                string name = reader.ReadString();
                if (seq2 >= SeqForReciving)
                {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    lock (mon) 
                    {
                        if (players.ContainsKey(name))
                        {
                            players[name].X = Interlocked.Increment(ref x);
                            players[name].Y = Interlocked.Increment(ref y);
                            if (us == 1)
                            {
                                we = players[name];
                            }
                        }
                        else
                        {
                            players.Add(name, new Player(x, y, this.Size.Width, this.Size.Height, name));
                            if(us == 1)
                            {
                                we = players[name];
                            } 
                        }
                        Console.WriteLine($"{players[name].X}, {we.X}");
                        foreach (KeyValuePair<string, Player> item in players)
                        {
                            Console.WriteLine($"{item.Value.X}, {item.Value.Y}");
                        }
                    }
                        SeqForReciving = Interlocked.Increment(ref seq2);
                }
            }
        }
      
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int x; int y;
            lock (mon)
            {
                x = we.X;
                y = we.Y;
            
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

                we.X = x;
                we.Y = y;
                players[we.Name] = we;
            }
            Send(sock, ref SeqForSending, x, y);
        }
        public void UpdateData()
        {
            while (running)
            {
                Receive(this.sock,ref this.SeqForReciving, players);
                Thread.Sleep(10);
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            //players.Add("",new Player(10, 10, this.Size.Width, this.Size.Height));
            thread = new Thread(new ThreadStart(UpdateData));
            thread.Start();
            running = true;
            Send(sock, ref SeqForSending, random.Next(0, 400), random.Next(0, 300));
        }
        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            lock (mon)
            {
                foreach (KeyValuePair<string, Player> item in players)
                {
                    var rect = new Rectangle(item.Value.X, item.Value.Y, 10, 10);
                    g.DrawEllipse(Pens.Red, rect);
                    g.FillEllipse(Brushes.Blue, rect);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
            System.Environment.Exit(1);
        }
    }
}
