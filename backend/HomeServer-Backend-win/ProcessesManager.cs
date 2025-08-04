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

        protected Dictionary<string, ProcessSlave> m_ProcessMap;
        Thread m_Supervisor_Thread;
        bool Running = false;

        /// <summary>
        /// Indicates if the manager is running and supervising processes.
        /// </summary>
        public bool ManagerStatus {  get { return Running; } }

        Mutex ManagerCommandMutex;

        public ProcessesManager()
        {
            ManagerCommandMutex = new Mutex();
            m_ProcessMap = new Dictionary<string, ProcessSlave>();
            m_Supervisor_Thread = new Thread(new ThreadStart(this.Supervising));
            Logger.LogInfo($"Process Manager Started on memory ${this}");
        }

        ~ProcessesManager()
        {
            this.Shutdown();

            //if (m_Supervisor_Thread != null && m_Supervisor_Thread.IsAlive)
            //{
            //}
        }

        public ProcessSlave[] GetProcesses()
        {
            return m_ProcessMap.Values.ToArray();
        }

        public ProcessSlaveArgs[]? ProcessesToSlavesArgs()
        {
            if (m_ProcessMap.Count == 0)
            {
                Logger.LogWarn("No processes to convert to ProcessSlaveArgs.");
                return null;
            }

            List<ProcessSlaveArgs> processesSlaves = new(m_ProcessMap.Count);

            foreach (var process in m_ProcessMap.Values)
            {
                if (process == null)
                {
                    Logger.LogError("Process is null, cannot convert to ProcessSlaveArgs.");
                    continue;
                }

                processesSlaves.Add(new ProcessSlaveArgs(process));
            }

            return processesSlaves.ToArray();
        }

        /// <summary>
        /// Will Shutdown the manager and all the processes owned and restart the manager
        /// Will also start the core processes no matter what
        /// </summary>
        /// <param name="StartAllProcess">Will start all processes above this level</param>
        public bool ForceStart(ProcessPriority minimumPriorityToStart = ProcessPriority.Core)
        {
            if (ManagerCommandMutex.WaitOne(1000) == false)
            {
                Logger.LogError("Failed to acquire mutex for ProcessesManager ForceStart command.");
                return false;
            }

            if (Running)
                this.Shutdown(true);

            Running = true;
            m_Supervisor_Thread = new Thread(new ThreadStart(this.Supervising));
            m_Supervisor_Thread.Start();
            ManagerCommandMutex.ReleaseMutex();
            
            return true;
        }

        /// <summary>
        /// Stopping all the owned processes and shutdown the manager supervisor thread.
        /// </summary>
        public bool Shutdown(bool force = false)
        {
            if (!force && ManagerCommandMutex.WaitOne(1000) == false)
            {
                Logger.LogError("Failed to acquire mutex for ProcessesManager ForceStart command.");
                return false;
            }

            Logger.LogWarn("Processes Manager Shutdown requested, stopping all processes and supervisor thread...");
            Running = false;

            // Stopping all processes
            foreach (var process in m_ProcessMap.Values)
            {
                try
                {
                    process.AutoStart = false;
                    process.ProcessHandler.StopProcess();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to stop process \"{process.ProcessHandler.Info.Tag}\": {ex.Message}");
                }
            }

            // Wait for supervisor thread to finish
            if (m_Supervisor_Thread != null && m_Supervisor_Thread.IsAlive)
            {
                m_Supervisor_Thread.Join();
            }

            Logger.LogWarn("Processes Manager Shutdown completed.");

            if (!force)
            {
                ManagerCommandMutex.ReleaseMutex(); 
            }
            return true;
        }

        /// <summary>
        /// Adding Process Slave to the manager.
        /// </summary>
        /// <param name="slave">Slave owning a process to add to manager</param>
        /// <returns>False if failed to add, True if added successfuly</returns>
        public bool AddProcess(ProcessSlave slave)
        {
            if (slave == null) { return false; }

            // Locking mutex to ensure thread safety when adding a process
            if (ManagerCommandMutex.WaitOne(1000) == false)
            {
                Logger.LogError("Failed to acquire mutex for ProcessesManager AddProcess command.");
                return false;
            }

            ProcessHandler handler = slave.ProcessHandler;

            try
            {
                // checking if the process with this Tag already exists
                if (m_ProcessMap.ContainsKey(handler.Info.Tag))
                {
                    Logger.LogError($"Failed To add process \"{handler.Info.Tag}\" Already exists process with that Tag");
                    return false;
                }

                // Adding Process
                m_ProcessMap.Add(handler.Info.Tag, slave);
                Logger.LogInfo($"process \"{handler.Info.Tag}\" added succesfuly");


            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to add process \"{handler.Info.Tag}\": {ex.Message}");
                ManagerCommandMutex.ReleaseMutex();
                return false;
            }

            // Releasing Mutex
            ManagerCommandMutex.ReleaseMutex();
            return true;
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
                if (m_ProcessMap.ContainsKey(proc.Info.Tag))
                {
                    Logger.LogError($"Failed To add process \"{proc.Info.Tag}\" Already exists process with that Tag");
                    return false;
                }

                // Adding Process
                ProcessSlave slave = new ProcessSlave(proc, true);
                m_ProcessMap.Add(proc.Info.Tag, slave);
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
        /// <param name="tag">name Tag of the process</param>
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

                if (!m_ProcessMap.TryGetValue(tag, out var procSlave))
                {
                    Logger.LogError($"Failed To Remove Process \"{tag}\" (Process not found)");
                    return false;
                }

                // Making sure autostart is off before removing the process
                procSlave.AutoStart = false;

                // If ShutdownFirst is true, stop the process before removing it
                if (ShutdownFirst)
                    procSlave.ProcessHandler.StopProcess();

                m_ProcessMap.Remove(tag);
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

        /// <summary>
        /// Running "ticks" that will check all processes and their status.
        /// </summary>
        void Supervising()
        {
            Logger.LogInfo("Process manager Supervisor Started");
            while (Running)
            { 
               foreach (var process in m_ProcessMap.Values)
               {
                   process.CheckProcess();
               }
                Thread.Sleep((int)Supervised_per_Millisecond);
            }
        }

        /// <summary>
        /// Getting ProcessSlave by its Tag.
        /// </summary>
        /// <param name="Tag">Tag name of the process</param>
        /// <returns>Process Slave containing the process with that Tag or null if failed to find</returns>
        public ProcessSlave? FindProcess(string Tag)
        {
            if (m_ProcessMap.TryGetValue(Tag, out var process))
                return process;
            return null;
        }
    }
}
 