using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace Custom_Chess_Bot
{
    public partial class Form1 : Form
    {
        PlayEngine pe;
        readonly Settings settings = new Settings();
        private GlobalKeyboardHook _globalKeyboardHook;

        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += KeyPressHandler;
        }


        public Form1()
        {
            SetupKeyboardHooks();
            InitializeComponent();
            var dt = Task.Run(() => DisplayThread());
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

        private async void Button1_Click(object sender, EventArgs e)
        {
            var imageAnalysis = new ImageAnalysis();
            var firstPosition = await GetMousePosition();
            log.Text = imageAnalysis.GetColourBrighness(firstPosition).ToString();
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            pe.CancelToken.Cancel();
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
            Thread.Sleep(250); //click duration
            log.Text = @"Click right-bottom board point.";
            var secondPosition = await GetMousePosition();
            var BoardSize = new Size(secondPosition.X - firstPosition.X, secondPosition.Y - firstPosition.Y);
            settings.CalibrateBoard(BoardSize, firstPosition);
            log.Text = @"Saved!";
        }
        private void DisplayThread()
        {
            while (true)
            {
                if (pe==null||pe.CancelToken.IsCancellationRequested)
                {
                    var ia = new ImageAnalysis();
                    DisplayBitMap(ia.CaptureScreen());
                    Thread.Sleep(Settings.RefreshRate);
                }
                else
                {
                    if (log.InvokeRequired)
                    {
                        log.Invoke(new MethodInvoker(delegate
                        {
                            log.Text = pe.log;
                        }));
                    }
                    Thread.Sleep(Settings.RefreshRate);
                }
            }
        }
    
        private void button3_Click(object sender, EventArgs e)
        {
            pe = new PlayEngine();
            var pt = Task.Run(() => pe.PlayThread());
 
        }
    }
}
