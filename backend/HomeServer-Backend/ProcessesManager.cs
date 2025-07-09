using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public enum ProcessPriority
    {
        Core,
        High,
        Normal,
        Low
    }



    public class ProcessesManager
    {
        public class ProcessSlave
        {
            public ProcessPriority Proc_Priority { get; set; }
            public ProcessHandler Handler { get; }

            

            public event EventHandler OnProcessStarted;
            public event EventHandler OnProcessStopped;
            public event EventHandler OnProcessCrashed;
            
            private DateTime LastErrorTimeStamp = DateTime.MinValue;
            public event EventHandler OnProcessError;

            private DateTime LastLogTimeStamp = DateTime.MinValue;
            public event EventHandler OnProcessLog;

            public ProcessSlave(ProcessHandler handler, ProcessPriority priority = ProcessPriority.Normal)
            {
                Handler = handler;
                Proc_Priority = priority;
            }

            public void CheckProcess()
            {
                    
            }
        }

        const int Supervised_per_Second = 1; // How many supervise Checks per second
        const float Supervised_per_Millisecond = 1000f / Supervised_per_Second; // How many milliseconds between each supervise check

        Dictionary<string, ProcessSlave> ProcessMap;
        Thread m_Supervisor_Thread;
        bool Running = true;

        ProcessesManager()
        {
            ProcessMap = new Dictionary<string, ProcessSlave>();
            m_Supervisor_Thread = new Thread(new ThreadStart(this.Supervising));
        }

        ~ProcessesManager()
        {
            if (m_Supervisor_Thread != null && m_Supervisor_Thread.IsAlive)
            {
                Running = false;
            }
        }

        void Supervising()
        {

        }
    }
}
 