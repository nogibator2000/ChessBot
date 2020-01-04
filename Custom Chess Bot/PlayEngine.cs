using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    class PlayEngine : IDisposable
    {
        Side MySide;
        ChessBoard Board;
        LogWriter Log;
        SettingsStore Settings;
        ChessEngine Engine;
        Controller AI;
        bool CustomGame;
        Turn DetectedTurn;

        private const int DefaultSkill = 20;
        private const int RetardedSkill = 1;
        public PlayEngine(string str=null)
        {
            if (str != null)
            {
                Board = new ChessBoard(str);
                CustomGame = true;
            }
            else
            {
                Board = new ChessBoard();
                CustomGame = false;
            }
            Settings = new SettingsStore();
            Log = new LogWriter(Settings);
            Engine = new ChessEngine(Settings.EnginePath);
            AI = new Controller(Settings);
        }
        const string GameDone = @"Game Done";
        private const int DelayTurns = 10;
        private const float DelayPart = 0.7f;

        private int CalcDelay(int minDelay, int maxDelay)
        {
            var rand = new Random();
            var delay =rand.Next(Convert.ToInt32(minDelay * DelayPart), Convert.ToInt32(maxDelay * DelayPart));
            if (rand.Next(1, DelayTurns + 1) == 1)
                delay+=rand.Next(Convert.ToInt32(minDelay * (1 - DelayPart) * DelayTurns), Convert.ToInt32(maxDelay * (1 - DelayPart) * DelayTurns));
            return delay;
        }

        public Turn DetectTurn(CancellationTokenSource ct)
        {
            var turn = ImageAnalysis.FindTurn(Log, Settings,ct);
            if (turn is null)
                return null;
            if (MySide == Side.Black)
                turn = turn.GetInverse();
            return turn;
        }
        public Turn MakeTurn(CancellationTokenSource ct, bool suggest)
        {
            var rand = new Random();
            var skill = DefaultSkill;
            if (rand.Next(1, Settings.MissplayTurns + 1) == Settings.MissplayTurns)
                skill = RetardedSkill;
            var delay = CalcDelay(Settings.TurnMinDelay, Settings.TurnMaxDelay);
            var turn = Engine.Query("" + Board.GetMoves(), delay, skill);
            if (turn is null)
                ct.Cancel();
                if (!ct.IsCancellationRequested&&!suggest)
                    if (MySide == Side.Black)
                    {
                        AI.MakeTurn(turn.GetInverse());
                    }
                    else
                        AI.MakeTurn(turn);
            return turn;
        }
        public void PlayThread(CancellationTokenSource ct, Form1 form)
        {
            if (!CustomGame)
            {
                MySide = ImageAnalysis.DetectSide(ImageAnalysis.SliceTitles(ImageAnalysis.CaptureScreen(Settings)));
            }
            else
            {
                MySide = Board.SideToMove;
            }
            Log.Report(MySide);
            form.Log("" + MySide);
            var side = new Side(MySide.ToString());
            var re = new ManualResetEvent(false);
            Task.Run(() =>
            {
                while (!ct.Token.IsCancellationRequested)
                {
                    DetectedTurn = DetectTurn(ct);
                    re.Set();
                }
            });
            while (!ct.Token.IsCancellationRequested)
            {
                if (side == Side.White)
                {
                    Task.Run(()=>{
                        var turn = MakeTurn(ct, form.Suggest);
                        if (form.Suggest)
                            form.Log("" + turn);
                    });
                }
                re.Reset();
                re.WaitOne();
                side.Switch();
                var turn = DetectedTurn;
//                var turn = DetectTurn(ct);
                Board.TurnIn(turn);
                var str = "" + turn;
                form.Log(str);
                Log.Report(str);
            }
            form.Log(GameDone);
            Log.Report(GameDone);
        }

        public void Dispose()
        {
            ((IDisposable)Engine).Dispose();
        }
    }
}
