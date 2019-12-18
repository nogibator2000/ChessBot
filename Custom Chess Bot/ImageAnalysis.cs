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
    class ImageAnalysis
    {
        private static readonly Settings settings = new Settings();
        public List<Bitmap> SliceTitles(Bitmap board)
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
        public bool AmIWhite(List<Bitmap> board)
        {
            var pixel1 = board[0].GetPixel(board[0].Width / 2, board[0].Height / 2).GetBrightness();
            var pixel57 = board[56].GetPixel(board[56].Width / 2, board[56].Height / 2).GetBrightness();
            File.AppendAllText(settings.LogPath, "[side] white?" + (pixel1 < pixel57).ToString()+Environment.NewLine);
            return pixel1 < pixel57;
        }
        public Turn AnalizingTurn(List<Bitmap> board1, List<Bitmap> board2, bool side)
        {
            var candidates = new List<int>();
            for (var i = 0; i < board1.Count; i++)
            {
                var iHash1 = GetImageHash(board1[i]);
                var iHash2 = GetImageHash(board2[i]);
                int equalElements = iHash1.Zip(iHash2, (k, j) => k == j).Count(eq => eq);
                var fullTrH = settings.threshold * settings.hash* settings.hash * (1 - 2 * settings.window) * (1 - 2 * settings.window);
                if (equalElements != iHash1.Count()|| equalElements != iHash2.Count())
                {
                    File.AppendAllText(settings.LogPath, "[ML][finding][equal]" + equalElements + "[tres]" + fullTrH + "[cell]" + i + Environment.NewLine);
                }
                if (equalElements < fullTrH)
                {
                    candidates.Add(i);
                }
                if (candidates.Count > 1 && candidates[0]>8 && candidates[1]>8) //finding "O-O"
                {
                    File.AppendAllText(settings.LogPath, "[ML][FoundTurn] " + new Turn(candidates[0], candidates[1]).GetStr() + Environment.NewLine);
                    return new Turn(candidates[0], candidates[1]);
                }
            }
            if (candidates.Count > 3)
            {
                var king = false;
                var _side = 0;
                for (var i = 0; i < candidates.Count(); i++)
                {
                    if (side)
                    {
                        if (candidates[i] == 4)
                        {
                            king = true;
                        }
                        else
                        {
                            if (candidates[i] < 4)
                            {
                                _side--;
                            }
                            else
                            {
                                _side++;
                            }
                        }
                    }
                    else
                    {
                        if (candidates[i] == 3)
                        {
                            king = true;
                        }
                        else
                        {
                            if (candidates[i] < 3)
                            {
                                _side--;
                            }
                            else
                            {
                                _side++;
                            }
                        }
                    }
                }
                if (side)
                {
                    if (king)
                    {
                        if (_side > 0)
                        {
                            File.AppendAllText(settings.LogPath, "[ML][FoundTurn] O-O" + Environment.NewLine);
                            return new Turn(4, 6);
                        }
                        else
                        {
                            File.AppendAllText(settings.LogPath, "[ML][FoundTurn] O-O-O" + Environment.NewLine);
                            return new Turn(4, 2);
                        }
                    }
                }
                else
                {
                    if (king)
                    {
                        if (_side > 0)
                        {
                            File.AppendAllText(settings.LogPath, "[ML][FoundTurn] O-O-O" + Environment.NewLine);
                            return new Turn(3, 5);
                        }
                        else
                        {
                            File.AppendAllText(settings.LogPath, "[ML][FoundTurn] O-O" + Environment.NewLine);
                            return new Turn(3, 1);
                        }
                    }
                }
            }
            if (candidates.Count > 1)
            {
                File.AppendAllText(settings.LogPath, "[ML][FoundTurn] " + new Turn(candidates[0], candidates[1]).GetStr() + Environment.NewLine);
                return new Turn(candidates[0], candidates[1]);
            }
            return new Turn(false);
           
        }
        private Bitmap ApplyFilter(Bitmap lockedBitmap)
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
        public List<int> GetImageHash(Bitmap bmp)
        {
            var lResult = new List<int>();
            Bitmap bmpMin = new Bitmap(bmp, new Size(settings.hash, settings.hash));
            for (var j = Convert.ToInt32(bmpMin.Height*settings.window); j < bmpMin.Height*(1-settings.window); j++)
            {
                for (var i = 0 + Convert.ToInt32(bmpMin.Width * settings.window); i < bmpMin.Width*(1-settings.window); i++)
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

        public string GetColourBrighness(Point position)
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
        public Bitmap CaptureScreen()
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
