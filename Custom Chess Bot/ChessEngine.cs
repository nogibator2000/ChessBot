using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    class ChessEngine
    {
        string turn;
        const string RANK_SEPARATOR = "/";
        private static readonly Settings settings = new Settings();

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start;//, End;
            if (strSource == null)
            {
                return "done";
            }
            if (strSource.Contains(strStart))// && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
 //               End = strSource.IndexOf(strEnd, Start);
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
        public async Task<Turn> NextMove(List<List<string>> board, bool side)
        {
            string _side = " w";
            if (!side) _side = " b";
            var _turn = new Turn(-1, -1);
            File.AppendAllText(settings.LogPath, "[Search] position fen " + translateBoardToFEN(board) + _side + Environment.NewLine);
            turn = "";
            SendLine("position fen " + translateBoardToFEN(board) + _side);
            SendLine("go movetime " + settings.MoveTime);
            await Task.Run(() => {
                while (turn == "")
                {

                }
            });
            if (turn == "done")
            {
                return new Turn(-2, -2);
            }
            var _var = turn.ToCharArray();
            for (var i = 0; i < settings.syms.Length; i++)
            {
                if (settings.syms[i] == _var[0].ToString())
                {
                    _turn.start = 64 - 8 * Convert.ToInt32(_var[1].ToString()) + i;
                    break;
                }
            }
            for (var i = 0; i < settings.syms.Length; i++)
            {
                if (settings.syms[i] == _var[2].ToString())
                {
                    _turn.end = 64 - 8 * Convert.ToInt32(_var[3].ToString()) + i;
                    break;
                }
            }
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

            myProcess = new Process();
            myProcess.StartInfo = si;
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
            turn = getBetween(e.Data, "bestmove ", " ");
        }
    }
}
