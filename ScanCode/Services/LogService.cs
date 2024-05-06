using ScanCode.Interfaces;
using ScanCode.Logging;
using ScanCode.Utils;
using System;
using System.IO;

namespace ScanCode.Services
{
    public class LogService : ILogger
    {
        public LogService()
        {
            Logger.Initialize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.LogPath));
        }

        public void Debug(object type, string message)
        {
            Logger.Debug(type, message);
        }

        public void Error(object type, string message, Exception ex)
        {
            Logger.Error(type, message, ex);
        }

        public void Fatal(object type, string message, Exception ex)
        {
            Logger.Fatal(type, message, ex);
        }

        public void Info(object type, string message)
        {
            Logger.Info(type, message);
        }

        public void Warn(object type, string message)
        {
            Logger.Warn(type, message);
        }
    }
}
