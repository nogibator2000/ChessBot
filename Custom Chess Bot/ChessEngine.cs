using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    public class ChessEngine:IDisposable
    {
        private const string TimeFlag = "setoption name Minimum Thinking Time value ";
        private const string SkillFlag = "setoption name Skill Level value ";
        private const string GoMTFlag = "go movetime ";
        private const string BestMoveFlag = "bestmove ";
        private const string SplitSymbol = " ";
        private ManualResetEvent RE;
        private string Move;
        public bool Running;
        private readonly Process ChessProcess;
        public ChessEngine(string  path)
        {
            Running = true;
            ProcessStartInfo si = new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            ChessProcess = new Process
            {
                StartInfo = si
            };
            ChessProcess.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceived);

            ChessProcess.Start();
            ChessProcess.BeginErrorReadLine();
            ChessProcess.BeginOutputReadLine();

        }
        private void SendLine(string command)
        {
            ChessProcess.StandardInput.WriteLine(command);
            ChessProcess.StandardInput.Flush();
        }
        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (Running)
                if (e.Data == null)
                {
                    Running = false;
                    RE.Set();
                }
                else if (e.Data.Contains(BestMoveFlag))
                {
                    var start = e.Data.IndexOf(BestMoveFlag) + BestMoveFlag.Length;
                    Move = e.Data.Substring(start).Split(SplitSymbol)[0];
                    RE.Set();
                }
        }

        public Turn Query(string query, int mt = 500, int skill= 20)
        {
            RE = new ManualResetEvent(false);
            SendLine(SkillFlag + skill);
            SendLine(TimeFlag + mt);
            SendLine(query);
            SendLine(GoMTFlag + mt);
            RE.Reset();
            RE.WaitOne();
            if (!Running)
                return null;
            return new Turn(Move);
        }


        public void Dispose()
        {
            if (ChessProcess != null)
                ChessProcess.Dispose();
            if(RE != null)
            RE.Dispose();
        }
    }
}
