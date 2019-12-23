using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    static class ImageAnalysis
    {
        private static readonly Logger logger = new Logger();
        public static Settings settings = new Settings();
        public static List<Bitmap> SliceTitles(Bitmap board)
        {
            var titles = new List<Bitmap>(Settings.BoardLenght * Settings.BoardLenght);
            var sliceWidth = (board.Width / Settings.BoardLenght);
            var sliceHight = (board.Height / Settings.BoardLenght);
            for (var i = 0; i < Settings.BoardLenght; i++)
            {
                for (var j = 0; j < Settings.BoardLenght; j++)
                {
                    var title = new Rectangle(j * sliceWidth, i * sliceHight, sliceWidth, sliceHight);
                    titles.Add(board.Clone(title, PixelFormat.Format32bppArgb));
                }
            }
            return titles;
        }
        public static bool AmIWhite(List<Bitmap> board)
        {
            // difference between middle A1 and middle A8 probably correct
            var pixel1 = board[0].GetPixel(board[0].Width / 2, board[0].Height / 2).GetBrightness();
            var pixel57 = board[56].GetPixel(board[56].Width / 2, board[56].Height / 2).GetBrightness();
            logger.Log(Logger.ML + Logger.Side, (pixel1 < pixel57).ToString());
            return pixel1 < pixel57;
        }
        public static Turn AnalizingTurn(List<Bitmap> board1, List<Bitmap> board2, bool side)
        {
            var candidates = new List<int>();
            for (var i = 0; i < board1.Count; i++)
            {
                var imageHash1 = GetImageHash(board1[i]);
                var imageHash2 = GetImageHash(board2[i]);
                int equalElements = imageHash1.Zip(imageHash2, (k, j) => k == j).Count(eq => eq);
                var treshold = settings.Threshold * settings.Hash * settings.Hash * (1 - 2 * settings.Window) * (1 - 2 * settings.Window);
                if (equalElements != imageHash1.Count() || equalElements != imageHash2.Count())
                {
                    logger.Log(Logger.ML + Logger.Processing, "[equal]" + equalElements + "[tres]" + treshold + "[cell]" + i);
                }
                if (equalElements < treshold)
                {
                    candidates.Add(i);
                }
                if (candidates.Count > 1 && candidates[0] > 8 && candidates[1] > 8) //finding "O-O"
                {
                    var turn = new Turn(candidates[0], candidates[1], side);
                    if (side)
                        turn = turn.Inverse();
                    logger.Log(Logger.ML + Logger.FoundTurn, turn.GetStr());
                    return turn;
                }
            }
            if (candidates.Count > 3)
            {
               var move =  MoveOO(side, candidates);
                if (move.valid)
                    return move;
            }
            else if (candidates.Count > 1)
            {
                var turn = new Turn(candidates[0], candidates[1], side);
                if (side)
                    turn = turn.Inverse();
                logger.Log(Logger.ML + Logger.FoundTurn, turn.GetStr());
                return turn;
            }
            return new Turn(false);

        }
        private static Turn MoveOO(bool side, List<int> candidates)
        {
            var score = 0;
            for (var i = 0; i < candidates.Count(); i++)
            {
                if (!side && (candidates[i] == 3 || candidates[i] == 2 || candidates[i] == 0 || candidates[i] == 4))
                {
                    score += 2;
                }
                else if (!side && (candidates[i] == 5 || candidates[i] == 6 || candidates[i] == 7))
                {
                    score += 1;
                }
                else if (side && (candidates[i] == 3 || candidates[i] == 4 || candidates[i] == 5 || candidates[i] == 7))
                {
                    score -= 2;
                }
                else if (side && (candidates[i] == 0 || candidates[i] == 1 || candidates[i] == 2))
                {
                    score -= 1;
                }
            }
            if (score < -7|| score > 7)
            {
                var turn = new Turn(false, side);
                logger.Log(Logger.ML + Logger.FoundTurn, turn.GetStr());
                return turn;
            } else
            if (score < -4|| score > 4)
            {
                var turn = new Turn(true, side);
                logger.Log(Logger.ML + Logger.FoundTurn, turn.GetStr());
                return turn;
            }
            return new Turn(false);
        }
        private static Bitmap ApplyFilter(Bitmap lockedBitmap)
        {

            for (int y = 0; y < lockedBitmap.Height; y++)
            {
                for (int x = 0; x < lockedBitmap.Width; x++)
                {
                    var pixel = lockedBitmap.GetPixel(x, y);
                    if (pixel.ToArgb() == settings.Color1 || pixel.ToArgb() == settings.Color2 || pixel.ToArgb() == settings.Color3 || pixel.ToArgb() == settings.Color4)
                    {
                        lockedBitmap.SetPixel(x, y, Color.FromArgb(settings.NeutralColor));
                    }
                }
            }
            return lockedBitmap;
        }
        public static List<int> GetImageHash(Bitmap bmp)
        {
            var lResult = new List<int>();
            Bitmap bmpMin = new Bitmap(bmp, new Size(settings.Hash, settings.Hash));
            for (var j = Convert.ToInt32(bmpMin.Height*settings.Window); j < bmpMin.Height*(1-settings.Window); j++)
            {
                for (var i = 0 + Convert.ToInt32(bmpMin.Width * settings.Window); i < bmpMin.Width*(1-settings.Window); i++)
                {
                    var pixelBright = bmpMin.GetPixel(i, j).GetBrightness();
                    if (pixelBright > settings.WhiteFilter)
                    {
                        lResult.Add(1);
                    }
                    else if (pixelBright < settings.BlackFilter)
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
        public static Bitmap CaptureScreen()
        {
            Bitmap screenshot = new Bitmap(settings.BoardSize.Width, settings.BoardSize.Height, PixelFormat.Format32bppArgb);

            Graphics gfxScreenshot = Graphics.FromImage(screenshot);

            gfxScreenshot.CopyFromScreen(settings.BoardPosition.X, settings.BoardPosition.Y, 0, 0, settings.BoardSize, CopyPixelOperation.SourceCopy);

            gfxScreenshot.Dispose();
            if (settings.FilterEnable)
            return ApplyFilter(screenshot);
            return screenshot;
        }
    }
}
