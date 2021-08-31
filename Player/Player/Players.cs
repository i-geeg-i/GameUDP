namespace Player
{
    public class Players
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
                if (value < xm-30 && value > 0)
                {
                    x = value;
                }
                else if(value < xm - 30 && value <= 0)
                {
                    x+=10;
                }
                else if (value >= xm - 30 && value > 0)
                {
                    x -= 10;
                }
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
                if (value < ym - 50 && value > 0)
                {
                    y = value;
                }
                else if (value < ym - 50 && value <= 0)
                {
                    y += 10;
                }
                else if (value >= ym - 50 && value > 0)
                {
                    y -= 10;
                }
            }
        }
        public Players(int X, int Y, int XM, int YM)
        {
            this.X = X;
            this.Y = Y;
            xm = XM;
            ym = YM;
        }
    }
}