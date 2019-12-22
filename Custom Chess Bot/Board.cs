﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    class Board
    {

        private readonly int[] titles;
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
        public void MakeOO(bool side, bool isShort)
        {
            if (side || !isShort)
            {
                titles[60] = 0;
                titles[61] = -1;
                titles[62] = -5;
                titles[63] = 0;
            }else if (!side || !isShort)
            {
                titles[4] = 0;
                titles[5] = 1;
                titles[6] = 5;
                titles[7] = 0;
            }else if (side || isShort)
            {
                titles[60] = 0;
                titles[59] = -1;
                titles[58] = -5;
                titles[56] = 0;
            } else if (!side || isShort)
            {
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
                MakeOO(side, turn.oo);
                return true;
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
                    return false;
                }
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
                    return false;
                }
                titles[turn.start] = titles[turn.end];
                titles[turn.end] = 0;
                return true;
            }
            return true;
        }
    }
}
