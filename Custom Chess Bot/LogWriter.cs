using System;
using System.IO;

namespace Custom_Chess_Bot
{
    public class LogWriter
    {
        private readonly bool Enable;
        public LogWriter(SettingsStore settings)
        {
            Enable = settings.LogEnable;
        }
        public const string ML = @"[ML]";
        public const string FoundTurn = @"[FoundTurn]";
        public const string Processing = @"[Processing]";
        public const string DetectedSide = @"[DetectedSide]";
        public const string Search = @"[Search]";
        public const string Turn = @"[Turn]";
        public const string Me = @"[Me]";
        public const string Enemy = @"[Enemy]";
        public const string SpawnThread = @"[SpawnThread]";
        public void Report(string turn)
        {
            Log(Turn, turn);
        }
        public void Report(Side side)
        {
            Log(DetectedSide, side.ToString());
        }
        public void Report(int equalElements, float treshold, int cell)
        {
            Log(Processing, "[equal]" + equalElements + "[tres]" + treshold + "[cell]" + cell);
        }
        private void Log(string flags, string report)
        {
            try
            {
                if (Enable)
                    File.AppendAllText(SettingsStore.LogPath, flags + ":" + report + Environment.NewLine);
            }
            catch
            {
                new ArgumentException("Fail to write log file.");
            }
        }

    }
}
