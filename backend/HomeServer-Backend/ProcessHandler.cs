﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using HomeServer_Backend.ExtensionsLibs;
using System.Management;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace HomeServer_Backend
{
    public class ProcessHandler
    {
        

        public struct ProcessRunningData
        {
            public string ProcessName;
            public long MemoryUsage;
            public long ChildrensMemoryUsage;
            public bool Running;
            public int ProcessID;
            public Process[] Childrens; // TODO: What to do with it

        }

        public struct ProcessInfo
        {
            public string Tag;
            public string Path;
            public string Arguments;
            public string WorkingDirectory;
            public string? ExitCodeInput;

            public ProcessInfo(string name, string path, string arguments, string workingDirectory, string? exitCodeInput = null)
            {
                Tag = name;
                Path = path;
                Arguments = arguments;
                WorkingDirectory = workingDirectory;
                ExitCodeInput = exitCodeInput;
            }

            public ProcessInfo Clone()
            {
                return (ProcessInfo)this.MemberwiseClone();
            }

            public override string ToString()
            {
                return $"Process Tag: {Tag}\n" +
                    $"Process Path: {Path}\n" +
                    $"Process Arguments: {Arguments}\n" +
                    $"Working Directory: {WorkingDirectory}" +
                    (ExitCodeInput != null ? $"\nExit Code Input: {ExitCodeInput}" : "");
            }
        }

        // Fields
        private ProcessInfo m_info;
        public ProcessInfo Info { get { return m_info; } }
        private Process? m_process;
        private ProcessLogger? m_logger;
        public bool IsRunning { get { return m_process != null && !m_process.HasExited; } }

        // Logs
        private void OutputLog(object sender, DataReceivedEventArgs args) 
        {
            if (args.Data == null) return;
            if (m_logger != null)
            {
                m_logger.LogInfo(args.Data);
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO - {args.Data}");
            }
        }

        public int? GetProcessID()
        {
            if (m_process == null || m_process.HasExited)
            {
                return null;
            }
            return m_process.Id;
        }

        public string GetProcessName()
        {
            if (m_process == null || m_process.HasExited)
            {
                return string.Empty;
            }
            return m_process.ProcessName;
        }

        public long GetMemoryUsage()
        {
            if (m_process == null || m_process.HasExited)
            {
                throw new InvalidOperationException("Process is not running or has already exited.");
            }
            
            m_process.Refresh();

            return m_process.WorkingSet64;
        }

        public string GetMemoryUsageString()
        {
            if (m_process == null || m_process.HasExited)
            {
                throw new InvalidOperationException("Process is not running or has already exited.");
            }

            m_process.Refresh();

            return m_process.GetMemoryUsageFormated();
        }

        public long GetChildrensMemoryUsage()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException("Process is not running or has already exited.");
            }

            long totalMemory = 0;
            try
            {
                m_logger?.LogInfo($"Retrieving child processes memory for PID: {m_process.Id}");
                foreach (Process obj in this.m_process.GetChildProcesses())
                {
                    totalMemory += Convert.ToInt64(obj.WorkingSet64);
                }
            }
            catch (Exception ex)
            {
                m_logger?.LogError($"Error retrieving child processes memory: {ex.Message}");
            }

            return totalMemory;
        }

        public string GetChildrensMemoryUsageString()
        {
            return ProcessExtensions.BytesToFormatedString(GetChildrensMemoryUsage());
        }

        private void OutputError(object sender, DataReceivedEventArgs args)
        {
            if (args.Data == null) return;
            if (m_logger != null)
            {
                m_logger.LogInfo(args.Data);
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO - {args.Data}");
            }
        }

        public ProcessHandler(ProcessInfo info)
        {
            // TODO
            m_info = info.Clone();
            m_process = null;
            // throw new Exception("ProcessHandler not implemented yet!");
        }

        ~ProcessHandler()
        {
            StopProcess();
        }

        public void StopProcess()
        {
            // TODO 
            m_logger?.LogInfo($"Stopping process {m_info.Tag} with PID: {m_process.Id}");
            if (IsRunning)
            {
                if (m_info.ExitCodeInput != null)
                {
                    WriteToProcess(m_info.ExitCodeInput);
                }

                m_process.Kill();
                m_process.WaitForExit();
                m_process.Close();

                m_logger?.LogInfo($"Process Closed!");
            }
            else
            {
                m_logger?.LogError("Process Already Closed!");
            }

            try
            {
                m_process.Dispose();
            }
            catch (Exception ex)
            {
                m_logger?.LogError($"Error disposing process: {ex.Message}");
            }
            finally
            {
                m_process = null;
                m_logger = null;
                Logger.LogInfo($"Process {m_info.Tag} stopped.");
            }   
        }

        public void WriteToProcess(string input)
        {
            if (m_process == null || m_process.HasExited)
            {
                throw new InvalidOperationException("Process is not running.");
            }

            if (m_process.StartInfo.RedirectStandardInput)
            {
                m_logger?.LogInfo($"Writing to process: {input}");
                m_process.StandardInput.WriteLine(input);
                m_process.StandardInput.Flush();
            }
            else
            {
                throw new InvalidOperationException("Standard input is not redirected.");
            }
        }

        public void StartProcess()
        {
            if (m_process != null && m_process.HasExited)
            {
                throw new InvalidOperationException("Process is already running or has not been stopped properly.");
            }

            m_process = new Process();

            m_process.StartInfo = new ProcessStartInfo
            {
                FileName = m_info.Path,
                Arguments = m_info.Arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WorkingDirectory = m_info.WorkingDirectory,
                CreateNoWindow = false,
            };
            

            // Adding logs event
            m_logger = new ProcessLogger(m_info.Tag);
            m_process.OutputDataReceived += OutputLog;
            m_process.ErrorDataReceived += OutputError;

            // Starting Process
            m_process.Start();
            m_process.BeginOutputReadLine();
            m_process.BeginErrorReadLine();

            Logger.LogInfo($"Process {m_info.Tag} started with PID: {m_process.Id}");
        }

        public override string ToString()
        {
            return m_info.ToString() + 
                $"\nRunning: {this.IsRunning}\n" +
                $"Process Name: {GetProcessName()}\n" +
                $"Process ID: {m_process.Id}\n" +
                $"Logger: {(m_logger != null ? "Enabled" : "Disabled")}\n" +
                $"Logger Path: {(m_logger != null ? m_logger.m_Logs_path : "N/A")}";
        }

        public Queue<Tuple<DateTime, string>> GetLastLogs()
        {
            return m_logger != null ? m_logger.LastLogs : new Queue<Tuple<DateTime, string>>(0);
        }

        public Queue<Tuple<DateTime, string>> GetLastErrors()
        {
            return m_logger != null ? m_logger.LastErrors : new Queue<Tuple<DateTime, string>>(0);
        }
    }
}
