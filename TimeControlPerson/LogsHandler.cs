using System;
using System.IO;

namespace TimeControlPerson
{
    class LogsHandler
    {
        public enum Status
        {
            INFO = 0,
            ERROR = 1
        }
        public static void LogWriter(Status status, string message)
        {
            DateTime dateTime = DateTime.Now;

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            using (StreamWriter streamWriter = new StreamWriter(Environment.CurrentDirectory + @"\logs\" + DateTime.Today.ToString("yyyy-MM-dd") + ".log", true))
            {
                streamWriter.WriteLine($"[{dateTime}] {status} - {message}");
            }
        }
    }
}
