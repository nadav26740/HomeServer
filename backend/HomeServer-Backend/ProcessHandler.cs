using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public class ProcessHandler
    {
        public struct ProcessInfo
        {
            public string Name;
            public string Path;
            public string Arguments;
            public string WorkingDirectory;
            public string? ExitCodeInput;

            public ProcessInfo(string name, string path, string arguments, string workingDirectory, string? exitCodeInput = null)
            {
                Name = name;
                Path = path;
                Arguments = arguments;
                WorkingDirectory = workingDirectory;
                ExitCodeInput = exitCodeInput;
            }

            public override string ToString()
            {
                return $"Process Name: {Name}\n" +
                    $"Process Path: {Path}\n" +
                    $"Process Arguments: {Arguments}\n" +
                    $"Working Directory: {WorkingDirectory}" +
                    (ExitCodeInput != null ? $"\nExit Code Input: {ExitCodeInput}" : "");
            }
        }

        // Fields
        private ProcessInfo m_info;
        private Process m_process;
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
            m_info = info;
            m_process = new Process();

            // throw new Exception("ProcessHandler not implemented yet!");
        }

        ~ProcessHandler()
        {
            StopProcess();
        }

        public void StopProcess()
        {
            // TODO 
            m_logger.LogInfo($"Stopping process {m_info.Name} with PID: {m_process.Id}");
            if (IsRunning)
            {
                if (m_info.ExitCodeInput != null)
                {
                    WriteToProcess(m_info.ExitCodeInput);
                }

                m_process.Kill();
                m_process.WaitForExit();
                m_process.Close();

                m_logger.LogInfo($"Process Closed!");
            }
            else
            {
                m_logger.LogError("Process Already Closed!");
            }
            try
            {
                m_process.Dispose();
            }
            catch (Exception ex)
            {
                m_logger.LogError($"Error disposing process: {ex.Message}");
            }
            finally
            {
                m_process = null;
                m_logger = null;
                Logger.LogInfo($"Process {m_info.Name} stopped.");
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
                m_logger.LogInfo($"Writing to process: {input}");
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
            m_logger = new ProcessLogger(m_info.Name);
            m_process.OutputDataReceived += OutputLog;
            m_process.ErrorDataReceived += OutputError;

            // Starting Process
            m_process.Start();
            m_process.BeginOutputReadLine();
            m_process.BeginErrorReadLine();

            Logger.LogInfo($"Process {m_info.Name} started with PID: {m_process.Id}");
        }

        public override string ToString()
        {
            return m_info.ToString() + 
                $"\nRunning: {this.IsRunning}\n" +
                $"Process ID: {m_process.Id}\n" +
                $"Logger: {(m_logger != null ? "Enabled" : "Disabled")}\n" +
                $"Logger Path: {(m_logger != null ? m_logger.m_Logs_path : "N/A")}";
        }
    }
}
