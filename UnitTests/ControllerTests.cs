using System;
using Xunit;
using Custom_Chess_Bot;
using System.IO;

namespace UnitTests
{
    public class ControllerTests
    {
        [Fact]
        public void CheckController()
        {
            var set = new SettingsStore();
            set.BoardPosition = new System.Drawing.Point(0, 0);
            set.BoardSize = new System.Drawing.Size(80, 80);
            var c = new Controller(set);
            var str = c.SimulateMakeTurn(new Turn(0, 1));
            Assert.Equal("5, 75 15, 75 ", str);
        }
        [Fact]
        public void CheckControllerInverse()
        {
            var set = new SettingsStore();
            set.BoardPosition = new System.Drawing.Point(0, 0);
            set.BoardSize = new System.Drawing.Size(80, 80);
            var c = new Controller(set);
            var turn = new Turn(63, 62);
            turn = turn.GetInverse();
            var str = c.SimulateMakeTurn(turn);
            Assert.Equal("5, 75 15, 75 ", str);
        }
        [Fact]
        public void CheckControllerPromote()
        {
            var set = new SettingsStore();
            set.BoardPosition = new System.Drawing.Point(100, 100);
            set.BoardSize = new System.Drawing.Size(80, 80);
            var c = new Controller(set);
            var turn = new Turn(63, 62, null, true);
            var str = c.SimulateMakeTurn(turn);
            Assert.Equal("175, 105 165, 105 165, 105", str);
        }
    }
}
