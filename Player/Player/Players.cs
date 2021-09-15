namespace Player
{
    public class Player
    {
        private int xm;
        private int ym;
        private int x;
        public string Name { get; }
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if (value < xm && value > 0)
                {
                    x = value;
                }
                else if(value <= 0)
                {
                    x +=10;
                }
                else if (value >= xm)
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
                if (value < ym && value > 0)
                {
                    y = value;
                }
                else if (value <= 0)
                {
                    y += 10;
                }
                else if (value >= xm)
                {
                    y -= 10;
                }
            }
        }
        public Player(int X, int Y, int XM, int YM, string Name)
        {
            xm = XM;
            ym = YM;
            this.X = X;
            this.Y = Y;
            this.Name = Name;

        }
    }
}