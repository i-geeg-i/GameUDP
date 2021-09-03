using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Player
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Seq { get; set; }
        public EndPoint Addr { get;}
        public string Name { get; }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public Player(int X, int Y, EndPoint addr, string name, int Seq = 0 )
        {
            this.X = X;
            this.Y = Y;
            this.Seq = Seq;
            this.Addr = addr;
            Name = name;
        }
    }
}
