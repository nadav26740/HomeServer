using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public partial class ProcessesManager
    {
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
            public ProcessHandler Handler { get; }


            public event EventHandler OnProcessStarted;
            public bool ProcessRunning { get; private set; }
            public event EventHandler OnProcessStopped;

            private event EventHandler OnProcessCrashed; // TODO

            private DateTime LastErrorTimeStamp = DateTime.MinValue;
            public event EventHandler<LogsEventArgs> OnProcessError;

            private DateTime LastLogTimeStamp = DateTime.MinValue;
            public event EventHandler<LogsEventArgs> OnProcessLog;

            public ProcessSlave(ProcessHandler handler, ProcessPriority priority = ProcessPriority.Normal)
            {
                Handler = handler;
                Proc_Priority = priority;
                ProcessRunning = handler.IsRunning;
            }

            // Checking and running Process events
            public void CheckProcess()
            {
                // Process Start Stop event checking
                if (ProcessRunning != Handler.IsRunning)
                {
                    if (ProcessRunning)
                    {
                        ProcessRunning = false;
                        OnProcessStopped?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        ProcessRunning = true;
                        OnProcessStarted?.Invoke(this, EventArgs.Empty);
                    }
                }

                // Logs Event Checking
                var lastLogs = Handler.GetLastLogs();

                if (lastLogs?.Last().Item1 > LastLogTimeStamp)
                {
                    LastLogTimeStamp = lastLogs.Last().Item1;

                    OnProcessLog.Invoke(this, new LogsEventArgs(lastLogs.ToArray()));
                }

                // Error Event checking
                lastLogs = Handler.GetLastErrors();
                if (lastLogs?.Last().Item1 > LastLogTimeStamp)
                {
                    LastLogTimeStamp = lastLogs.Last().Item1;
                    OnProcessError.Invoke(this, new LogsEventArgs(lastLogs.ToArray()));
                }
            }
        }
    }
}
