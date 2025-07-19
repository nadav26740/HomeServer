using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public class ProcessLogger
    {
        public string m_Logs_path { get; }
        private string m_ProcessName = "HomeServer_Backend";
        private StreamWriter? LogFileWriter;
        private Mutex LogFileMutex;

        private const int MaxLogsInMemory = 25;
        public Queue<Tuple<DateTime, string>> LastErrors { get; } = new Queue<Tuple<DateTime, string>>(MaxLogsInMemory); // Store last 10 logs
        public Queue<Tuple<DateTime, string>> LastLogs { get; } = new Queue<Tuple<DateTime, string>>(MaxLogsInMemory); // Store last 10 logs
        
        private DateTime LastErrorTimestamp = DateTime.MaxValue;
        private DateTime LastLogTimestamp = DateTime.MaxValue;

        public string GetLastLogs()
        {
            if (!LogFileMutex.WaitOne(1000))
            {
                throw new Exception("Failed to acquire mutex for log file access.");
            }

            LogFileMutex.ReleaseMutex();
            return "";
        }

        /// <summary>
        /// Creating Process logger to allow specification of the process's logs.
        /// </summary>
        /// <param name="ProcessTag">The Tag name of the process</param>
        public ProcessLogger(string ProcessTag)
        {
            LogFileMutex = new Mutex();
            
            Directory.CreateDirectory($"Logs/{ProcessTag}"); // Ensure the directory exists
            // Create a log file with the current date and time
            this.m_Logs_path = $"Logs/{ProcessTag}/{DateTime.Now:yyyy-MM-dd (HH-mm-ss)}.log";
            m_ProcessName = ProcessTag;

            Logger.LogInfo("ProcessLogger initialized with path: " + m_Logs_path);
            
            try
            {
                LogFileWriter = new StreamWriter(path: m_Logs_path, append: true);
                LogFileWriter.AutoFlush = true; // Ensure that the log is written immediately
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to open log file at {m_Logs_path}: {ex.Message}");
                LogFileWriter = null;
            }
        }

        ~ProcessLogger()
        {
            if (LogFileWriter != null)
            {
                LogFileWriter.Close();
                LogFileWriter.DisposeAsync();
            }
        }

        /// <summary>
        /// Logging info messages to the log file and console.
        /// </summary>
        /// <param name="message">Log Message</param>
        public void LogInfo(string message)
        {
            this.P_Log($"INFO - {message}");
        }

        private void P_Log(string message)
        {
            if (LastLogs.Count >= MaxLogsInMemory)
            {
                LastLogs.Dequeue();
            }

            LastLogs.Enqueue(new Tuple<DateTime, string>(DateTime.Now, message));


            Console.WriteLine($"({m_ProcessName}) [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");

            if (LogFileWriter != null)
            {
                LogFileMutex.WaitOne();
                LogFileWriter.WriteLine(message);
                LogFileMutex.ReleaseMutex();
            }
        }

        public void LogError(string message)
        {
            string formatedLog = $"ERROR - {message}";
            this.P_Log(formatedLog);

            if (LastErrors.Count >= MaxLogsInMemory)
            {
                LastErrors.Dequeue();
            }

            LastErrors.Enqueue(new Tuple<DateTime, string>(DateTime.Now, formatedLog));
        }
    }
}
