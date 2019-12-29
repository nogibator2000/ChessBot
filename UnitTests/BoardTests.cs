using System;
using Xunit;
using Custom_Chess_Bot;
namespace UnitTests
{
    public class BoardTests
    {
        [Fact]
        public void CreateEmptyBoard()
        {
            var board = new ChessBoard();
            Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.GetFen());
        }
        [Fact]
        public void NullTest()
        {
            var board = new ChessBoard();
            Assert.False(board.TurnIn(null));
        }
        [Fact]
        public void MakeTurn()
        {
            var board = new ChessBoard();
            Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", board.GetFen());
            board.TurnIn(new Turn(8, 24, new Side(true)));
            Assert.Equal("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR b KQkq - 0 1", board.GetFen());
            board.TurnIn(new Turn(54, 38, new Side(false)));
            Assert.Equal("rnbqkbnr/pppppp1p/8/6p1/P7/8/1PPPPPPP/RNBQKBNR w KQkq - 0 2", board.GetFen());
            board.TurnIn(new Turn(32, 24, new Side(true)));
            Assert.Equal("rnbqkbnr/pppppp1p/8/P5p1/8/8/1PPPPPPP/RNBQKBNR b KQkq - 0 2", board.GetFen());
            board.TurnIn(new Turn(30, 38, new Side(false)));
            Assert.Equal("rnbqkbnr/pppppp1p/8/P7/6p1/8/1PPPPPPP/RNBQKBNR w KQkq - 0 3", board.GetFen());
            board.TurnIn(new Turn(15 + 16, 15, new Side(true)));
            Assert.Equal("rnbqkbnr/pppppp1p/8/P7/6pP/8/1PPPPPP1/RNBQKBNR b KQkq - 0 3", board.GetFen());
            board.TurnIn(new Turn(7 + 16, 30, new Side(false)));
            Assert.Equal("rnbqkbnr/pppppp1p/8/P7/8/7p/1PPPPPP1/RNBQKBNR w KQkq - 0 4", board.GetFen());
            board.TurnIn(new Turn(14, 23, new Side(true)));
            board.TurnIn(new Turn(49, 41, new Side(false)));
            board.TurnIn(new Turn(41, 32, new Side(true)));
            Assert.Equal("rnbqkbnr/p1pppp1p/1P6/8/8/7P/1PPPPP2/RNBQKBNR b KQkq - 0 5", board.GetFen());
        }
        [Fact]
        public void MakeOO()
        {
            var board = new ChessBoard("r3k2r/p1pppp1p/8/8/8/8/1PPPPP2/R3K2R w KQkq - 1 1");
            Assert.Equal("r3k2r/p1pppp1p/8/8/8/8/1PPPPP2/R3K2R w KQkq - 1 1", board.GetFen());
            board.TurnIn(new Turn(4, 6, new Side(true)));
            board.TurnIn(new Turn(60, 58, new Side(false)));
            Assert.Equal("2kr3r/p1pppp1p/8/8/8/8/1PPPPP2/R4RK1 w - - 1 2", board.GetFen());
            board = new ChessBoard("r3k2r/p1pppp1p/8/8/8/8/1PPPPP2/R3K2R w KQkq - 1 1");
            board.TurnIn(new Turn(4, 2, new Side(true)));
            board.TurnIn(new Turn(60, 62, new Side(false)));
            Assert.Equal("r4rk1/p1pppp1p/8/8/8/8/1PPPPP2/2KR3R w - - 1 2", board.GetFen());
        }
        [Fact]
        public void ExtractMoves()
        {
            var board = new ChessBoard();
            board.TurnIn(new Turn(8, 24, new Side(true)));
            board.TurnIn(new Turn(54, 38, new Side(false)));
            board.TurnIn(new Turn(32, 24, new Side(true)));
            Assert.Equal("position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves a2a4 g7g5 a4a5 ", board.GetMoves());

        }
        [Fact]
        public void TestPromotion()
        {
            var board = new ChessBoard("8/1P6/4k3/8/4K3/8/p7/8 w - - 0 1");
            Assert.Equal("8/1P6/4k3/8/4K3/8/p7/8 w - - 0 1", board.GetFen());
            board.TurnIn(new Turn("b7b8q", new Side(Side.White)));
            Assert.Equal("1Q6/8/4k3/8/4K3/8/p7/8 b - - 0 1", board.GetFen());
            board.TurnIn(new Turn(0,8, new Side(Side.Black)));
            Assert.Equal("1Q6/8/4k3/8/4K3/8/8/q7 w - - 0 2", board.GetFen());
        }
    }
}
