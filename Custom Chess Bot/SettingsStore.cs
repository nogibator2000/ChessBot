using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Chess_Bot
{
    public class SettingsStore
    {
        public int AnimationHash;
        public const int AnimationWindow = 0;
        public static string SettingsPath = @"Settings.txt";
        public static string LogPath = @"Logs.txt";
        public const int BoardLenght = 8;
        public int Hash;
        public int MissplayTurns;
        public float Window;
        public float Threshold;
        public string EnginePath;
        public Size BoardSize;
        public Point BoardPosition;
        public int NeutralColor, Color1, Color2, Color3, Color4;
        public bool FilterEnable;
        public int AnimationDelay;
        public int RefreshRate;
        public int TurnMinDelay;
        public int TurnMaxDelay;
        public float WhiteBright;
        public float BlackBright;
        public bool LogEnable;
        public SettingsStore()
        {
            Load();
        }
        public void Save()
        {
            var settings =
            "BoardSize.Width|" + BoardSize.Width.ToString() + Environment.NewLine +
            "BoardSize.Height|" + BoardSize.Height.ToString() + Environment.NewLine +
            "BoardPosition.X|" + BoardPosition.X.ToString() + Environment.NewLine +
            "BoardPosition.Y|" + BoardPosition.Y.ToString() + Environment.NewLine +
            "(speed)Hash|" + Hash.ToString() + Environment.NewLine +
            "(you will get banned w/o it)MissplayEveryXTurns|" + MissplayTurns.ToString() + Environment.NewLine +
            "(accuracy)Window|" + Window.ToString() + Environment.NewLine +
            "(accuracy)Treshold|" + Threshold.ToString() + Environment.NewLine +
            "(StockfishPath|" + EnginePath.ToString() + Environment.NewLine +
            "(filter)NeutralColor|" + NeutralColor.ToString() + Environment.NewLine +
            "(filter)Color1|" + Color1.ToString() + Environment.NewLine +
            "(filter)Color2|" + Color2.ToString() + Environment.NewLine +
            "(filter)Color3|" + Color3.ToString() + Environment.NewLine +
            "(filter)Color4|" + Color4.ToString() + Environment.NewLine +
            "FilterEnable|" + FilterEnable.ToString() + Environment.NewLine +
            "AnimationRefreshRate|" + AnimationDelay.ToString() + Environment.NewLine +
            "RefreshRate|" + RefreshRate.ToString() + Environment.NewLine +
            "TurnDelayMin|" + TurnMinDelay.ToString() + Environment.NewLine +
            "TurnDelayMax|" + TurnMaxDelay.ToString() + Environment.NewLine +
            "WhiteBrightTreshold|" + WhiteBright.ToString() + Environment.NewLine +
            "BlackBrightTresholdr|" + BlackBright.ToString() + Environment.NewLine +
            "(debug)LogEnable|" + LogEnable.ToString() + Environment.NewLine +
            "(speed)AnimationHash|" + AnimationHash.ToString();            
            File.WriteAllText(SettingsPath, settings);
        }
        private void Load()
        {
            try {
                var settings = File.ReadAllLines(SettingsPath);
                BoardSize.Width = Convert.ToInt32(settings[0].Split("|")[1]);
                BoardSize.Height = Convert.ToInt32(settings[1].Split("|")[1]);
                BoardPosition.X = Convert.ToInt32(settings[2].Split("|")[1]);
                BoardPosition.Y = Convert.ToInt32(settings[3].Split("|")[1]);
                Hash = Convert.ToInt32(settings[4].Split("|")[1]);
                MissplayTurns = Convert.ToInt32(settings[5].Split("|")[1]);
                Window = Convert.ToSingle(settings[6].Split("|")[1]);
                Threshold = Convert.ToSingle(settings[7].Split("|")[1]);
                EnginePath = settings[8].Split("|")[1];
                NeutralColor = Convert.ToInt32(settings[9].Split("|")[1]);
                Color1 = Convert.ToInt32(settings[10].Split("|")[1]);
                Color2 = Convert.ToInt32(settings[11].Split("|")[1]);
                Color3 = Convert.ToInt32(settings[12].Split("|")[1]);
                Color4 = Convert.ToInt32(settings[13].Split("|")[1]);
                FilterEnable = Convert.ToBoolean(settings[14].Split("|")[1]);
                AnimationDelay = Convert.ToInt32(settings[15].Split("|")[1]);
                RefreshRate = Convert.ToInt32(settings[16].Split("|")[1]);
                TurnMinDelay = Convert.ToInt32(settings[17].Split("|")[1]);
                TurnMaxDelay = Convert.ToInt32(settings[18].Split("|")[1]);
                WhiteBright = Convert.ToSingle(settings[19].Split("|")[1]);
                BlackBright = Convert.ToSingle(settings[20].Split("|")[1]);
                LogEnable = Convert.ToBoolean(settings[21].Split("|")[1]);
                AnimationHash = Convert.ToInt32(settings[22].Split("|")[1]);
            }
            catch
            {
                InitilizeDefault();
                Save();
            }
        }
        public void InitilizeDefault()
        {
            BoardSize.Width = 779;
            BoardSize.Height = 779;
            BoardPosition.X = 229;
            BoardPosition.Y = 170;
            Hash = 32;
            Window = 0.2f;
            Threshold = 0.9f;
            EnginePath = @"C:\Users\nogib\Downloads\stockfish-10-win\stockfish-10-win\Windows\stockfish_10_x64.exe";
            NeutralColor = Color.FromArgb(127, 127, 127).ToArgb();
            Color1 = Color.FromArgb(118, 150, 86).ToArgb();
            Color2 = Color.FromArgb(186, 202, 68).ToArgb();
            Color3 = Color.FromArgb(246, 246, 130).ToArgb();
            Color4 = Color.FromArgb(238, 238, 210).ToArgb();
            FilterEnable = false;
            AnimationDelay = 30;
            RefreshRate = 50;
            TurnMinDelay = 500;
            TurnMaxDelay = 2000;
            WhiteBright = 0.9f;
            BlackBright = 0.35f;
            LogEnable = true;
            AnimationHash = 200;
            MissplayTurns = 4;
        }
        public void CalibrateBoard(Size boardSize, Point boardPosition)
        {
            BoardSize = boardSize;
            BoardPosition = boardPosition;
            Save();
        }


    }
}

