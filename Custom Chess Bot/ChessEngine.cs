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
        const string RANK_SEPARATOR = "/";
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

        public string translateBoardToFEN(List<List<string>> board)
        {
            string fen = "";
            for (int rank = 0; rank < board.Count(); rank++)
            {
                int empty = 0;
                string rankFen = "";
                for (int file = 0; file < board[rank].Count(); file++)
                {
                    if (board[rank][file] == "")
                    {
                        empty++;
                    }
                    else
                    {
                        // add the number to the fen if not zero.
                        if (empty != 0) rankFen += empty;
                        // add the letter to the fen
                        rankFen += board[rank][file];
                        // reset the empty
                        empty = 0;
                    }
                }
                // add the number to the fen if not zero.
                if (empty != 0) rankFen += empty;
                // add the rank to the fen
                fen += rankFen;
                // add rank separator. If last then add a space
                if (!(rank == board.Count() - 1))
                {
                    fen += RANK_SEPARATOR;
                }
                else
                {
                    fen += " ";
                }
            }
            return fen;
        }
        public Turn NextMove(List<List<string>> board, bool side)
        {
            string _side = " w";
            if (!side) _side = " b";
    //        var _turn = new Turn(0, 0);
            logger.Log(Logger.Search + Logger.Me, "position fen " + translateBoardToFEN(board) + _side);
            turn = "";
            SendLine("position fen " + translateBoardToFEN(board) + _side);
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
                FileName = settings.uciPath,
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
