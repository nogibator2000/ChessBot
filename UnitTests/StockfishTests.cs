using System;
using Xunit;
using Custom_Chess_Bot;
using System.IO;

namespace UnitTests
{
    public class StockfishTests
    {
        [Fact]
        public void CheckEngineByMoves()
        {
            var set = new SettingsStore();
            var sf = new ChessEngine(set.EnginePath);
            var board = new ChessBoard();
            var turn = sf.Query(board.GetMoves());
            Assert.True("e2e4" == turn + ""|| "g1f3" == turn + "");
            turn.ApplySide(new Side(true));
            Assert.True(board.TurnIn(turn));
            turn = sf.Query(board.GetMoves());
            Assert.Equal("e7e5", turn + "");
            sf.Dispose();
        }
        [Fact]
        public void CheckEngineByFen()
        {
            var set = new SettingsStore();
            var sf = new ChessEngine(set.EnginePath);
            var board = new ChessBoard();
            var turn = sf.Query(board.GetFen());
            Assert.Equal("e2e4", turn + "");
            sf.Dispose();
        }
    }
}
