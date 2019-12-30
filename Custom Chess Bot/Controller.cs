using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Custom_Chess_Bot
{
    public class Controller
    {
        private readonly Point FirstPoint;
        private readonly Size CellSize;
        [DllImport("user32.dll")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
//        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
//        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public Controller(SettingsStore settings)
        {
            FirstPoint = new Point(settings.BoardSize.Width / SettingsStore.BoardLenght / 2+settings.BoardPosition.X, settings.BoardSize.Height / SettingsStore.BoardLenght / 2+settings.BoardPosition.Y);
            CellSize = new Size(settings.BoardSize.Width / SettingsStore.BoardLenght, settings.BoardSize.Height / SettingsStore.BoardLenght);
        }
        private void SetPosition(Point pos)
        {
            Cursor.Position = pos;
        }
        private void MakeMouseClick(int X, int Y)
        {
            SetPosition(new Point(X, Y));
            Thread.Sleep(1);
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)X, (uint)Y, 0, 0);
            Thread.Sleep(1);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)X, (uint)Y, 0, 0);
        }
        private void MakeTransform(int X, int Y)
        {
            MakeMouseClick(X, Y);
        }
        public void MakeTurn(Turn turn)
        {
            if(!(turn is null))
            {
                turn.ToInt(out int ws, out int ns, out int we, out int ne);
                ns = SettingsStore.BoardLenght - 1 - ns;
                ne = SettingsStore.BoardLenght - 1 - ne;
                MakeMouseClick(FirstPoint.X + ws * CellSize.Width, FirstPoint.Y + ns * CellSize.Height);
                MakeMouseClick(FirstPoint.X + we * CellSize.Width, FirstPoint.Y + ne * CellSize.Height);
                if (turn.Promote)
                    MakeTransform(FirstPoint.X + we * CellSize.Width, FirstPoint.Y + ne * CellSize.Height);
            }
        }
        public string SimulateMakeTurn(Turn turn)
        {
            var str = ""; 
            if (!(turn is null))
            {
                turn.ToInt(out int ws, out int ns, out int we, out int ne);
                ns = SettingsStore.BoardLenght - 1 - ns;
                ne = SettingsStore.BoardLenght - 1 - ne;
                str += (FirstPoint.X + ws * CellSize.Width)+", "+ (FirstPoint.Y + ns * CellSize.Height)+" ";
                str += (FirstPoint.X + we * CellSize.Width) + ", " + (FirstPoint.Y + ne * CellSize.Height)+" ";
                if (turn.Promote)
                    str += (FirstPoint.X + we * CellSize.Width)+ ", "+ (FirstPoint.Y + ne * CellSize.Height);
            }
            return str;
        }

    }
}
