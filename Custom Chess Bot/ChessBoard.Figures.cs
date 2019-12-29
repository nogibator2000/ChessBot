namespace Custom_Chess_Bot
{

    public partial class ChessBoard
    {
        public class Figures
        {
            private readonly ChessBoard Board;
            public Figures(ChessBoard board)
            {
                Board = board;
            }
            public string Key(string figure)
            {
                return figure;
            }
            public void LegalValidate(Turn turn)
            {
                //todo
            }
            public void SwapCheck(Turn turn, bool transformed = false)
            {
                turn.ToInt(out int wordStart, out int numStart, out int wordEnd, out int numEnd);
                if (!(turn.TurnSide == Side.Black && (Board.Cells[numStart][wordStart] == BlackPawn ||
                    Board.Cells[numStart][wordStart] == BlackBishop || Board.Cells[numStart][wordStart] == BlackKing ||
                    Board.Cells[numStart][wordStart] == BlackKnight || Board.Cells[numStart][wordStart] == BlackQueen ||
                    Board.Cells[numStart][wordStart] == BlackRook) || turn.TurnSide == Side.White &&
                    (Board.Cells[numStart][wordStart] == WhitePawn || Board.Cells[numStart][wordStart] == WhiteBishop ||
                    Board.Cells[numStart][wordStart] == WhiteKing || Board.Cells[numStart][wordStart] == WhiteKnight ||
                    Board.Cells[numStart][wordStart] == WhiteQueen || Board.Cells[numStart][wordStart] == WhiteRook)))
                {
                    if (!transformed)
                    {
                        turn.ApplySwap();
                        SwapCheck(turn, true);
                    }
                    else
                        turn.Invalidate();
                }
            }
            public bool OOTurn(Turn turn, out Turn extra)
            {
                turn.ToInt(out int wordStart, out int numStart, out int wordEnd, out int numEnd);
                if (Board.Cells[numStart][wordStart] == WhiteKing && numStart == 0 && wordStart == 4)
                {
                    if (wordEnd == 2 && numEnd == 0)
                    {
                        extra = new Turn(0, 3, new Side(Side.White));
                        return true;
                    }
                    if (wordEnd == 6 && numEnd == 0)
                    {
                        extra = new Turn(7, 5, new Side(Side.White));
                        return true;
                    }
                }
                if (Board.Cells[numStart][wordStart] == BlackKing && numStart == 7 && wordStart == 4)
                {
                    if (wordEnd == 2 && numEnd == 7)
                    {
                        extra = new Turn(56, 59, new Side(Side.Black));
                        return true;
                    }
                    if (wordEnd == 6 && numEnd == 7)
                    {
                        extra = new Turn(63, 61, new Side(Side.Black));
                        return true;
                    }
                }
                extra = null;
                return false;
            }
            public void PromotionCheck(Turn turn)
            {
                if (!turn.Promote)
                {
                    turn.ToInt(out int wordStart, out int numStart, out int wordEnd, out int numEnd);
                    if (numEnd == 0)
                    {
                        if (Board.Cells[numStart][wordStart] == BlackPawn)
                            turn.ApplyPromotion();
                    }
                    if (numEnd == 7)
                    {
                        if (Board.Cells[numStart][wordStart] == WhitePawn)
                            turn.ApplyPromotion();
                    }
                }
            }
            public const string Space = " ";
            public const string WhitePawn = "P";
            public const string WhiteRook = "R";
            public const string WhiteKnight = "N";
            public const string WhiteBishop = "B";
            public const string WhiteQueen = "Q";
            public const string WhiteKing = "K";
            public const string BlackPawn = "p";
            public const string BlackRook = "r";
            public const string BlackKnight = "n";
            public const string BlackBishop = "b";
            public const string BlackQueen = "q";
            public const string BlackKing = "k";
        }
    }
}
