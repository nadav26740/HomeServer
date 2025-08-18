using System;
using System.Collections.Generic;
using System.Diagnostics;
using HomeServer_Backend.ExtensionsLibs;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace HomeServer_Backend
{
    public class ProcessHandler
    {
        protected class CacheData
        {
            private const double default_TTL_Seconds = 3;
            
            public CacheData(object data, DateTime? expressionTime = null)
            {
                this.ExpressionTime = expressionTime ?? DateTime.Now.AddSeconds(default_TTL_Seconds);
                this.Data = data;
            }

            public object Data;
            public DateTime ExpressionTime;
        }

        enum CacheType
        {
            MemoryUsage,
            ChildrensMemoryUsage,
            CPUUsage
        }

        private Dictionary<CacheType, CacheData> ResourceCache = new();

        /// <summary>
        /// Check the cache for the specified type and return the cached data if it exists and is not expired.
        /// </summary>
        /// <param name="type"> The Type of cache to get </param>
        /// <returns> The cache value </returns>
        private object? CheckCache(CacheType type)
        {
            if (ResourceCache.TryGetValue(type, out CacheData? cacheData))
            {
                if (cacheData.ExpressionTime > DateTime.Now)
                {
                    m_logger?.LogInfo("Getting data from cache: " + type.ToString() + " - " + cacheData.ExpressionTime.ToString("yyyy-MM-dd HH:mm:ss") + " - " + cacheData.Data.ToString());
                    return cacheData.Data;
                }
                else
                {
                    ResourceCache.Remove(type);
                }
            }
            m_logger?.LogInfo("Cache miss for type: " + type.ToString() + ". Data is either expired or not found.");
            return null;
        }

        /// <summary>
        /// load data into the cache with an optional expiration time.
        /// </summary>
        private void LoadIntoCache(CacheType type, object data, DateTime? expressionTime = null)
        {
            m_logger?.LogInfo($"Cache Loaded with {type.ToString()} - {data.ToString()} - Expiration Time: {(expressionTime.HasValue ? expressionTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "No expiration")}");
            ResourceCache[type] = new CacheData(data, expressionTime);
        }

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

            public string DeserilizeToJson() => 
                "{\n" + 
                    $"\t\"{nameof(Tag)}\": \"{Tag}\", \n" +
                $"\t\"{nameof(Path)}\": \"{Path}\", \n" +
                $"\t\"{nameof(Arguments)}\": \"{Arguments}\", \n" +
                $"\t\"{nameof(WorkingDirectory)}\": \"{WorkingDirectory.Replace('\\', '/')}\", \n" +
                $"\t\"{nameof(ExitCodeInput)}\": {(ExitCodeInput != null ? $"\"{ExitCodeInput}\"" : "")} \n" +
                "}";
        }

        // Fields
        private ProcessInfo m_info;
        public ProcessInfo Info { get { return m_info; } }
        private Process? m_process;
        private ProcessLogger? m_logger;
        private bool Started = false; // by using that we don't need to check when we know thre is no chance the process running

        public bool IsRunning { 
            get {   
                try 
                {
                    // IF not started for sure isn't running
                    if (!Started)
                        return false;

                    Started = m_process != null && !m_process.HasExited;
                    return Started;
                } 
                catch (Exception ex)// if error occur while checking just return false
                { 
                    m_logger?.LogError($"Error checking if process {m_info.Tag} is running: {ex.Message}");
                    return false; 
                } } }

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

        // ==================== CPU ====================

        // CPU RECORDS
        DateTime LastCPUUsageCheck;
        double LastCPUTotalProcessorTimeCheck = 0;
        
        public double GetProcessCPUUsage()
        {
            const double TTL_SECONDS = 3; // Cache TTL in seconds

            // checking if we have cached value
            double? cachedValue = CheckCache(CacheType.CPUUsage) as double?;
         
            if (cachedValue.HasValue)
            {
                return cachedValue.Value;
            }

            if (m_process == null || m_process.HasExited)
            {
                throw new InvalidOperationException("Process is not running or has already exited.");
            }

            m_process.Refresh();

            try
            {
                // Calculating the cpu usage
                double cpuTimeSinceLastCheck = (DateTime.Now - LastCPUUsageCheck).TotalMilliseconds;
                double averageCpuUsage = (m_process.TotalProcessorTime.TotalMilliseconds - LastCPUTotalProcessorTimeCheck) / cpuTimeSinceLastCheck;
                LastCPUTotalProcessorTimeCheck = m_process.TotalProcessorTime.TotalMilliseconds;
                LastCPUUsageCheck = DateTime.Now;

                // Loading it into cache
                cachedValue = averageCpuUsage / Environment.ProcessorCount;
                LoadIntoCache(CacheType.CPUUsage, cachedValue, DateTime.Now.AddSeconds(TTL_SECONDS));

                return cachedValue ?? 0;
            }
            catch (Exception ex)
            {
                m_logger?.LogError($"Error retrieving CPU usage: {ex.Message}");
                return -1;
            }
        }

        // ==================== MEMORY ====================

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
                
                m_logger?.LogInfo($"Retrieving child processes memory for PID: {m_process?.Id}");
                foreach (Process obj in this.m_process?.GetChildProcesses())
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

        public long GetTotalMemoryUsage()
        {
            if (!this.IsRunning)
            {
                Logger.LogError($"Failed To Get Total Memory Usage of {this.m_info.Tag} Process is not running or has already exited.");
                return -1;
                // throw new InvalidOperationException("Process is not running or has already exited.");
            }

            long totalMemory = GetMemoryUsage() + GetChildrensMemoryUsage();
            // m_logger?.LogInfo($"Total memory usage for PID {m_process.Id}: {totalMemory} bytes");
            return totalMemory;
        }

        public string GetTotalMemoryUsageString()
        {
            return ProcessExtensions.BytesToFormatedString(GetTotalMemoryUsage());
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

        /// <summary>
        /// Stopping the process if it is running.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// throwing error if process is not existing.
        /// </exception>
        public void StopProcess()
        {
            if (m_process == null)
            {
                m_logger?.LogError("Process is not running or has already exited.");
                throw new InvalidOperationException("no processes is running!.");
            }

            // TODO 
            if (IsRunning)
            {
                m_logger?.LogInfo($"Stopping process {m_info.Tag} with PID: {m_process?.Id}");
                if (m_info.ExitCodeInput != null && m_info.ExitCodeInput != string.Empty)
                {
                    WriteToProcess(m_info.ExitCodeInput);
                }

                m_process?.Kill();
                m_process?.WaitForExit();
                m_process?.Close();

                m_logger?.LogInfo($"Process Closed!");
            }
            else
            {
                m_logger?.LogError("Process Already Closed!");
            }

            try
            {
                m_process?.Dispose();
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

        /// <summary>
        /// Starting the processes
        /// </summary>
        /// <exception cref="InvalidOperationException">exception if process is already not running</exception>
        public void StartProcess()
        {
            if (this.IsRunning)
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
            m_process.Exited += ProcessExitedEvent;

            // Starting Process
            m_process.Start();
            m_process.BeginOutputReadLine();
            m_process.BeginErrorReadLine();
            
            // Reseting CPU records
            LastCPUTotalProcessorTimeCheck = 0;
            LastCPUUsageCheck = DateTime.Now;

            m_logger.LogInfo($"Process {m_info.Tag} started with PID: {m_process.Id}");
            Started = true;
        }

        private void ProcessExitedEvent(object? sender, EventArgs e)
        {
            m_logger?.LogError($"Process {m_process?.ProcessName} Exited!");
         }

        public override string ToString()
        {
            return m_info.ToString() + 
                $"\nRunning: {this.IsRunning}\n" +
                $"Process Name: {GetProcessName()}\n" +
                $"Process ID: {m_process?.Id}\n" +
                $"Logger: {(m_logger != null ? "Enabled" : "Disabled")}\n" +
                $"Logger Path: {(m_logger != null ? m_logger.m_Logs_path : "N/A")}";
        }

        public Queue<Tuple<long, string>> GetLastLogs()
        {
            return m_logger != null ? m_logger.LastLogs : new Queue<Tuple<long, string>>(0);
        }

        public Queue<Tuple<long, string>> GetLastErrors()
        {
            return m_logger != null ? m_logger.LastErrors : new Queue<Tuple<long, string>>(0);
        }

        public DateTime GetProcessStartTime()
        {
            if (m_process == null || m_process.HasExited)
                return DateTime.MinValue;

            return m_process.StartTime;
        }
    }
}
