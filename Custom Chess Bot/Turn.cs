using System;
using System.Collections.Generic;
using System.Linq;

namespace Custom_Chess_Bot
{
    public class Turn
    {
        private static readonly Dictionary<string, int> Symbols = new Dictionary<string, int>()
        { { "a", 0 }, { "b", 1 }, { "c", 2 }, { "d", 3 }, { "e", 4 }, { "f", 5 }, { "g", 6 }, { "h", 7 } };
        public int Start;
        public int End;
        public Side TurnSide;
        public bool Promote;
        private const string TransformSimbol = "q";
        public bool Valid;

        public Turn(int start, int end, Side side=null, bool promote = false)
        {
            Valid = true;
            TurnSide = side;
            Promote = promote;
            Start = start;
            End = end;
        }
        public Turn(string turn, Side side=null)
        {
            try
            {
                Valid = true;
                TurnSide = side;
                var startSym = Symbols[turn[0].ToString()];
                var startNum = Convert.ToInt32(turn[1].ToString()) - 1;
                var endSym = Symbols[turn[2].ToString()];
                var endNum = Convert.ToInt32(turn[3].ToString()) - 1;
                Start = startNum * SettingsStore.BoardLenght + startSym;
                End = endNum * SettingsStore.BoardLenght + endSym;
                if (turn.Length > 4)
                {
                    Promote = true;
                }
                else
                {
                    Promote = false;
                }
            }
            catch
            {
                Valid = false;
            }
        }
        public void ApplyPromotion()
        {
            Promote = true;
        }
        public void ApplySide(Side side)
        {
            TurnSide = side;
        }
        public Turn GetInverse()
        {
            return new Turn(63 - Start, 63 - End, TurnSide, Promote);
        }
        public void ApplySwap()
        {
            var i = Start;
            Start = End;
            End = i;
        }
        public Turn GetSwap()
        {
            return new Turn(End, Start, TurnSide, Promote);
        }
        public void Invalidate()
        {
            Valid = false;
        }

        public override string ToString()
        {
            var start = Symbols.FirstOrDefault(x => x.Value == Start % SettingsStore.BoardLenght).Key + (1+Start / SettingsStore.BoardLenght);
            var end = Symbols.FirstOrDefault(x => x.Value == End % SettingsStore.BoardLenght).Key + (1+End / SettingsStore.BoardLenght);
            var m = "";
            if (Promote)
                m += TransformSimbol;
            return start+end+m;
        }
        public void ToInt(out int wordStart, out int numStart, out int wordEnd, out int numEnd)
        {
            wordStart = Start % SettingsStore.BoardLenght;
            numStart = Start / SettingsStore.BoardLenght;
            wordEnd = End % SettingsStore.BoardLenght;
            numEnd = End / SettingsStore.BoardLenght;
        }
        public static bool operator ==(Turn turn, Turn _turn)
        {
            if (turn == null)
            {
                if (_turn == null)
                    return true;
                return false;
            }
            return (turn.Start==_turn.Start||turn.Start==_turn.End)&&(turn.End==_turn.End||turn.End==_turn.Start);
        }
        public static bool operator !=(Turn turn, Turn _turn)
        {
            return !(turn==_turn);
        }
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            return ReferenceEquals(this, obj) ? true : obj.GetType() == GetType() && (Turn)obj == this;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Start*End;
            }
        }
    }
}
