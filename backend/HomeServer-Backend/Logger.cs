using HomeServer_Backend.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public class Logger
    {
        private static string _logPath = Loader.ConfigData.default_config_path;
        private static Logger? m_instance = null;
        public static Logger Instance { get {  return GetIntance(); } }
     
        private Logger()
        {
            // TODO: Initialize logging settings here if needed
        }

        // Core!
        /// <summary>
        /// Intance to the singleton Logger class.
        /// </summary>
        /// <returns>singleton Logger Intance</returns>
        private static Logger GetIntance()
        {
            if (m_instance == null)
            {
                m_instance = new Logger();
            }
            return m_instance;
        }
        
        /// <summary>
        /// Logging core function.
        /// </summary>
        /// <param name="message"></param>
        private void _Log(string message)
        {
            if (!Config.data.EnableLogging)
                return;

            // Write logs
            // TODO
            Console.WriteLine(message);
        }

        // Changing Log Vars
        /// <summary>
        /// Changing the log file path.
        /// </summary>
        /// <param name="path">New Log path</param>
        /// <exception cref="Exception"> Logger already running </exception>
        public static void SetLoggingPath(string path)
        {
            if (m_instance != null)
            {
                throw new Exception("unable to change logging path after log started");
            }

            _logPath = path;
        }

        /// <summary>
        /// Restarting the logger with a new path.
        /// </summary>
        /// <param name="path">New Log path</param>
        public static void ForceSetLoggingPath(string path)
        {
            LogWarn($"Changing Log Path to: {path}");
            _logPath = path;
            if (Logger.m_instance != null)
                m_instance = new Logger();
        }

        // Loggers Types
        // Log info
        private void _LogInfo(string message)
        {
            string logMessage = $"[{DateTime.Now.ToString()}] Info: {message}";        
            this._Log(logMessage);
        }

        public static void LogInfo(string message)
        {
            GetIntance()._LogInfo(message);
        }

        // Warn Info
        private void _LogWarn(string message)
        {
            string logMessage = $"[{DateTime.Now.ToString()}] Warn: {message}";
            this._Log(logMessage);
        }

        public static void LogWarn(string message)
        {
            GetIntance()._LogWarn(message);
        }   

        // Error Info
        private void _LogError(string message)
        {
            string logMessage = $"[{DateTime.Now.ToString()}] Error: {message}";
            this._Log(logMessage);
        }

        public static void LogError(string message)
        {
            GetIntance()._LogError(message);
        }
    }
}
