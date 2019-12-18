using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    class Settings
    {
        public string SettingsPath = @"Settings.txt";
        public readonly string[] syms = { "a", "b", "c", "d", "e", "f", "g", "h" };
        public string LogPath = @"log.txt";
        public const int RefreshRate = 250;
        public const int BoardLenght = 8;
        public int hash;
        public int MoveTime;
        public float window;
        public float threshold;
        public string uciPath;
        public Size BoardSize;
        public Point BoardPosition;
        public int NeutralColor, Color1, Color2, Color3, Color4;
        public bool FilterEnable;
        public int AnimationDelay;
        public int HumanBeingDelayMin;
        public int HumanBeingDelayMax;
        public float WhiteFilter;
        public float BlackFilter;
        public void Save()
        {
            var settings =
            BoardSize.Width.ToString() + Environment.NewLine +
            BoardSize.Height.ToString() + Environment.NewLine +
            BoardPosition.X.ToString() + Environment.NewLine +
            BoardPosition.Y.ToString() + Environment.NewLine +
            hash.ToString() + Environment.NewLine +
            MoveTime.ToString() + Environment.NewLine +
            window.ToString() + Environment.NewLine +
            threshold.ToString() + Environment.NewLine +
            uciPath.ToString() + Environment.NewLine +
            NeutralColor.ToString() + Environment.NewLine +
            Color1.ToString() + Environment.NewLine +
            Color2.ToString() + Environment.NewLine +
            Color3.ToString() + Environment.NewLine +
            Color4.ToString() + Environment.NewLine +
            FilterEnable.ToString() + Environment.NewLine +
            AnimationDelay.ToString() + Environment.NewLine +
            HumanBeingDelayMin.ToString() + Environment.NewLine +
            HumanBeingDelayMax.ToString() + Environment.NewLine +
            WhiteFilter.ToString() + Environment.NewLine +
            BlackFilter.ToString();
            File.WriteAllText(SettingsPath, settings);
        }
        private void Load()
        {
            try {
                var settings = File.ReadAllLines(SettingsPath);
                BoardSize.Width = Convert.ToInt32(settings[0]);
                BoardSize.Height = Convert.ToInt32(settings[1]);
                BoardPosition.X = Convert.ToInt32(settings[2]);
                BoardPosition.Y = Convert.ToInt32(settings[3]);
                hash = Convert.ToInt32(settings[4]);
                MoveTime = Convert.ToInt32(settings[5]);
                window = Convert.ToSingle(settings[6]);
                threshold = Convert.ToSingle(settings[7]);
                uciPath = settings[8];
                NeutralColor = Convert.ToInt32(settings[9]);
                Color1 = Convert.ToInt32(settings[10]);
                Color2 = Convert.ToInt32(settings[11]);
                Color3 = Convert.ToInt32(settings[12]);
                Color4 = Convert.ToInt32(settings[13]);
                FilterEnable = Convert.ToBoolean(settings[14]);
                AnimationDelay = Convert.ToInt32(settings[15]);
                HumanBeingDelayMin = Convert.ToInt32(settings[16]);
                HumanBeingDelayMax = Convert.ToInt32(settings[17]);
                WhiteFilter = Convert.ToSingle(settings[18]);
                BlackFilter = Convert.ToSingle(settings[19]);
            }
            catch
            {
                InitilizeDefault();
                Save();
            }
        }
        public void InitilizeDefault()
        {
            BoardSize.Width = 400;
            BoardSize.Height = 400;
            BoardPosition.X = 200;
            BoardPosition.Y = 200;
            hash = 32;
            MoveTime = 1600;
            window = 0.15f;
            threshold = 0.80f;
            uciPath = @"C:\Users\nogib\Downloads\stockfish-10-win\stockfish-10-win\Windows\stockfish_10_x64.exe";
            NeutralColor = Color.FromArgb(127, 127, 127).ToArgb();
            Color1 = Color.FromArgb(118, 150, 86).ToArgb();
            Color2 = Color.FromArgb(186, 202, 68).ToArgb();
            Color3 = Color.FromArgb(246, 246, 130).ToArgb();
            Color4 = Color.FromArgb(238, 238, 210).ToArgb();
            FilterEnable = true;
            AnimationDelay = 600;
            HumanBeingDelayMin = 50;
            HumanBeingDelayMax = 350;
            WhiteFilter = 0.75f;
            BlackFilter = 0.45f;
        }
        public Settings()
        {
            Load();
        }
        public void CalibrateBoard(Size boardSize, Point boardPosition)
        {
            BoardSize = boardSize;
            BoardPosition = boardPosition;
            Save();
        }
    }
}

