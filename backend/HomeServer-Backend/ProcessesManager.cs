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
            public ProcessHandler Handler;

            public ProcessSlave(ProcessHandler handler, ProcessPriority priority = ProcessPriority.Normal)
            {
                Handler = handler;
                Proc_Priority = priority;
            }
        }

    }
}
 