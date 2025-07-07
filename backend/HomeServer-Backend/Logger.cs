﻿using HomeServer_Backend.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public class Logger
    {

        private static string m_logPath = Loader.ConfigData.default_log_path;
        private static Logger? m_instance = null;
        private bool WriteToFile = true;
        private StreamWriter? LogFileWriter;
        Mutex FileUsageMutex;
        

        public static Logger Instance { get {  return GetIntance(); } }
     
        private Logger()
        {
            FileUsageMutex = new Mutex();
            // TODO: Initialize logging settings here if needed
            if (WriteToFile)
            {
                Directory.CreateDirectory(m_logPath); // Ensure the directory exists
                try
                {
                    LogFileWriter = new StreamWriter(path: $"{m_logPath}/{DateTime.Now:yyyy-MM-dd (HH-mm-ss)}.log", append: true);
                    LogFileWriter.AutoFlush = true; // Ensure that the log is written immediately
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to open log file at {m_logPath}/{DateTime.Now:yyyy-MM-dd (HH-mm-ss)}.log: {ex.Message}");
                    LogFileWriter = null;
                }

            }
        }

        ~Logger()
        {
            if (LogFileWriter != null)
            {
                LogFileWriter.Close();
                LogFileWriter.DisposeAsync();
            }
        }

        private void _LoadFromConfig()
        {
            // TODO: loading settings from config
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
        private async Task _Log(string message)
        {
            if (!Config.data.EnableLogging)
                return;

            // Write logs
            Console.WriteLine(message);

            Task.Run(() =>
            {
                if (LogFileWriter != null)
                {
                    FileUsageMutex.WaitOne(); // Ensure thread safety when writing to the file
                    LogFileWriter.WriteLine(message);
                    FileUsageMutex.ReleaseMutex();
                }
            });
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

            m_logPath = path;
        }

        /// <summary>
        /// Restarting the logger with a new path.
        /// </summary>
        /// <param name="path">New Log path</param>
        public static void ForceSetLoggingPath(string path)
        {
            LogWarn($"Changing Log Path to: {path}");
            m_logPath = path;
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
