using System;

namespace Custom_Chess_Bot
{
    class Turn
    {
        public Turn transform()
        {
            return new Turn(end, start, side);
        }
        private readonly static Settings settings = new Settings();
        public Turn(bool _valid)
        {
            valid = _valid;
        }
        public Turn(bool isShort ,bool _side)
        {
            valid = true;
            side = _side;
            if (isShort)
            {
                oo = true;
                ooo = false;
            }
            else
            {
                ooo = true;
                oo = false;
            }
        }
        public Turn(int _start, int _end, bool _side)
        {
            if (start >= 0 && start < 64 && end >= 0 && end < 64)
                valid = true;
            else
                valid = false;
            side = _side;
            start = _start;
            end = _end;
            oo = false;
            ooo = false;
        }
        public bool oo;
        public bool ooo;
        public readonly bool valid;
        public int start;
        public int end;
        public bool side;
        public Turn Inverse()
        {
            return new Turn(63 - start, 63 - end, side);
        }
        public int GetNumStart()
        {
            return start % 8;
        }
        public int GetSymStart()
        {
            return start / 8;
        }
        public int GetNumEnd()
        {
            return end % 8;
        }
        public int GetSymEnd()
        {
            return end / 8;
        }
        public string GetStr()
        {
            if (valid)
            {
                if (oo && side)
                    return "e1c1";
                if (ooo && side)
                    return "e1g1";
                if (oo && !side)
                    return "e8g8";
                if (ooo && !side)
                    return "e8c8";
                return Settings.syms[GetNumStart()] + (8 - GetSymStart()) + Settings.syms[GetNumEnd()] + (8 - GetSymEnd());
            }
            return "Not valid.";
        }
    }
}
