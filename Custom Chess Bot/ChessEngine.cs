using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    class ChessEngine:IDisposable
    {
        private static readonly Logger logger = new Logger();
        string turn;
        private static readonly Settings settings = new Settings();

        public static string ExtractTurn(string strSource, string flag)
        {
            int Start;
            if (strSource == null)
            {
                return "terminated";
            }
            if (strSource.Contains(flag))
            {
                Start = strSource.IndexOf(flag, 0) + flag.Length;
                return strSource.Substring(Start);
            }
            else
            {
                return "";
            }
        }

        public Turn NextMove(Board board, bool side)
        {
            logger.Log(Logger.Search + Logger.Me, "position fen " + board.translateBoardToFEN(side));
            turn = "";
            //            SendLine("position fen " + board.translateBoardToFEN(side));
            SendLine("position startpos moves " + board.moves);
            var rand = new Random();
            var MissPlayCheck = rand.Next(1, settings.MissplayEveryXTurns + 1);
            if (MissPlayCheck == settings.MissplayEveryXTurns)
            {
                SendLine("setoption name Skill Level value 1");
            }
            else
            {
                SendLine("setoption name Skill Level value 20");
            }
            SendLine("go movetime " + settings.MoveTime);
            while (turn == "")
            {

            }
            if (turn == "terminated")
            {
                return new Turn(false);
            }
            var _var = turn.ToCharArray();
            var startpoint = 0;
            for (var i = 0; i < Settings.syms.Length; i++)
            {
                if (Settings.syms[i] == _var[0].ToString())
                {
                    startpoint = Settings.BoardLenght*(Settings.BoardLenght - Convert.ToInt32(_var[1].ToString())) + i;
                    break;
                }
            }
            var endpoint = 0;
            for (var i = 0; i < Settings.syms.Length; i++)
            {
                if (Settings.syms[i] == _var[2].ToString())
                {
                    endpoint = Settings.BoardLenght* (Settings.BoardLenght -  Convert.ToInt32(_var[3].ToString())) + i;
                    break;
                }
            }
            var _turn = new Turn(startpoint, endpoint, side);
//            if (!side) _turn = _turn.Inverse();
            return _turn;
        }
        Process myProcess;
        public ChessEngine() 
        {
            ProcessStartInfo si = new ProcessStartInfo()
            {
                FileName = settings.EnginePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            myProcess = new Process
            {
                StartInfo = si
            };
            myProcess.OutputDataReceived += new DataReceivedEventHandler(myProcess_OutputDataReceived);

            myProcess.Start();
            myProcess.BeginErrorReadLine();
            myProcess.BeginOutputReadLine();

        }
        public void SendLine(string command)
        {
            myProcess.StandardInput.WriteLine(command);
            myProcess.StandardInput.Flush();
        }

        public void myProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            turn = ExtractTurn(e.Data, "bestmove ");
        }

        public void Dispose()
        {
            myProcess.Dispose();
            GC.Collect();
        }
    }
}
