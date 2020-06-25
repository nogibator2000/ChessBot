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
        private ManualResetEvent RE;
        private string Move;
        public bool Running;
        private readonly Process ChessProcess;
        string _option1;
        string _option2;
        string _option3;
        public ChessEngine(string  path, string option1, string option2, string option3)
        {
            _option1 = option1;
            _option2 = option2;
            _option3 = option3;
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
            var check = new Stopwatch();
            check.Start();
            RE = new ManualResetEvent(false);
            SendLine(SkillFlag + skill);
            if (_option1 != @"null")
            {
                SendLine(_option1);
            }
            if (_option2 != @"null")
            {
                SendLine(_option2);
            }
            if (_option3 != @"null")
            {
                SendLine(_option3);
            }
            SendLine(query);
            SendLine(GoMTFlag + mt);
            RE.Reset();
            RE.WaitOne();
            if (!Running)
                return null;
            check.Stop();
            if (check.ElapsedMilliseconds < mt)
                Thread.Sleep(mt - Convert.ToInt32(check.ElapsedMilliseconds));
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
