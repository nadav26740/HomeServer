using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend
{

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

        Mutex ManagerCommandMutex;

        public ProcessesManager()
        {
            ManagerCommandMutex = new Mutex();
            ProcessMap = new Dictionary<string, ProcessSlave>();
            m_Supervisor_Thread = new Thread(new ThreadStart(this.Supervising));
            m_Supervisor_Thread.Start();
        }

        ~ProcessesManager()
        {
            this.Shutdown();

            //if (m_Supervisor_Thread != null && m_Supervisor_Thread.IsAlive)
            //{
            //}
        }

        /// <summary>
        /// Will Shutdown the manager and all the processes owned and restart the manager
        /// Will also start the core processes no matter what
        /// </summary>
        /// <param name="StartAllProcess">Will start all processes above this level</param>
        public void ForceStart(ProcessPriority minimumPriorityToStart = ProcessPriority.Core)
        {
            if (ManagerCommandMutex.WaitOne(1000) == false)
            {
                Logger.LogError("Failed to acquire mutex for ProcessesManager ForceStart command.");
                return;
            }

            this.Shutdown(true);

            Running = true;
            m_Supervisor_Thread = new Thread(new ThreadStart(this.Supervising));
            m_Supervisor_Thread.Start();
            ManagerCommandMutex.ReleaseMutex();
        }

        /// <summary>
        /// Stopping all the owned processes and shutdown the manager supervisor thread.
        /// </summary>
        public void Shutdown(bool force = false)
        {
            if (!force && ManagerCommandMutex.WaitOne(1000) == false)
            {
                Logger.LogError("Failed to acquire mutex for ProcessesManager ForceStart command.");
                return;
            }

            Logger.LogInfo("Processes Manager Shutdown requested, stopping all processes and supervisor thread...");
            Running = false;

            // Stopping all processes
            foreach (var process in ProcessMap.Values)
            {
                try
                {
                    process.AutoStart = false;
                    process.Handler.StopProcess();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to stop process \"{process.Handler.Info.Tag}\": {ex.Message}");
                }
            }

            // Wait for supervisor thread to finish
            if (m_Supervisor_Thread != null && m_Supervisor_Thread.IsAlive)
            {
                m_Supervisor_Thread.Join();
            }

            Logger.LogInfo("Processes Manager Shutdown completed.");

            if (!force)
            {
                ManagerCommandMutex.ReleaseMutex(); 
            }
        }

        /// <summary>
        /// Return false if unable to add process
        /// </summary>
        /// <param name="proc">Process to add</param>
        /// <returns>True process added, false if failed to add</returns>
        public bool AddProcess(ProcessHandler proc)
        {
            if (ManagerCommandMutex.WaitOne(1000) == false)
            {
                Logger.LogError("Failed to acquire mutex for ProcessesManager AddProcess command.");
                return false;
            }

            try
            {
                if (ProcessMap.ContainsKey(proc.Info.Tag))
                {
                    Logger.LogError($"Failed To add process \"{proc.Info.Tag}\" Already exists process with that tag");
                    return false;
                }

                // Adding Process
                ProcessSlave slave = new ProcessSlave(proc, true);
                ProcessMap.Add(proc.Info.Tag, slave);
                Logger.LogInfo($"process \"{proc.Info.Tag}\" added succesfuly");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to add process \"{proc.Info.Tag}\": {ex.Message}");
                ManagerCommandMutex.ReleaseMutex();
                return false;
            }

            ManagerCommandMutex.ReleaseMutex();
            return true;
        }

        /// <summary>
        /// Removing process from the manager.
        /// </summary>
        /// <param name="tag">name tag of the process</param>
        /// <param name="ShutdownFirst">Should the process be closed before removing it</param>
        /// <returns>return false if process not found or unable to remove, True if removed successfuly</returns>
        public bool RemoveProcess(string tag, bool ShutdownFirst = true)
        {
            
            if (ManagerCommandMutex.WaitOne(1000) == false)
            {
                Logger.LogError("Failed to acquire mutex for ProcessesManager RemoveProcess command.");
                return false;
            }

            try
            {

                if (!ProcessMap.ContainsKey(tag))
                {
                    Logger.LogError($"Failed To Remove Process \"{tag}\" (Process not found)");
                    return false;
                }

                if (ShutdownFirst)
                    ProcessMap[tag].Handler.StopProcess();

                ProcessMap.Remove(tag);
                Logger.LogInfo($"Process \"{tag}\" removed successfuly");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to remove process \"{tag}\": {ex.Message}");
                ManagerCommandMutex.ReleaseMutex();
                return false;
            }
            
            ManagerCommandMutex.ReleaseMutex();    
            return true;
        }

        // NEED TO BE TESTED!
        void Supervising()
        {
            Logger.LogInfo("Process manager Supervisor Started");
            while (Running)
            { 
               foreach (var process in ProcessMap.Values)
               {
                   process.CheckProcess();
               }
                Thread.Sleep((int)Supervised_per_Millisecond);
            }
        }
    }
}
 