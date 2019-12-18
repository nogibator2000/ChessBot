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
    class PlayEngine
    {
        public CancellationTokenSource CancelToken = new CancellationTokenSource();

        private static readonly Settings settings = new Settings();
        public string log;
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public void MakeMouseClick(int X, int Y)
        {
            Cursor.Position = new Point(X, Y);
            Thread.Sleep(200);
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)X, (uint)Y, 0, 0);

        }

        public void PlayThread()
        {
            var ia = new ImageAnalysis();
            var side = ia.AmIWhite(ia.SliceTitles(ia.CaptureScreen()));
            var engine = new ChessEngine();
            if (side)
            {
                log = @"You are White!";
                var board = new Board();
                Task.Run(() =>
                {
                    while (!CancelToken.IsCancellationRequested)
                    {
                        Task.Run(async () =>
                        {
                            var _turn = await engine.NextMove(board.GetTitles(), true);
                            if (_turn.start != -2)
                            {
                                MadeMove(_turn);
                                var transform = board.MakeTurn(_turn, side);
                                if (!transform)
                                {
                                    MadeMove(new Turn(_turn.end, _turn.end));
                                }
                                File.AppendAllText(settings.LogPath, "[turn][me]: " + _turn.GetStr() + Environment.NewLine);
                                log = _turn.GetStr();
                            }
                            else CancelToken.Cancel();
                        }).Wait();
                        Thread.Sleep(400);
                        var slicedBoard = ia.SliceTitles(ia.CaptureScreen());
                        var turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()),side);
                        while (turn.start == -1 || turn.end == -1)
                        {
                            turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()), side);
                        }
                        Thread.Sleep(400);
                        turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()), side);
                        while (turn.start == -1 || turn.end == -1)
                        {
                            turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()), side);
                        }
                        board.MakeTurn(turn, !side);
                        log = turn.GetStr();
                        File.AppendAllText(settings.LogPath, "[turn][enemy]: " + turn.GetStr() + Environment.NewLine);

                    }
                });
            }
            else
            {
                log = @"You are Black!";
                var board = new Board();
                Task.Run(() =>
                {
                    while (!CancelToken.IsCancellationRequested)
                    {
                        var slicedBoard = ia.SliceTitles(ia.CaptureScreen());
                        var turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()), side);
                        while (!turn.valid)
                        {
                            Thread.Sleep(Settings.RefreshRate);
                            turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()), side);
                        }
                        Thread.Sleep(400);
                        turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()), side);
                        while (turn.start == -1 || turn.end == -1)
                        {
                            turn = ia.AnalizingTurn(slicedBoard, ia.SliceTitles(ia.CaptureScreen()), side);
                        }
                        board.MakeTurn(turn.Inverse(), !side);
                        log = turn.GetStr();
                        File.AppendAllText(settings.LogPath, "[turn][enemy]: " + turn.GetStr() + Environment.NewLine);
                        Task.Run(async () =>
                        {
                            var _turn = await engine.NextMove(board.GetTitles(), false);
                            if (_turn.start != -2)
                            {
                                MadeMove(_turn.Inverse());
                                _turn.Inverse();
                                var transform = board.MakeTurn(_turn, side);
                                if (!transform)
                                {
                                    MadeMove(new Turn(_turn.end, _turn.end));
                                }
                                File.AppendAllText(settings.LogPath, "[turn][me]: " + _turn.GetStr() + Environment.NewLine);
                                log = _turn.GetStr();
                            }
                            else CancelToken.Cancel();
                        }).Wait();
                        Thread.Sleep(400);

                    }
                });
            }
        }
        private void MadeMove(Turn turn)
        {

            MakeMouseClick(settings.BoardPosition.X + settings.BoardSize.Width / 8 * turn.GetNumStart() + settings.BoardSize.Width / 16, settings.BoardPosition.Y + settings.BoardSize.Height / 8 * turn.GetSymStart() + settings.BoardSize.Height / 16);
            var HumanDelay = new Random();
            Thread.Sleep(HumanDelay.Next(50, 250));
            Thread.Sleep(HumanDelay.Next(50, 250));
            if (HumanDelay.Next(1, 7) == 5)
            {
                Thread.Sleep(HumanDelay.Next(1000, 1500));
            }
            MakeMouseClick(settings.BoardPosition.X + settings.BoardSize.Width / 8 * turn.GetNumEnd() + settings.BoardSize.Width / 16, settings.BoardPosition.Y + settings.BoardSize.Height / 8 * turn.GetSymEnd() + settings.BoardSize.Height / 16);
        }
    }
}
