using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.Communication
{
    public class ProcessesStatus
    {
        public bool Running = false;
        public bool IsAutoStart = false;
        public ProcessesManager.ProcessPriority Priority = ProcessesManager.ProcessPriority.Normal;
        public string Tag = "ProcessName";
        public long ProcessMemoryUsage = 0;


        public ProcessesStatus(ProcessesManager.ProcessSlave slave) 
        {
            Running = slave.ProcessRunning;
            IsAutoStart = slave.AutoStart;
            Tag = slave.ProcessHandler.Info.Tag;
            // ProcessMemoryUsage = slave.ProcessHandler.GetTotalMemoryUsage(); // need to optimize
            ProcessMemoryUsage = -1;
            Priority = slave.Proc_Priority;
        }

    }
}
