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
    static class ImageAnalysis
    {
        const float AnimationWindow = 0.0f;
        public static Bitmap CaptureScreen(SettingsStore settings)
        {
            Bitmap screenshot = new Bitmap(settings.BoardSize.Width, settings.BoardSize.Height, PixelFormat.Format32bppArgb);

            Graphics gfxScreenshot = Graphics.FromImage(screenshot);

            gfxScreenshot.CopyFromScreen(settings.BoardPosition.X, settings.BoardPosition.Y, 0, 0, settings.BoardSize, CopyPixelOperation.SourceCopy);

            gfxScreenshot.Dispose();
            if (settings.FilterEnable)
                return ApplyFilter(screenshot, settings);
            return screenshot;
        }
        private static Bitmap ApplyFilter(Bitmap bmpBoard, SettingsStore settings)
        {

            for (int y = 0; y < bmpBoard.Height; y++)
            {
                for (int x = 0; x < bmpBoard.Width; x++)
                {
                    var pixel = bmpBoard.GetPixel(x, y);
                    if (pixel.ToArgb() == settings.Color1 || pixel.ToArgb() == settings.Color2 || pixel.ToArgb() == settings.Color3 || pixel.ToArgb() == settings.Color4)
                    {
                        bmpBoard.SetPixel(x, y, Color.FromArgb(settings.NeutralColor));
                    }
                }
            }
            return bmpBoard;
        }
        public static Side DetectSide(List<Bitmap> board)
        {
            // difference between middle A1 and middle A8 probably correct
            var pixel1 = board[0].GetPixel(board[0].Width / 2, board[0].Height / 2).GetBrightness();
            var pixel57 = board[56].GetPixel(board[56].Width / 2, board[56].Height / 2).GetBrightness();
            var side = new Side(pixel1 > pixel57);
            return side;
        }
        private static List<Bitmap> GetSmoothBoard(SettingsStore Settings, CancellationTokenSource ct)
        {
            Bitmap candidate1, candidate2;
            while (!ct.IsCancellationRequested)
            {
                candidate1 = CaptureScreen(Settings);
                Thread.Sleep(Settings.AnimationDelay);
                candidate2 = CaptureScreen(Settings);
                var c1h = GetImageHash(candidate1, Settings.AnimationHash, AnimationWindow, Settings.WhiteBright, Settings.BlackBright);
                var c2h = GetImageHash(candidate2, Settings.AnimationHash, AnimationWindow, Settings.WhiteBright, Settings.BlackBright);
                int equalElements = c1h.Zip(c2h, (k, j) => k == j).Count(eq => eq);
                if (equalElements == c1h.Count())
                    return SliceTitles(candidate1);
            }
            return null;
        }
        public static Turn FindTurn(LogWriter logger, SettingsStore settings, CancellationTokenSource ct)
        {
            Turn turn;
            var slicedBoard = GetSmoothBoard(settings,  ct);
            do
            {
                Thread.Sleep(settings.RefreshRate);
                var _slicedBoard = GetSmoothBoard(settings, ct);
                turn = AnalizingTurn(slicedBoard, _slicedBoard, settings, logger);
            } while (turn == null && !ct.IsCancellationRequested);
            return turn;
        }

        public static Turn AnalizingTurn(List<Bitmap> board1, List<Bitmap> board2, SettingsStore Settings, LogWriter log)
        {
            if (board1 == null || board2 == null||board1.Count==0||board2.Count==0)
                return null;
            var candidates = new List<int>();
            for (var i = 0; i < board1.Count; i++)
            {
                var imageHash1 = GetImageHash(board1[i], Settings.Hash, Settings.Window, Settings.WhiteBright, Settings.BlackBright);
                var imageHash2 = GetImageHash(board2[i], Settings.Hash, Settings.Window, Settings.WhiteBright, Settings.BlackBright);
                int equalElements = imageHash1.Zip(imageHash2, (k, j) => k == j).Count(eq => eq);
                var th = Settings.Threshold * imageHash1.Count();
                if (equalElements != imageHash1.Count() || equalElements != imageHash2.Count())
                {
                    log.Report(equalElements, Settings.Threshold, i);
                }
                if (equalElements < th)
                {
                    candidates.Add(i);
                }
            }
            if (candidates.Count == 3)
            {
                return MovePV(candidates);
            }
            else if (candidates.Count == 4)
            {
                return MoveOO(candidates);
            }
            else if (candidates.Count == 2)
            {
                return new Turn(candidates[0], candidates[1]);
            }
            return null;

        }

    
        private static Turn MovePV(List<int> candidates)
        {
            if (candidates[0] - candidates[1] == 7 || candidates[0] - candidates[1] == -7 || candidates[0] - candidates[1] == 9 || candidates[0] - candidates[1] == -9)
                return new Turn(candidates[0], candidates[1]);
            if (candidates[2] - candidates[1] == 7 || candidates[2] - candidates[1] == -7 || candidates[2] - candidates[1] == 9 || candidates[2] - candidates[1] == -9)
                return new Turn(candidates[2], candidates[1]);
            if (candidates[2] - candidates[0] == 7 || candidates[2] - candidates[0] == -7 || candidates[2] - candidates[0] == 9 || candidates[2] - candidates[0] == -9)
                return new Turn(candidates[2], candidates[0]);
            return null;
        }
        private static Turn MoveOO(List<int> candidates)
        {
            candidates.Sort();
            if (candidates[0] == 0 || candidates[1] == 2 || candidates[2] == 3 || candidates[3] == 4)
            {
                return new Turn(4, 2, new Side(Side.White));
            }
            if (candidates[0] == 4 || candidates[1] == 5 || candidates[2] == 6 || candidates[3] == 7)
            {
                return new Turn(4, 7, new Side(Side.White));
            }
            if (candidates[0] == 56 || candidates[1] == 58 || candidates[2] == 59 || candidates[3] == 60)
            {
                return new Turn(60, 58, new Side(Side.Black));
            }
            if (candidates[0] == 60 || candidates[1] == 61 || candidates[2] == 62 || candidates[3] == 63)
            {
                return new Turn(60, 62, new Side(Side.Black));
            }
            if (candidates[0] == 59 || candidates[1] == 60 || candidates[2] == 61 || candidates[3] == 63)
            {
                return new Turn(4, 2, new Side(Side.White));
            }
            if (candidates[0] == 56 || candidates[1] == 57 || candidates[2] == 58 || candidates[3] == 59)
            {
                return new Turn(4, 7, new Side(Side.White));
            }
            if (candidates[0] == 3 || candidates[1] == 4 || candidates[2] == 5 || candidates[3] == 7)
            {
                return new Turn(60, 58, new Side(Side.Black));
            }
            if (candidates[0] == 0 || candidates[1] == 1 || candidates[2] == 2 || candidates[3] == 3)
            {
                return new Turn(60, 62, new Side(Side.Black));
            }
            return null;
        }
        public static List<Bitmap> SliceTitles(Bitmap board)
        {
            var titles = new List<Bitmap>(SettingsStore.BoardLenght * SettingsStore.BoardLenght);
            var sliceWidth = (board.Width / SettingsStore.BoardLenght);
            var sliceHight = (board.Height / SettingsStore.BoardLenght);
            for (var i = SettingsStore.BoardLenght-1; i >= 0 ; i--)
            {
                for (var j = 0; j < SettingsStore.BoardLenght; j++)
                {
                    var title = new Rectangle(j * sliceWidth, i * sliceHight, sliceWidth, sliceHight);
                    titles.Add(board.Clone(title, PixelFormat.Format32bppArgb));
                }
            }
            return titles;
        }
        public static List<int> GetImageHash(Bitmap bmp, int hash, float window, float whiteTexture, float blackTexture)
        {
            var lResult = new List<int>();
            Bitmap bmpMin = new Bitmap(bmp, new Size(hash, hash));
            for (var j = Convert.ToInt32(bmpMin.Height*window); j < bmpMin.Height*(1-window); j++)
            {
                for (var i = 0 + Convert.ToInt32(bmpMin.Width * window); i < bmpMin.Width*(1-window); i++)
                {
                    var pixelBright = bmpMin.GetPixel(i, j).GetBrightness();
                    if (pixelBright > whiteTexture)
                    {
                        lResult.Add(1);
                    }
                    else if (pixelBright < blackTexture)
                    {
                        lResult.Add(-1);
                    } 
                    else
                        lResult.Add(0);
                }
            }
            bmpMin.Dispose();
            return lResult;
        }

        public static string GetColourBrighness(Point position)
        {
            int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            Bitmap screenshot = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            Graphics gfxScreenshot = Graphics.FromImage(screenshot);

            gfxScreenshot.CopyFromScreen(0, 0, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            gfxScreenshot.Dispose();
            var pixel = screenshot.GetPixel(position.X, position.Y);
            return "B:" + pixel.GetBrightness()+" ARBG:"+pixel.ToArgb();

        }
    }
}
