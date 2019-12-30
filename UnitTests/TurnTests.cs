using System;
using Xunit;
using Custom_Chess_Bot;
namespace UnitTests
{
    public class TurnTests
    {
        [Fact]
        public void CreateTurnByInt()
        {
            var turn = new Turn(0, 8);
            Assert.Equal("a1a2", turn.ToString());
            turn = new Turn(63, 62, new Side(Side.White), true);
            Assert.Equal("h8g8q", turn.ToString());
        }
        [Fact]
        public void CreateTurnByStr()
        {
            var turn = new Turn("f1a1");
            var _turn = new Turn(5, 0);
            Assert.Equal("" + turn, "" + _turn);
            var turn1 = new Turn("a1a3");
            var _turn1 = new Turn(0, 16);
            Assert.Equal("" + turn1, "" + _turn1);
        }
        [Fact]
        public void TurnPromotion()
        {
            var _turn = new Turn(63, 55, null, true);
            Assert.Equal("h8h7q", "" + _turn);
        }
        [Fact]
        public void TurnSwap()
        {
            var turn = new Turn(0, 5);
            var _turn = new Turn(5, 0);
            turn.ApplySwap();
            Assert.Equal("" + turn, "" + _turn);
        }
        [Fact]
        public void TurnInverse()
        {
            var turn = new Turn(62, 63);
            var _turn = new Turn(1, 0);
            turn = turn.GetInverse();
            Assert.Equal("" + turn, "" + _turn);
        }
        [Fact]
        public void TurnEqual()
        {
            var turn = new Turn("h5d1");
            var _turn = new Turn("d1h5");
            Assert.True(turn == _turn);
        }
    }
}
