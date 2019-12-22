using System;
using System.IO;

namespace Custom_Chess_Bot
{
    class Logger
    {
        static Settings settings = new Settings();
        public const string ML = @"[ML]";
        public const string FoundTurn = @"[FoundTurn]";
        public const string Processing = @"[Processing]";
        public const string Side = @"[Side(white)]";
        public const string Search = @"[Search]";
        public const string Turn = @"[Turn]";
        public const string Me = @"[Me]";
        public const string Enemy = @"[Enemy]";
        public const string SpawnThread = @"[SpawnThread]";
        internal void Log(string flags, string report)
        {
            try
            {
                if (settings.LogEnable)
                    File.AppendAllText(Settings.LogPath, flags + ":" + report + Environment.NewLine);
            }
            catch
            {
                new ArgumentException("logger");
            }
        }

    }
}
