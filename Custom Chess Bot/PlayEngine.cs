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
        private const int DefaultSkill = 20;
        private const int RetardedSkill = 1;
        public PlayEngine(string str=null)
        {
            if (str != null)
            {
                Board = new ChessBoard(str);
            }
            else
            {
                Board = new ChessBoard();
            }
            Settings = new SettingsStore();
            Log = new LogWriter(Settings);
            Engine = new ChessEngine(Settings.EnginePath);
            AI = new Controller(Settings);
        }
        const string Completed = @"Completed";
        private const int DelayTurns = 10;
        private const float DelayPart = 0.7f;

        private int CalcDelay(int minDelay, int maxDelay)
        {
            var rand = new Random();
            var delay =rand.Next(Convert.ToInt32(minDelay * DelayPart), Convert.ToInt32(maxDelay * DelayPart));
            if (rand.Next(0, DelayTurns + 1) == 1)
                delay+=rand.Next(Convert.ToInt32(minDelay * (1 - DelayPart) * DelayTurns), Convert.ToInt32(maxDelay * (1 - DelayPart) * DelayTurns));
            return delay;
        }

        public Turn EnemyTurn(CancellationTokenSource ct)
        {
            var turn = ImageAnalysis.FindTurn(Log, Settings,ct);
            if (turn == null)
                return null;
            if (MySide == Side.Black)
                turn = turn.GetInverse();
            Board.TurnIn(turn);
            return turn;
        }
        public Turn MyTurn(CancellationTokenSource ct)
        {
            var rand = new Random();
            var skill = DefaultSkill;
            if (rand.Next(1, Settings.MissplayTurns + 1) == Settings.MissplayTurns)
                skill = RetardedSkill;
            var delay = CalcDelay(Settings.TurnMinDelay, Settings.TurnMaxDelay);
            var turn = Engine.Query("" + Board.GetMoves(), delay, skill);
            if (turn == null)
                ct.Cancel();
            Thread.Sleep(delay);
            if (!ct.IsCancellationRequested)
                if (MySide == Side.Black)
                {
                    AI.MakeTurn(turn.GetInverse());
                }
                else
                    AI.MakeTurn(turn);
            Board.TurnIn(turn);
            return turn;
        }
        public void PlayThread(CancellationTokenSource ct, Form1 form)
        {
            MySide  = ImageAnalysis.DetectSide(ImageAnalysis.SliceTitles(ImageAnalysis.CaptureScreen(Settings)));
            Log.Report(MySide);
            form.Log("" + MySide);
            if (MySide == Side.Black&& !ct.Token.IsCancellationRequested)
            {
                var str = ""+EnemyTurn(ct);
                form.Log(str);
                Log.Report(str);
            }
            while (!ct.Token.IsCancellationRequested)
            {
                var _str = ""+MyTurn(ct);
                form.Log(_str);
                Log.Report(_str);
                var str = ""+EnemyTurn(ct);
                form.Log(str);
                Log.Report(str);
            }
            form.Log(Completed);
            Log.Report(Completed);
        }

        public void Dispose()
        {
            ((IDisposable)Engine).Dispose();
        }
    }
}
