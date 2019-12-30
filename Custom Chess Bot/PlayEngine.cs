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
        const string Completed = @"Completed";
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

        public Turn EnemyTurn(CancellationTokenSource ct, Turn lastTurn)
        {
            var turn = ImageAnalysis.FindTurn(Log, Settings,ct, lastTurn);
            if (turn is null)
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
            if (turn is null)
                ct.Cancel();
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
            if (MySide == Side.Black&& !ct.Token.IsCancellationRequested)
            {
                var str = ""+EnemyTurn(ct, null);
                form.Log(str);
                Log.Report(str);
            }
            while (!ct.Token.IsCancellationRequested)
            {
                var myTurn = MyTurn(ct);
                var _str = "" +myTurn;
                form.Log(_str);
                Log.Report(_str);
                var enemyTurn = EnemyTurn(ct, myTurn);
                var str = "" + enemyTurn;
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
