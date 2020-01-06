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
        const string TM = @"[F10] Auto turn";
        const string SM = @"[F10] Suggest";
        public bool Suggest = false;
        public bool KeyUpDown = false;
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
            KeyUpDown = !KeyUpDown;
            if (KeyUpDown)
            {
                if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.F2)
                {
                    button3.PerformClick();
                }
                if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.F4)
                {
                    button4.PerformClick();
                }
                if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.F7)
                {
                    button10.PerformClick();
                }
                if (e.KeyboardData.VirtualCode == GlobalKeyboardHook.F10)
                {
                    button7.PerformClick();
                }
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
                    if (!(PicBox is null))
                    panel1.Controls.Add(PicBox);
                }));
            }
        }
 
        public class Prompt : IDisposable
        {
            private Form prompt { get; set; }
            public string Result { get; }

            public Prompt(string text, string caption, string fen)
            {
                Result = ShowDialog(text, caption, fen);
            }
            private const string StartFen = " w KQkq - 0 1";
            private string ShowDialog(string text, string caption, string fen)
            {

                prompt = new Form()
                {
                    Width = 550,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen,
                    TopMost = true
                };
                Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 500, Text = fen+StartFen };
                Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 170, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }

            public void Dispose()
            {
                if (prompt != null)
                {
                    prompt.Dispose();
                }
            }
        }

        private void CustomRun(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                    isRunning = true;
                var detect = new Detection();
                var fen = ChessBoard.GetFenS(detect.DetectPos());
                using (Prompt prompt = new Prompt("check fen string", "Run from Custom position (beta)", fen))
                {
                    string result = prompt.Result;
                    CTPlay = new CancellationTokenSource();
                        Task.Run(() =>
                        {
                            if(!(result is null))
                            using (var pe = new MainThread(result))
                                pe.Run(CTPlay, this);
                            isRunning = false;
                        });
                }
            }
        }
        private void CancelBtn(object sender, EventArgs e)
        {
            if (CTPlay != null)
            {
                CTPlay.Cancel();
            }
        }
        private void OpenSettings(object sender, EventArgs e)
        {
            new SettingsStore();
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = SettingsStore.SettingsPath
                }
            }
            )
            {
                process.Start();
                process.WaitForExit();
            }
        }
        private void ToggleSuggest(object sender, EventArgs e)
        {
            Suggest = !Suggest;
            if (Suggest)
                button7.Text = SM;
            else
                button7.Text = TM;
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
            new Detection();
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
                Thread.Sleep(Displayfps);
            }
        }
        private const int Displayfps = 200;
        private void DisplayButton(object sender, EventArgs e)
        {
            if (button5.Text == "Test display")
            {
                CTDisplay = new CancellationTokenSource();
                Task.Run(() => DisplayThread(CTDisplay), CTDisplay.Token);
                button5.Text = "Stop";
            }
            else
            {
                CTDisplay.Cancel();
                Thread.Sleep(1);
                PicBox.Dispose();
                CTDisplay.Dispose();
                button5.Text = "Test display";
            }
        }
        private void RunBtn(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                isRunning = true;
                CTPlay = new CancellationTokenSource();
                Task.Run(() => {
                    var tsk = Task.Run(() =>
                    {
                        using (var pe = new MainThread())
                            pe.Run(CTPlay, this);
                        isRunning = false;
                    });
                    tsk.Wait();
                    tsk.Dispose();
                });
            }
        }
  }
}
