using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    class Detection
    {
        private List<List<int>> Data = new List<List<int>>();
        const string space = " ";
        public Detection()
        {
            try
            {
                var strData = File.ReadAllLines(SettingsStore.DetectionPresetPath);
                foreach (var row in strData)
                {
                    var r = new List<int>();
                    foreach (var i in row.Split(space))
                    {
                        r.Add(Convert.ToInt32(i));
                    }
                    Data.Add(r);
                }
                Data[12][0] = Data[12][0];//check borders

            }
            catch
            {
                Create();
            }
        }
        private void Create()
        {
            var set = new SettingsStore();
            var screen = ImageAnalysis.CaptureScreen(set);
            var titles = ImageAnalysis.SliceTitles(screen);
            var side = ImageAnalysis.DetectSide(titles);
            if (side == Side.White)
            {
                Data.Add(ImageAnalysis.GetImageHash(titles[0], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[1], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[2], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[3], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[4], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[8], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[63], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[62], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[61], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[60], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[59], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[55], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
                Data.Add(ImageAnalysis.GetImageHash(titles[40], set.Hash, set.Window, set.WhiteBright, set.BlackBright));
            }
            WriteData();
        }
        private void WriteData()
        {
            var str = "";
            foreach (var row in Data)
            {
                foreach (var i in row)
                {
                    str += i + space;
                }
                str += Environment.NewLine;
            }
            File.WriteAllText(SettingsStore.DetectionPresetPath, str);
        }
        private string DetectFigure(List<int> hash)
        {
            var e = 0;
            var el = "";
            for (var i = 0; i < Data.Count; i++)
            {
                int equalElements = hash.Zip(Data[i], (k, j) => k == j).Count(eq => eq);
                if (e < equalElements)
                {
                    e = equalElements;
                    el = Sw(i);
                }
            }
            return el;
        }
        public List<List<string>> DetectPos()
        {
            var set = new SettingsStore();
            var pos = new List<List<string>>();
            var screen = ImageAnalysis.CaptureScreen(set);
            var cells = ImageAnalysis.SliceTitles(screen);
            for (var i =0; i < SettingsStore.BoardLenght; i++)
            {
                var row = new List<string>();
                for (var j = 0; j < SettingsStore.BoardLenght; j++)
                {
                    row.Add(DetectFigure(ImageAnalysis.GetImageHash(cells[i*SettingsStore.BoardLenght+j], set.Hash, set.Window, set.WhiteBright, set.BlackBright)));
                }
                pos.Add(row);
            }
            return pos;
        }
        private string Sw(int i)
        {
            switch (i)
            {
                case 0:
                    return ChessBoard.Figures.WhiteRook;
                case 1:
                    return ChessBoard.Figures.WhiteKnight;
                case 2:
                    return ChessBoard.Figures.WhiteBishop;
                case 3:
                    return ChessBoard.Figures.WhiteQueen;
                case 4:
                    return ChessBoard.Figures.WhiteKnight;
                case 5:
                    return ChessBoard.Figures.WhitePawn;
                case 6:
                    return ChessBoard.Figures.BlackRook;
                case 7:
                    return ChessBoard.Figures.BlackKnight;
                case 8:
                    return ChessBoard.Figures.BlackBishop;
                case 9:
                    return ChessBoard.Figures.BlackQueen;
                case 10:
                    return ChessBoard.Figures.BlackKing;
                case 11:
                    return ChessBoard.Figures.BlackPawn;
                default:
                    return ChessBoard.Figures.Space;
            }
        }
    }
}
