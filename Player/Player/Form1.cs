using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }
        int seq = 0;
        Socket sock = new Socket(
    AddressFamily.InterNetwork,
    SocketType.Dgram,
    ProtocolType.Udp
    );
        private PictureBox pictureBox= new PictureBox();
        private List<Players> players = new List<Players>();
        private Font fnt = new Font("Arial", 10);
        Thread thread;
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
        static void Recive(Socket sock, ref int seq, ref List<Players> players)
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
        public void Time()
        {

            
            while (1 == 1)
            {
                
                Recive(sock,ref seq, ref players);
                this.Invoke((MethodInvoker)delegate {
                    this.Refresh();
                });

                //players[0].X += 1;
                //players[0].X = 500;
                
                Thread.Sleep(10);
                Console.WriteLine("C");
            }


        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.D)
            {
                players[0].X += 1;
            }
            else if (e.KeyCode == Keys.A)
            {
                players[0].X -= 1;
            }
            else if (e.KeyCode == Keys.W)
            {
                players[0].Y -= 1;
            }
            else if (e.KeyCode == Keys.S)
            {
                players[0].Y += 1;
            }
            Send(sock, ref seq, players[0].X,players[0].Y);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.BackColor = Color.White;
            pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.Controls.Add(pictureBox);
            players.Add(new Players(10, 10, this.Size.Width, this.Size.Height));

            players.Add(new Players(100, 100, this.Size.Width, this.Size.Height));
            Send(sock, ref seq, players[0].X, players[0].Y);
            thread = new Thread(new ThreadStart(Time));
            thread.Start();
        }
        
        private void pictureBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {          
            Graphics g = e.Graphics;
            List<Rectangle> ListOfRectangles = new List<Rectangle>();
            //Thread.Sleep(5000);
            for (int i = 0; i < players.Count; i++)
            {
                ListOfRectangles.Add(new Rectangle(players[i].X, players[i].Y, 10, 10));
                g.DrawEllipse(System.Drawing.Pens.Red, ListOfRectangles[i]);
                g.FillEllipse(System.Drawing.Brushes.Blue, ListOfRectangles[i]);
            }
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            thread.Join();
        }

    }
}
