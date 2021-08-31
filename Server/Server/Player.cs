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
        private int xm;
        private int ym;
        private int x;
        public int X
        {
            get
            {
                return x;
            }
            set
            {

                    x = value;
                

            }
        }
        private int y;
        public int Y
        {
            get
            {
                return y;
            }
            set
            {

                    y = value;
                
            }
        }
        int seq;
        public int Seq
        {
            get
            {
                return seq;
            }
            set
            {

                seq = value;

            }
        }
        
        EndPoint addr;
        public EndPoint Addr
        {
            get
            {
                return addr;
            }
        }
        public Player(int X, int Y, EndPoint addr, int Seq = 0 )
        {
            x = X;
            y = Y;
            seq = Seq;
            this.addr = addr;
            xm = 500;
            ym = 300;
        }
    }
}
