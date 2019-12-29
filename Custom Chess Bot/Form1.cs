using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Custom_Chess_Bot
{
    public partial class Form1 : Form, IDisposable
    {
        private bool isRunning = false;
        private static CancellationTokenSource CTPlay;
        private GlobalKeyboardHook _globalKeyboardHook;
        PictureBox PicBox;
        private static CancellationTokenSource CTDisplay;
        const string Ready = @"Ready!";
        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += KeyPressHandler;
        }


        public Form1()
        {
            SetupKeyboardHooks();
            InitializeComponent();
            Log(Ready);
        }
        private void KeyPressHandler(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.F2)
            {
                button3.PerformClick();
            }
            if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.F4)
            {
                button4.PerformClick();
            }

        }
        private void DisplayBitMap(Bitmap pic)
        {
            PicBox = new PictureBox
            {
                Image = pic,
                Width = 300,
                Height = 300,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            if (panel1.InvokeRequired)
            {
                panel1.Invoke(new MethodInvoker(delegate
                {
                    panel1.Controls.Clear();
                    panel1.Controls.Add(PicBox);
                }));
            }
        }
        private async void Button1_Click(object sender, EventArgs e)
        {
            var firstPosition = await GetMousePosition();
            log.Text = ImageAnalysis.GetColourBrighness(firstPosition).ToString();
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            if (CTPlay != null)
            {
                CTPlay.Cancel();
            }
        }
        private void Button6_Click(object sender, EventArgs e)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = SettingsStore.SettingsPath
                }
            };

            process.Start();
            process.WaitForExit();
            process.Dispose();
        }
        private async Task<Point> GetMousePosition()
        {
            await Task.Run(() =>
            {
                while (MouseButtons != MouseButtons.Left) { Thread.Sleep(10); }
            });
            return Cursor.Position;


        }
        private async void CalibrateBoard(object sender, EventArgs e)
        {
            log.Text = @"Click top-left board point.";
            var firstPosition = await GetMousePosition();
            Thread.Sleep(500); //click duration
            log.Text = @"Click right-bottom board point.";
            var secondPosition = await GetMousePosition();
            var BoardSize = new Size(secondPosition.X - firstPosition.X, secondPosition.Y - firstPosition.Y);
            var settings = new SettingsStore();
            settings.CalibrateBoard(BoardSize, firstPosition);
            log.Text = @"Saved!";
        }
        public void Log(string _log)
        {
            if (log.InvokeRequired)
            {
                log.Invoke(new MethodInvoker(delegate
                {
                    log.Text = _log;
                }));
            }
            else
            {
                log.Text = _log;
            }
        }
        private void DisplayThread(CancellationTokenSource cts)
        {
            while (!cts.IsCancellationRequested)
            {
                var settings = new SettingsStore();
                DisplayBitMap(ImageAnalysis.CaptureScreen(settings));
                Thread.Sleep(34);
            }
        }
        private void DisplayButton(object sender, EventArgs e)
        {
            if (button5.Text == "Display")
            {
                CTDisplay = new CancellationTokenSource();
                Task.Run(() => DisplayThread(CTDisplay), CTDisplay.Token);
                button5.Text = "Stop display";
            }
            else
            {
                CTDisplay.Cancel();
                CTDisplay.Dispose();
                button5.Text = "Display";
            }
        }
        private void CancelBtn(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                CTPlay = new CancellationTokenSource();
                Task.Run(() =>
                {
                    using (var pe = new PlayEngine())
                        pe.PlayThread(CTPlay, this);
                    isRunning = false;
                });
            }
        }
        public new void Dispose()
        {
            PicBox.Dispose();
            _globalKeyboardHook.Dispose();
            components.Dispose();
        }
    }
}
