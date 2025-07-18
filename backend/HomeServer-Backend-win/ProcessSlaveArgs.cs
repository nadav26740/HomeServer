using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{
    public class ProcessSlaveArgs
    {
        public ProcessesManager.ProcessPriority Priority { get; set; }
        public ProcessHandler.ProcessInfo ProcessInfo { get; set; }
        public bool AutoStart { get; set; } // If true, the process will be started automatically if it is not running

        // functions

        /// <summary>
        /// Creating Process slave that the process manager can use
        /// </summary>
        /// <returns></returns>
        public ProcessesManager.ProcessSlave CreateProcessSlave() => new ProcessesManager.ProcessSlave(new ProcessHandler(ProcessInfo), AutoStart, Priority);
        
        // fast deserilize to json
        public string DeserilizeToJson()
        {
            return "{ \n" + 
                        $"\t\"{nameof(Priority)}\": {(Int16)Priority}, \n" + 
                        $"\t\"{nameof(ProcessInfo)}\":{ProcessInfo.DeserilizeToJson()}, \n" +
                        $"\t\"{nameof(AutoStart)}\": {(AutoStart ? "true" : "false")}\n" + 
                    "}";
        }

        public override string ToString()
        {
            return $"{ProcessInfo.ToString()}\nPriority: {Priority}\nAutoStart: {AutoStart}";
        }
    }
}
