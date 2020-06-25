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
        public static string DetectionPresetPath = @"DP.txt";
        public static string LogPath = @"Logs.txt";
        public const int BoardLenght = 8;
        public int Hash;
        public int MissplayTurns;
        public float Window;
        public float Threshold;
        public string EnginePath;
        public Size BoardSize;
        public Point BoardPosition;
        public int AnimationDelay;
        public int RefreshDelay;
        public int TurnMinDelay;
        public int TurnMaxDelay;
        public float DelayPart;
        public float WhiteBright;
        public float BlackBright;
        public bool LogEnable;
        public int TurnsForExtraDelay;
        public int MoveTime;
        public int MouseClickDelay;
        public string Option1;
        public string Option2;
        public string Option3;
        public SettingsStore()
        {
            Load();
        }
        public void Save()
        {
            var settings =
            "(calibrate)BoardSize.Width|" + BoardSize.Width.ToString() + Environment.NewLine +
            "(calibrate)BoardSize.Height|" + BoardSize.Height.ToString() + Environment.NewLine +
            "(calibrate)BoardPosition.X|" + BoardPosition.X.ToString() + Environment.NewLine +
            "(calibrate)BoardPosition.Y|" + BoardPosition.Y.ToString() + Environment.NewLine +
            "(speed)Hash|" + Hash.ToString() + Environment.NewLine +
            "(humanity)MissplayEveryXTurns|" + MissplayTurns.ToString() + Environment.NewLine +
            "(accuracy)Window|" + Window.ToString() + Environment.NewLine +
            "(accuracy)Treshold|" + Threshold.ToString() + Environment.NewLine +
            "StockfishPath|" + EnginePath.ToString() + Environment.NewLine +
            "(accuracy)AnimationDelay|" + AnimationDelay.ToString() + Environment.NewLine +
            "(speed)RefreshDelay|" + RefreshDelay.ToString() + Environment.NewLine +
            "(humanity)TurnDelayMin|" + TurnMinDelay.ToString() + Environment.NewLine +
            "(humanity)TurnDelayMax|" + TurnMaxDelay.ToString() + Environment.NewLine +
            "(accuracy)WhiteBrightTreshold|" + WhiteBright.ToString() + Environment.NewLine +
            "(accuracy)BlackBrightTreshold|" + BlackBright.ToString() + Environment.NewLine +
            "(debug)LogEnable|" + LogEnable.ToString() + Environment.NewLine +
            "(humanity)TurnsForExtraDelay|" + TurnsForExtraDelay.ToString() + Environment.NewLine +
            "(humanity)DelayPart|" + DelayPart.ToString() + Environment.NewLine +
            "(humanity)MouseClickDelay|" + MouseClickDelay.ToString() + Environment.NewLine +
            "MaxEngineTime|" + MoveTime.ToString() + Environment.NewLine +
            "(speed)AnimationHash|" + AnimationHash.ToString() + Environment.NewLine +
            "Option1|" + Option1.ToString() + Environment.NewLine +
            "Option2|" + Option2.ToString() + Environment.NewLine +
            "Option3|" + Option3.ToString();
            
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
                AnimationDelay = Convert.ToInt32(settings[9].Split("|")[1]);
                RefreshDelay = Convert.ToInt32(settings[10].Split("|")[1]);
                TurnMinDelay = Convert.ToInt32(settings[11].Split("|")[1]);
                TurnMaxDelay = Convert.ToInt32(settings[12].Split("|")[1]);
                WhiteBright = Convert.ToSingle(settings[13].Split("|")[1]);
                BlackBright = Convert.ToSingle(settings[14].Split("|")[1]);
                LogEnable = Convert.ToBoolean(settings[15].Split("|")[1]);
                TurnsForExtraDelay = Convert.ToInt32(settings[16].Split("|")[1]);
                DelayPart = Convert.ToSingle(settings[17].Split("|")[1]);
                MouseClickDelay = Convert.ToInt32(settings[18].Split("|")[1]);
                MoveTime = Convert.ToInt32(settings[19].Split("|")[1]);
                AnimationHash = Convert.ToInt32(settings[20].Split("|")[1]);
                Option1 = settings[21].Split("|")[1];
                Option2 = settings[22].Split("|")[1];
                Option3 = settings[23].Split("|")[1];
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
            EnginePath = @"stockfish_10_x32.exe";
            AnimationDelay = 0;
            RefreshDelay = 0;
            TurnMinDelay = 500;
            TurnMaxDelay = 2000;
            TurnsForExtraDelay = 20;
            DelayPart = 0.85f;
            WhiteBright = 0.9f;
            BlackBright = 0.35f;
            LogEnable = true;
            AnimationHash = 200;
            MissplayTurns = 3;
            MoveTime = 400;
            MouseClickDelay = 333;
            Option1 = @"null";
            Option2 = @"null";
            Option3 = @"null";
        }
        public void CalibrateBoard(Size boardSize, Point boardPosition)
        {
            BoardSize = boardSize;
            BoardPosition = boardPosition;
            Save();
        }


    }
}

