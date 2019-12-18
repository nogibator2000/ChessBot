using System;

namespace Custom_Chess_Bot
{
    class Turn
    {
        private readonly static Settings settings = new Settings();
        public Turn(bool _valid)
        {
            if (!valid)
                valid = false;
            else throw new ArgumentException("should be false.");
        }
        public Turn(int _start, int _end)
        {
            if (start > 0 && start < 64 && end > 0 && end < 64)
                valid = true;
            else
                valid = false;
                start = _start;
            end = _end;
        }
        public readonly bool valid;
        public int start;
        public int end;
        public Turn Inverse()
        {
            if (valid) 
            { 
                start = 63 - start;
                end = 63 - end;
            }
            return this;
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
                return settings.syms[GetNumStart()] + (8 - GetSymStart()) + "-" + settings.syms[GetNumEnd()] + (8 - GetSymEnd());
            }
            return "Not valid.";
        }
    }
}
