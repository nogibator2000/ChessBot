using System;
using Xunit;
using Custom_Chess_Bot;
using System.IO;

namespace UnitTests
{
    public class WriteLogTests
    {
        [Fact]
        public void CheckLogFile()
        {
            File.WriteAllText(SettingsStore.LogPath, "");
            var settings = new SettingsStore();
            var log = new LogWriter(settings);
            log.Report(new Side(true));
            log.Report("a1a3");
            log.Report(10, 0.3f, 10);
            var file = File.ReadAllText(SettingsStore.LogPath);
            Assert.False(file == "");
        }
    }
}
