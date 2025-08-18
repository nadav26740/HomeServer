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

        private static string m_logPath = Loader.Config.data.LogPath;
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

        private void P_LoadFromConfig()
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
        private async void P_Log(string message)
        {
            // Write logs
            Console.WriteLine($"[{DateTime.Now.ToString()}] {message}");
            Console.ResetColor();

            await P_SaveLog(message); // Save log to file asynchronously
        }

        private async Task P_SaveLog(string message)
        {
            if (!Config.data.EnableLogging)
                return;

            await Task.Run(() =>
            {
                if (LogFileWriter != null)
                {
                    FileUsageMutex.WaitOne(); // Ensure thread safety when writing to the file
                    LogFileWriter.WriteLine($"[{DateTime.Now.ToString()}] {message}");
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
            if (path == m_logPath)
            {
                LogInfo($"Not changing Log Path because already set to: {path}");
                return;
            }

            LogWarn($"Changing Log Path to: {path}");
            m_logPath = path;
            if (Logger.m_instance != null)
                m_instance = new Logger();
        }

        // Loggers Types
        // Log info
        private void P_LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string logMessage = $"Info: {message}";        
            this.P_Log(logMessage);
        }

        public static void LogInfo(string message)
        {
            GetIntance().P_LogInfo(message);
        }

        // Warn Info
        private void P_LogWarn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string logMessage = $"Warn: {message}";
            this.P_Log(logMessage);
        }

        public static void LogWarn(string message)
        {
            GetIntance().P_LogWarn( message);
        }

        // Logging Debug
        public static void LogDebug(string message)
        {
            if (!Config.data.EnableDebugLogging)
                return;

            GetIntance().P_Log(message);
        }

        private async void P_LogDebug(string message)
        {
            string logMessage = $"Debug: {message}";

            Console.ForegroundColor = ConsoleColor.Cyan;
            this.P_Log($"[{DateTime.Now.ToString()}] {logMessage}");
        }

        // Error Info
        private async void P_LogError(string message)
        {
            string logMessage = $"Error: {message}";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"[{DateTime.Now.ToString()}] {logMessage}");
            Console.ResetColor();

            await this.P_SaveLog(logMessage);
        }

        public static void LogError(string message)
        {
            GetIntance().P_LogError(message);
        }
    }
}
