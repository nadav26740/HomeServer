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
            Low,
            Normal,
            High,
            Core, // Core processes that should always run
        }

        public class ProcessSlave
        {
            public class LogsEventArgs : EventArgs
            {
                public Tuple<DateTime, string>[] logs { get; }

                public LogsEventArgs(Tuple<DateTime, string>[] logsMessage)
                {
                    logs = logsMessage;
                }
            }

            public ProcessPriority Proc_Priority { get; set; }
            public ProcessHandler ProcessHandler { get; }

            public event EventHandler? OnProcessStarted;
            public bool ProcessRunning { get; private set; }
            public event EventHandler? OnProcessStopped;

            private event EventHandler? OnProcessCrashed; // TODO

            private DateTime LastErrorTimeStamp = DateTime.MinValue;
            public event EventHandler<LogsEventArgs>? OnProcessError;

            private DateTime LastLogTimeStamp = DateTime.MinValue;
            public event EventHandler<LogsEventArgs>? OnProcessLog;

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
            }

            /// <summary>
            /// Checking and trigger Process events
            /// </summary>
            public void CheckProcess()
            {
                AutoStartTrigger();

                // Process Start Stop event checking
                if (ProcessRunning != ProcessHandler.IsRunning)
                {
                    if (ProcessRunning)
                    {
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
                        Logger.LogInfo($"Process \"{ProcessHandler.Info.Tag}\" (Priority: {Proc_Priority}) has started.");
                        ProcessRunning = true;
                        OnProcessStarted?.Invoke(this, EventArgs.Empty);
                    }
                }

                // Logs Event Checking
                var lastLogs = ProcessHandler.GetLastLogs().ToArray();
                if (lastLogs.Length > 0)
                {
                    DateTime lastLOgTime = lastLogs.Last().Item1;

                    if (lastLogs?.Length > 0 && lastLogs?.Last().Item1 > LastLogTimeStamp)
                    {
                        LastLogTimeStamp = lastLogs.Last().Item1;

                        OnProcessLog?.Invoke(this, new LogsEventArgs(lastLogs.ToArray()));
                    }
                }

                // Error Event checking
                lastLogs = ProcessHandler.GetLastErrors().ToArray();
                if (lastLogs?.Length > 0 && lastLogs?.Last().Item1 > LastLogTimeStamp)
                {
                    LastLogTimeStamp = lastLogs.Last().Item1;
                    OnProcessError?.Invoke(this, new LogsEventArgs(lastLogs.ToArray()));
                }
            }
        }
    }
}
