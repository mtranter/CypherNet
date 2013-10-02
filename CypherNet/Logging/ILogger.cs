using System;
using System.Diagnostics;

namespace CypherNet.Logging
{
    public interface ILogger
    {
        void Log(string message, LogLevel logLevel);
    }

    class Logger
    {
        static Logger()
        {
            Current = new EmptyLogger();
        }

        internal static ILogger Current { get; set; }
    }

    public class TraceLogger : ILogger
    {
        public void Log(string message, LogLevel logLevel)
        {
            Trace.WriteLine(String.Format("{0}: {1}", logLevel, message));
        }
    }

    internal class EmptyLogger : ILogger
    {
        public void Log(string message, LogLevel logLevel)
        {
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
}
