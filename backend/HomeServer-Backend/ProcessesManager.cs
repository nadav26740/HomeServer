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



    public partial class ProcessesManager
    { 
        /// <summary>
        /// Wrapper that will allow the process manager to control the processes in easier way
        /// </summary>
        

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
 