using System;
using System.Collections.Generic;
using System.Linq;

namespace Custom_Chess_Bot
{
    public partial class ChessBoard
    {
        public class OO
        {
            private bool OOWhiteE;
            private bool OOBlackE;
            private bool OOOWhiteE;
            private bool OOOBlackE;
            private const string Space = "-";
            private const string OOWHITE = "K";
            private const string OOOWHITE = "Q";
            private const string OOBLACK = "k";
            private const string OOOBLACK = "q";
            private const int BKPlace = 60;
            private const int BSRPlace = 63;
            private const int BLRPlace = 56;
            private const int WKPlace = 4;
            private const int WSRPlace = 7;
            private const int WLRPlace = 0;
            public void Change(string figure, int stPnt)
            {
                if (OOBlackE || OOOBlackE)
                {
                    if (stPnt == BKPlace && figure == Figures.BlackKing)
                    {
                        OOBlackE = false;
                        OOOBlackE = false;
                    }
                    if (stPnt == BSRPlace && figure == Figures.BlackRook)
                        OOBlackE = false;
                    if (stPnt == BLRPlace && figure == Figures.BlackRook)
                        OOOBlackE = false;
                }
                if (OOWhiteE || OOOWhiteE)
                {
                    if (stPnt == WKPlace && figure == Figures.WhiteKing)
                    {
                        OOWhiteE = false;
                        OOOWhiteE = false;
                    }
                    if (stPnt == WSRPlace && figure == Figures.WhiteRook)
                        OOWhiteE = false;
                    if (stPnt == WLRPlace && figure == Figures.WhiteRook)
                        OOOWhiteE = false;
                }
            }
            public OO(string str):this(str.Contains(OOWHITE), str.Contains(OOOWHITE), str.Contains(OOBLACK), str.Contains(OOOBLACK)) { }
     
            public OO(bool OOWhite=true, bool OOOWhite=true, bool OOBlack=true, bool OOOBlack=true)
            {
                OOWhiteE = OOWhite;
                OOOWhiteE = OOOWhite;
                OOBlackE = OOBlack;
                OOOBlackE = OOOBlack;
            }
            public override string ToString()
            {
                var str = "";
                if (OOWhiteE)
                    str += OOWHITE;
                if (OOOWhiteE)
                    str += OOOWHITE;
                if (OOBlackE)
                    str += OOBLACK;
                if (OOOBlackE)
                    str += OOOBLACK;
                if (str == "")
                    str = Space;
                return str;
            }
        }
    }
}
