namespace Custom_Chess_Bot
{
    public class Side
    {
        private bool BoolSide;
        public const string White = "w";
        public const string Black = "b";
        public Side(bool side)
        {
            BoolSide = side;
        }
        public Side(string side)
        {
            if (side.ToLower() == White)
            {
                BoolSide = true;
            }
            else
            if (side.ToLower() == Black)
            {
                BoolSide = false;
            }
            else
                throw new System.ArgumentException("Can not convert to side");
        }
        public void Switch()
        {
            BoolSide = !BoolSide;
        }
        public override string ToString()
        {
            if (BoolSide)
                return White;
            return Black;
        }
        public static bool operator ==(Side side, Side _side)
        {
            return side.BoolSide == _side.BoolSide;
        }
        public static bool operator !=(Side side, Side _side)
        {
            return side.BoolSide != _side.BoolSide;
        }
        public static bool operator ==(Side side, string str)
        {
            return side.ToString() == str;
        }
        public static bool operator !=(Side side, string str)
        {
            return side.ToString() != str;
        }
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return ReferenceEquals(this, obj) ? true : obj.GetType() == GetType() && (Side)obj == this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var i = 1;
                if (!BoolSide)
                    i = 0;
                return i;
            }
        }
    }
}