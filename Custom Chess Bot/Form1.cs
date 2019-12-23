using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Custom_Chess_Bot
{
    public partial class Form1 : Form
    {
        private bool isRunning = false;
        private static CancellationTokenSource ct;
        private Settings settings = new Settings();
        private GlobalKeyboardHook _globalKeyboardHook;
        private static readonly Logger logger = new Logger();
        private static CancellationTokenSource cts;

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += KeyPressHandler;
        }


        public Form1()
        {
            SetupKeyboardHooks();
            InitializeComponent();
            Log("Ready!");
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
            var picBox = new PictureBox();
            picBox.Image = pic;
            picBox.Width = 300;
            picBox.Height = 300;
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
            if (panel1.InvokeRequired)
            {
                panel1.Invoke(new MethodInvoker(delegate {
                    panel1.Controls.Clear();
                    panel1.Controls.Add(picBox);
                }));
            }

        }
        private void Reload()
        {
            ImageAnalysis.settings = new Settings();
            settings = new Settings();
        }
        private async void Button1_Click(object sender, EventArgs e)
        {
            var firstPosition = await GetMousePosition();
            log.Text = ImageAnalysis.GetColourBrighness(firstPosition).ToString();
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            if (ct != null)
            {
                ct.Cancel();
            }
        }
        private void Button6_Click(object sender, EventArgs e)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = Settings.SettingsPath
            };

            process.Start();
            process.WaitForExit();
            Reload();
        }
        private async Task<Point> GetMousePosition()
        {
            await Task.Run(() =>
            {
                while (MouseButtons != MouseButtons.Left) { Thread.Sleep(10); }
            });
            return Cursor.Position;


        }
        private async void button2_Click(object sender, EventArgs e)
        {
            log.Text = @"Click top-left board point.";
            var firstPosition = await GetMousePosition();
            Thread.Sleep(500); //click duration
            log.Text = @"Click right-bottom board point.";
            var secondPosition = await GetMousePosition();
            var BoardSize = new Size(secondPosition.X - firstPosition.X, secondPosition.Y - firstPosition.Y);
            settings.CalibrateBoard(BoardSize, firstPosition);
            log.Text = @"Saved!";
            Reload();
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
                    DisplayBitMap(ImageAnalysis.CaptureScreen());
                    Thread.Sleep(1000);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "Display")
            {
                cts = new CancellationTokenSource();
                Task.Run(() => DisplayThread(cts), cts.Token);
                button5.Text = "Stop display";
            }
            else
            {
                cts.Cancel();
                cts.Dispose();
                button5.Text = "Display";
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                Task.Run(() =>
                {
                    if (ct != null)
                    {
                        ct.Cancel();
                    }
                    ct = new CancellationTokenSource();
                    Task.Run(() =>
                     {
                         logger.Log(Logger.SpawnThread, "Play");
                         var pe = new PlayEngine(ct, this);
                         pe.Dispose();
                     }).Wait();
                    Log("Complited");
                    isRunning = false;
                });
            }
        }
    }
}
