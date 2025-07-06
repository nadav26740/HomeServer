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
        
        public ProcessLogger(string ProcessName)
        {
            Directory.CreateDirectory($"Logs/{ProcessName}"); // Ensure the directory exists
            // Create a log file with the current date and time
            this.m_Logs_path = $"Logs/{ProcessName}/{DateTime.Now:yyyy-MM-dd (HH-mm-ss)}.log";
            m_ProcessName = ProcessName;

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

        public void LogInfo(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO - {message}";
            Console.WriteLine($"({m_ProcessName}) {logMessage}");
            
            if (LogFileWriter != null)
            {
                LogFileWriter.WriteLine(logMessage);
            }
        }

        public void LogError(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR - {message}";
            Console.WriteLine($"({m_ProcessName}) {logMessage}");
            
            if (LogFileWriter != null)
            {
                LogFileWriter.WriteLine(logMessage);
            }
        }
    }
}
