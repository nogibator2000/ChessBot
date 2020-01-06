using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{

    public partial class ChessBoard
    {
        private readonly Figures Figure;
        private const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private const string FenSeparator = "/";
        private const string FenSpace = " ";
        private const string PositionFenWord = "position fen";
        private const string MovesWord = "moves";
        private enum FenSpliter : int { fen, side, oo, idkwtf, halfTurn, turn }
        private List<List<string>> Cells;
        private string Moves;
        public Side SideToMove;
        private string IDKWTF;
        private int HalfTurns;
        private int Turns;
        private OO OOEnable;
        private string Fen;
        public ChessBoard(List<List<string>> cells, Side side)
        {
            Cells = cells;
            SideToMove = side;
            OOEnable = new OO();
            IDKWTF = "-";
            HalfTurns = 0;
            Turns = 1;
        }
        public ChessBoard(string fen = StartFen)
        {
            Moves = "";
            Figure = new Figures(this);
            Fen = fen;
            var splitedFenBySpace = fen.Split(FenSpace);
            var splitedFen = splitedFenBySpace[Convert.ToInt32(FenSpliter.fen)].Split(FenSeparator);
            SideToMove = new Side(splitedFenBySpace[Convert.ToInt32(FenSpliter.side)]);
            OOEnable = new OO(splitedFenBySpace[Convert.ToInt32(FenSpliter.oo)]);
            IDKWTF = splitedFenBySpace[Convert.ToInt32(FenSpliter.idkwtf)];
            HalfTurns = Convert.ToInt32(splitedFenBySpace[Convert.ToInt32(FenSpliter.halfTurn)]);
            Turns = Convert.ToInt32(splitedFenBySpace[Convert.ToInt32(FenSpliter.turn)]);
            Cells = new List<List<string>>();
            for (var i = SettingsStore.BoardLenght-1; i >= 0 ; i--)
            {
                var fenRow = splitedFen[i];
                var row = new List<string>();
                for (var j = 0; j < SettingsStore.BoardLenght; j++)
                {
                    if (int.TryParse(fenRow[0].ToString(), out int result))
                    {
                        row.Add(Figures.Space);
                        fenRow = fenRow.Substring(1);
                        if (result > 1)
                            fenRow = (result - 1).ToString() + fenRow;
                    }
                    else
                    {
                        row.Add(Figure.Key(fenRow[0].ToString()));
                        fenRow = fenRow.Substring(1); 
                    }
                }
                Cells.Add(row);
            }
        }
        public string GetMoves()
        {
            return PositionFenWord + FenSpace + Fen + FenSpace + MovesWord + FenSpace + Moves;
        }
        public static string GetFenS(List<List<string>> cells)
        {
            string fen = "";
            for (int rank = cells.Count() - 1; rank >= 0; rank--)
            {
                int empty = 0;
                string rankFen = "";
                for (int file = 0; file < cells[rank].Count(); file++)
                {
                    if (cells[rank][file] == Figures.Space)
                    {
                        empty++;
                    }
                    else
                    {
                        if (empty != 0)
                            rankFen += empty;
                        rankFen += cells[rank][file];
                        empty = 0;
                    }
                }
                if (empty != 0) rankFen += empty;
                fen += rankFen;
                if (rank != 0)
                {
                    fen += FenSeparator;
                }
            }
            return fen;
        }
        public string GetFen()
        {
            return GetFenS(Cells) + FenSpace + SideToMove + FenSpace + OOEnable + FenSpace + IDKWTF + FenSpace + HalfTurns + FenSpace + Turns;
        }
        /// <summary>Method <c>Turn</c> make turn and return true if it valid or return false.</summary>
        public bool TurnIn(Turn turn)
        {
            if (turn is null)
                return false;
            if (turn.TurnSide is null)
                turn.ApplySide(SideToMove);
            Figure.SwapCheck(turn);
            Figure.PromotionCheck(turn);
            //           Figure.LegalValidate(turn);
//            if (turn.TurnSide.ToString() != SideToMove.ToString() || !turn.Valid)
            if (!turn.Valid)
               return false;
            MakeTurn(turn);
            Moves += turn + FenSpace;
            SideToMove.Switch();
            if (SideToMove == Side.White)
                Turns += 1;
            return true;
        }
        private void MakeTurn(Turn turn)
        {
            turn.ToInt(out int wordStart, out int numStart, out int wordEnd, out int numEnd);
            var fgr = Cells[numStart][wordStart];
            OOEnable.Change(fgr, turn.Start);
            if (fgr == Figures.WhitePawn && numEnd == 5 && Cells[numEnd][wordEnd] == Figures.Space)
            {
                Cells[numEnd-1][wordEnd] = Figures.Space;
            }
            if (fgr == Figures.BlackPawn && numEnd == 2 && Cells[numEnd][wordEnd] == Figures.Space)
            {
                Cells[numEnd+1][wordEnd] = Figures.Space;
            }
            if (turn.Promote)
                fgr = SideToMove == Side.White ? Figures.WhiteQueen : Figures.BlackQueen;

            if (Figure.OOTurn(turn, out Turn extra))
            {
                MakeTurn(extra);
            }
            Cells[numStart][wordStart] = Figures.Space;
            Cells[numEnd][wordEnd] = fgr;
        }
    }
}
