using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Custom_Chess_Bot
{
    class PlayEngine: IDisposable
    {
        public PlayEngine(CancellationTokenSource ct, Form1 form)
        {
            PlayThread(ct, form);
        }
        private static readonly Logger logger = new Logger();

        private static readonly Settings settings = new Settings();
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public void MakeMouseClick(int X, int Y)
        {
                Cursor.Position = new Point(X, Y);
                Thread.Sleep(10);
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)X, (uint)Y, 0, 0);
            Thread.Sleep(10);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)X, (uint)Y, 0, 0);
        }
        public Turn findEnemyTurn(bool side, CancellationTokenSource ct)
        {
            Turn turn;
            var slicedBoard = ImageAnalysis.SliceTitles(ImageAnalysis.CaptureScreen());
            do
            {
                Thread.Sleep(settings.RefreshRate);
                turn = ImageAnalysis.AnalizingTurn(slicedBoard, ImageAnalysis.SliceTitles(ImageAnalysis.CaptureScreen()), side);
                if (turn.valid)
                {
                    Thread.Sleep(settings.RefreshRate);
                    turn = ImageAnalysis.AnalizingTurn(slicedBoard, ImageAnalysis.SliceTitles(ImageAnalysis.CaptureScreen()), side);
                }
            } while (!turn.valid&&!ct.IsCancellationRequested);
            return turn;
        }
        public Turn enemyTurn(bool side, Board board, CancellationTokenSource ct)
        {
            var turn = findEnemyTurn(side, ct);
            board.MakeTurn(turn, side);
            logger.Log(Logger.Turn + Logger.Enemy, turn.GetStr());
            return turn;
        }
        public Turn myTurn(bool side, Board board, CancellationTokenSource ct, ChessEngine engine)
        {
            var _turn = engine.NextMove(board, side);
            if (!_turn.valid)
                ct.Cancel();
            if (!ct.IsCancellationRequested)
            {
                MadeMove(_turn, side);
                if (!board.MakeTurn(_turn, side))
                    MadeMove(_turn.end, side);//transform
            }
            logger.Log(Logger.Turn + Logger.Me, _turn.GetStr());
            return _turn;
        }
        public void PlayThread(CancellationTokenSource ct, Form1 form)
        {
            var engine = new ChessEngine();
            var side = ImageAnalysis.AmIWhite(ImageAnalysis.SliceTitles(ImageAnalysis.CaptureScreen()));
            var board = new Board();
            if (side)
            {
                form.Log(@"You are White!");
                while (!ct.Token.IsCancellationRequested)
                {
                    form.Log(myTurn(side, board, ct, engine).GetStr());
                    form.Log(enemyTurn(!side, board, ct).GetStr());
                }
            }
            else
            {
                form.Log(@"You are Black!");
                while (!ct.Token.IsCancellationRequested)
                {
                    form.Log(enemyTurn(!side, board, ct).GetStr());
                    form.Log(myTurn(side, board, ct, engine).GetStr());
                }
            }
            engine.Dispose();
        }
        private void MadeMove(int transformClick, bool side)
        {
            MadeMove(new Turn(transformClick, transformClick, side), side);
        }
        private void MadeMove(Turn _turn, bool side)
        {
            var turn = _turn;
            if (!side)
                turn = _turn.Inverse();
            var HumanDelay = new Random();
            MakeMouseClick(settings.BoardPosition.X + settings.BoardSize.Width / 8 * turn.GetNumStart() + settings.BoardSize.Width / 16, settings.BoardPosition.Y + settings.BoardSize.Height / 8 * turn.GetSymStart() + settings.BoardSize.Height / 16);
            Thread.Sleep(HumanDelay.Next(Convert.ToInt32(settings.HumanBeingDelayMin*0.7), Convert.ToInt32(settings.HumanBeingDelayMax * 0.7)));
            if (HumanDelay.Next(0, 9) == 0)
                Thread.Sleep(HumanDelay.Next(Convert.ToInt32(settings.HumanBeingDelayMin * 3), Convert.ToInt32(settings.HumanBeingDelayMax * 3)));
            MakeMouseClick(settings.BoardPosition.X + settings.BoardSize.Width / 8 * turn.GetNumEnd() + settings.BoardSize.Width / 16, settings.BoardPosition.Y + settings.BoardSize.Height / 8 * turn.GetSymEnd() + settings.BoardSize.Height / 16);
            Thread.Sleep(settings.AnimationDelay);
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
