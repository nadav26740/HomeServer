using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public partial class ProcessesManager
    {
        public enum ProcessPriority : Int16
        {
           Core,
           High,
           Normal,
           Low
        }

        public class ProcessSlave
        {
            // We using diffrenet class for logs to avoid live changes issues
            public class LogsEventArgs : EventArgs
            {
                public Tuple<long, string>[] logs { get; }

                public LogsEventArgs(Tuple<long, string>[] logsMessage)
                {
                    logs = logsMessage;
                }
            }

            // Priority of the process for control and management
            public ProcessPriority Proc_Priority { get; set; }
            
            // process
            public ProcessHandler ProcessHandler { get; }

            // Process information
            /// <summary>
            /// Process start time
            /// </summary>
            public DateTime ProcessStartTime { get { return GetProcessDateTime(); } }
            
            /// <summary>
            /// Cache for optimization of process start time retrieval.
            /// will get reset anytime the process will be recognized as stopped or crashed.
            /// </summary>
            private DateTime? CacheProcessStartTime = null;

            /// <summary>
            /// Getting the process start time.
            /// </summary>
            /// <returns>Process start time or min value if didn't start</returns>
            private DateTime GetProcessDateTime()
            {
                if (null == CacheProcessStartTime)
                {
                    CacheProcessStartTime = ProcessHandler.GetProcessStartTime();
                }
                return CacheProcessStartTime.Value;
            }

            void ResetProcessStartTime(object? sender, System.EventArgs e)
            {
                
                // Reset the cache when the process is stopped or crashed
                CacheProcessStartTime = null;
            }


            // start stop status and events
            
            /// <summary>
            /// Events for process start
            /// </summary>
            public event EventHandler? OnProcessStarted;
            
            public bool ProcessRunning { get; private set; }

            /// <summary>
            /// Events on process stop
            /// </summary>
            public event EventHandler? OnProcessStopped;

            /// <summary>
            /// Events on processs crash
            /// </summary>
            private event EventHandler? OnProcessCrashed; // TODO
            
            // Last logs
            private long LastErrorTimeStamp = long.MinValue;
            /// <summary>
            /// Events on logs error from process
            /// </summary>
            public event EventHandler<LogsEventArgs>? OnProcessError;

            private long LastLogTimeStamp = long.MinValue;
            /// <summary>
            /// Events on logs from process
            /// </summary>
            public event EventHandler<LogsEventArgs>? OnProcessLog;

            // AUTO START
            public bool AutoStart { get; set; } = false;
            private DateTime AutoStartCooldown = DateTime.MinValue;
            private const int AutoStartCooldownSeconds = 30; // Cooldown for auto start attempts

            /// <summary>
            /// Checking if autostart is on
            /// if autostart and process is not running, it will attempt to start the process
            /// have cooldown!
            /// </summary>
            private void AutoStartTrigger()
            {
                if (AutoStart)
                {
                    if (AutoStartCooldown > DateTime.Now)
                        return;
                    
                    if (!ProcessHandler.IsRunning)
                    {
                        Logger.LogWarn($"AutoStart Triggered in Process \"{ProcessHandler.Info.Tag}\"");
                        AutoStartCooldown = DateTime.Now.AddSeconds(AutoStartCooldownSeconds); // Cooldown for 5 seconds before next auto start attempt
                        try
                        {
                            ProcessHandler.StartProcess();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Failed to auto start process \"{ProcessHandler.Info.Tag}\": {ex.Message}");
                            Logger.LogWarn($"Attempting to restart process \"{ProcessHandler.Info.Tag}\" in {AutoStartCooldownSeconds} seconds.");
                        }
                    }
                }
            }

            public ProcessSlave(ProcessHandler handler, bool AutoStart = false, ProcessPriority priority = ProcessPriority.Normal)
            {
                this.AutoStart = AutoStart;
                ProcessHandler = handler;
                Proc_Priority = priority;
                ProcessRunning = handler.IsRunning;

                // adding process start time remover
                OnProcessCrashed += ResetProcessStartTime;
                Console.WriteLine($"TESt: {JsonConvert.SerializeObject(this, Formatting.Indented)}");
            }

            /// <summary>
            /// Checking and trigger Process events
            /// </summary>
            public void CheckProcess()
            {
                AutoStartTrigger();

                // Logs Event Checking
                if (ProcessRunning)
                {
                    Tuple<long, string>[] lastLogs;

                    // Checking if there is any event that can be invoke before wasting 
                    if (OnProcessLog != null)
                    {
                        lastLogs = ProcessHandler.GetLastLogs().ToArray();
                        if (lastLogs.Length > 0)
                        {
                            long lastLOgTime = lastLogs.Last().Item1;

                            if (lastLogs?.Length > 0 && lastLogs?.Last().Item1 > LastLogTimeStamp)
                            {
                                LastLogTimeStamp = lastLogs.Last().Item1;

                                OnProcessLog.Invoke(this, new LogsEventArgs(lastLogs.ToArray()));
                            }
                        }
                    }

                    // Checking if there is any event that can be invoke before wasting resources
                    if (OnProcessError != null)
                    {
                        // Error Event checking
                        lastLogs = ProcessHandler.GetLastErrors().ToArray();
                        if (lastLogs?.Length > 0 && lastLogs?.Last().Item1 > LastLogTimeStamp)
                        {
                            LastLogTimeStamp = lastLogs.Last().Item1;
                            OnProcessError?.Invoke(this, new LogsEventArgs(lastLogs.ToArray()));
                        }
                    }
                }

                // Checking if processes state has been changed
                if (ProcessRunning != ProcessHandler.IsRunning)
                {
                    // Process state has been changed
                    if (ProcessRunning)
                    {
                        // process has stopped
                        AutoStartCooldown = DateTime.Now.AddSeconds(AutoStartCooldownSeconds);

                        if (Proc_Priority == ProcessPriority.Core)
                        {
                            Logger.LogError($"Process \"{ProcessHandler.Info.Tag}\" has stopped but it is a core process, it should not stop! {(AutoStart ? $"Attempting to restart in {AutoStartCooldownSeconds} seconds." : "Auto Start off manual restart required!")}");
                        }
                        else
                        {
                            Logger.LogWarn($"Process \"{ProcessHandler.Info.Tag}\" (Priority: {Proc_Priority}) has stopped. {(AutoStart ? $"Attempting to restart in {AutoStartCooldownSeconds} seconds." : "")}");
                        }

                        ProcessRunning = false;
                        OnProcessStopped?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        // process has started
                        Logger.LogInfo($"Process \"{ProcessHandler.Info.Tag}\" (Priority: {Proc_Priority}) has started.");
                        ProcessRunning = true;
                        OnProcessStarted?.Invoke(this, EventArgs.Empty);
                    }
                }

                
            }
        }
    }
}
