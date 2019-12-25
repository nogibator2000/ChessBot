using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    class Board
    {

        private readonly int[] titles;
        public bool WhiteOO;
        public bool WhiteOOO;
        public bool BlackOO;
        public bool BlackOOO;
        const string RANK_SEPARATOR = "/";
        public string moves = "";
        public string translateBoardToFEN(bool side)
        {
            string _side = " w";
            if (!side) _side = " b";
            var board = GetTitles();
            string fen = "";
            for (int rank = 0; rank < board.Count(); rank++)
            {
                int empty = 0;
                string rankFen = "";
                for (int file = 0; file < board[rank].Count(); file++)
                {
                    if (board[rank][file] == "")
                    {
                        empty++;
                    }
                    else
                    {
                        // add the number to the fen if not zero.
                        if (empty != 0) rankFen += empty;
                        // add the letter to the fen
                        rankFen += board[rank][file];
                        // reset the empty
                        empty = 0;
                    }
                }
                // add the number to the fen if not zero.
                if (empty != 0) rankFen += empty;
                // add the rank to the fen
                fen += rankFen;
                // add rank separator. If last then add a space
                if (!(rank == board.Count() - 1))
                {
                    fen += RANK_SEPARATOR;
                }
                else
                {
                    fen += " ";
                }
            }
            var o = " ";
            if (WhiteOO)
                o += "K";
            if (WhiteOOO)
                o += "Q";
            if (BlackOO)
                o += "k";
            if (BlackOOO)
                o += "q";
            return fen +_side+o;
        }

        public List<List<string>> GetTitles()
        {
            var _var = new List<List<string>>();
            for (var i = 0; i < 8; i++)
            {
                var __var = new List<string>();
                for (var j = 0; j < 8; j++)
                {
                    var str = (titles[i * 8 + j]) switch
                    {
                        1 => "r",
                        -1 => "R",
                        2 => "n",
                        -2 => "N",
                        3 => "b",
                        -3 => "B",
                        4 => "q",
                        -4 => "Q",
                        5 => "k",
                        -5 => "K",
                        6 => "p",
                        -6 => "P",
                        _ => "",
                    };
                    
                    __var.Add(str);
                }
                _var.Add(__var);
            }
            return _var;
        }
        public Board()
        {
            WhiteOO = true;
            WhiteOOO = true;
            BlackOO = true;
            BlackOOO = true;
            titles = new int[64];
            titles[0] = 1;
            titles[7] = 1;
            titles[1] = 2;
            titles[6] = 2;
            titles[2] = 3;
            titles[5] = 3;
            titles[3] = 4;
            titles[4] = 5;
            titles[8] = 6;
            titles[9] = 6;
            titles[10] = 6;
            titles[11] = 6;
            titles[12] = 6;
            titles[13] = 6;
            titles[14] = 6;
            titles[15] = 6;
            titles[63 - 0] = -1;
            titles[63 - 7] = -1;
            titles[63 - 1] = -2;
            titles[63 - 6] = -2;
            titles[63 - 2] = -3;
            titles[63 - 5] = -3;
            titles[63 - 3] = -5;
            titles[63 - 4] = -4;
            titles[63 - 8] = -6;
            titles[63 - 9] = -6;
            titles[63 - 10] = -6;
            titles[63 - 11] = -6;
            titles[63 - 12] = -6;
            titles[63 - 13] = -6;
            titles[63 - 14] = -6;
            titles[63 - 15] = -6;
        }
        public void MakeOO(Turn turn)
        {
            moves += turn.GetStr() + " ";
            if (turn.side && turn.oo)
            {
                WhiteOO = false;
                WhiteOOO = false;
                titles[60] = 0;
                titles[61] = -1;
                titles[62] = -5;
                titles[63] = 0;
            }else if (!turn.side && turn.oo)
            {
                BlackOO = false;
                BlackOOO = false;
                titles[4] = 0;
                titles[5] = 1;
                titles[6] = 5;
                titles[7] = 0;
            }else if (turn.side && turn.ooo)
            {
                WhiteOO = false;
                WhiteOOO = false;
                titles[60] = 0;
                titles[59] = -1;
                titles[58] = -5;
                titles[56] = 0;
            } else if (!turn.side && turn.ooo)
            {
                BlackOO = false;
                BlackOOO = false;
                titles[4] = 0;
                titles[3] = 1;
                titles[2] = 5;
                titles[0] = 0;
            }
        }
        public bool MakeTurn(Turn turn, bool side)
        {
            if (turn.oo || turn.ooo)
            {
                MakeOO(turn);
                return true;
            }
            if ((WhiteOO || WhiteOOO) && (turn.start == 60 || turn.end == 60))
            {
                WhiteOO = false;
                WhiteOOO = false;
            }
            if ((BlackOO || BlackOOO) && (turn.start == 4 || turn.end == 4))
            {
                BlackOO = false;
                BlackOOO = false;
            }
            if ((BlackOO) && (turn.start == 7 || turn.end == 7))
            {
                BlackOO = false;
            }
            if ((BlackOOO) && (turn.start == 0 || turn.end == 0))
            {
                BlackOOO = false;
            }
            if ((WhiteOO) && (turn.start == 63 || turn.end == 63))
            {
                WhiteOO = false;
            }
            if ((WhiteOOO) && (turn.start == 56 || turn.end == 56))
            {
                WhiteOOO = false;
            }
            if (titles[turn.start] == 5|| titles[turn.start]==-5|| titles[turn.end] == 5 || titles[turn.end] == -5)
            {
                if ((turn.start == 60 && turn.end == 58) || (turn.start == 58 && turn.end == 60))
                {
                    MakeOO(new Turn(false, true));
                    return true;
                }
                if ((turn.start == 60 && turn.end == 62) || (turn.start == 62 && turn.end == 60))
                {
                    MakeOO(new Turn(true, true));
                    return true;
                }
                if ((turn.start == 4 && turn.end == 6) || (turn.start == 6 && turn.end == 4))
                {
                    MakeOO(new Turn(true, false));
                    return true;
                }
                if ((turn.start == 4 && turn.end == 2) || (turn.start == 2 && turn.end == 4))
                {
                    MakeOO(new Turn(false, false));
                    return true;
                }
            }
            if ((titles[turn.start] > 0 && !side && titles[turn.end] <= 0) ||
                (titles[turn.start] < 0 && side && titles[turn.end] >= 0))
            {
                if ((titles[turn.start]==6|| titles[turn.start] == -6)&&(turn.end<8||turn.end>55))
                {
                    if (titles[turn.start]==6)
                    {
                        titles[turn.end] = 4;
                    }
                    else
                    {
                        titles[turn.end] = -4;
                    }
                    titles[turn.start] = 0;
                    moves += turn.GetStr() + "q ";
                    return false;
                }
                moves += turn.GetStr()+" ";
                titles[turn.end] = titles[turn.start];
                titles[turn.start] = 0;
                return true;
            }
            if ((titles[turn.end] > 0 && !side && titles[turn.start] <= 0) ||
                (titles[turn.end] < 0 && side && titles[turn.start] >= 0))
            {
                if ((titles[turn.end] == 6 || titles[turn.end] == -6) && (turn.start < 8 || turn.start > 55))
                {
                    if (titles[turn.end] == 6)
                    {
                        titles[turn.start] = 4;
                    }
                    else
                    {
                        titles[turn.start] = -4;
                    }
                    titles[turn.end] = 0;
                    moves += turn.transform().GetStr() + "q ";
                    return false;
                }
                moves += turn.transform().GetStr() + " ";
                titles[turn.start] = titles[turn.end];
                titles[turn.end] = 0;
                return true;
            }
            return true;
        }
    }
}
