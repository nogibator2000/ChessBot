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
        private const string SkillFlag = "setoption name Skill Level value ";
        private const string GoMTFlag = "go movetime ";
        private const string BestMoveFlag = "bestmove ";
        private const string SplitSymbol = " ";
        private ManualResetEvent Promise;
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
                    Promise.Set();
                }
                else if (e.Data.Contains(BestMoveFlag))
                {
                    var start = e.Data.IndexOf(BestMoveFlag) + BestMoveFlag.Length;
                    Move = e.Data.Substring(start).Split(SplitSymbol)[0];
                    Promise.Set();
                }
        }

        public Turn Query(string query, int mt = 500, int skill= 20)
        {
            Promise = new ManualResetEvent(false);
            SendLine(SkillFlag + skill);
            SendLine(query);
            SendLine(GoMTFlag + mt);
            Promise.Reset();
            Promise.WaitOne();
            if (!Running)
                return null;
            return new Turn(Move);
        }


        public void Dispose()
        {
            if (ChessProcess != null)
                ChessProcess.Dispose();
            if(Promise != null)
            Promise.Dispose();
        }
    }
}
