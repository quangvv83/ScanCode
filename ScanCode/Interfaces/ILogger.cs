using System;

namespace ScanCode.Interfaces
{
    public interface ILogger
    {
        void Fatal(object type, string message, Exception ex);
        void Error(object type, string message, Exception ex);
        void Warn(object type, string message);
        void Debug(object type, string message);
        void Info(object type, string message);
    }
}
