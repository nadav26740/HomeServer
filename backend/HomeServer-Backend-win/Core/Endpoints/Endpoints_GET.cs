using HomeServer_Backend.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.Core
{
    public partial class ServerCore
    {
        /// <summary>
        /// Return all the server processes slaves
        /// </summary>
        /// <param name="data">No use for now</param>
        /// <returns>server processes slaves in message format</returns>
        private ServerMessageFormat ApiProcesses(string data)
        {
            try
            {
                var processes = m_Manager.GetProcesses();

                Logger.LogInfo($"[ApiProcesses] Returning {processes.Length} processes.");
                return new() { Data = processes, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ApiProcesses] Error while getting processes: {ex.Message}");
                return new() { Data = "Error while getting processes", StatusCode = 500 };
            }
        }

        /// <summary>
        /// Return all the server processes status
        /// </summary>
        /// <param name="data">No use for now</param>
        /// <returns>server processes status in message format</returns>
        /// TODO: PROFILING AND OPTIMIZATION!
        private ServerMessageFormat ApiProcessesStatus(string data)
        {
            try
            {
                var processes = m_Manager.GetProcesses();
                var status = new ProcessesStatus[processes.Length];

                for (int i = 0; i < processes.Length; i++)
                {
                    status[i] = new ProcessesStatus(processes[i]);
                }
                Logger.LogInfo($"[ApiProcessesStatus] Returning {status.Length} processes status.");

                return new() { Data = status, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ApiProcessesStatus] Error while getting processes status: {ex.Message}");
                return new() { Data = "Error while getting processes status", StatusCode = 500 };
            }
        }

        /// <summary>
        /// Getting Processes tag and return his last logs
        /// </summary>
        /// <param name="data">Processes tag</param>
        /// <returns>ServerMessageFormat Contains logs or failure answer</returns>
        private ServerMessageFormat ApiProcessLastLogs(string data)
        {

            try
            {
                var process = m_Manager.FindProcess(data);
                if (process == null)
                {
                    Logger.LogError($"[ApiProcessLastLogs] Process {data} not found.");
                    return new() { Data = "Process not found", StatusCode = 404 };
                }


                Logger.LogInfo($"Returning status for process {data}.");

                return new() { Data = process.ProcessHandler.GetLastLogs(), StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error while getting process status: {ex.Message}");
                return new() { Data = "Error while getting process status", StatusCode = 500 };
            }
        }

        /// <summary>
        /// Getting Processes tag and return his last errors
        /// </summary>
        /// <param name="data">Processes tag</param>
        /// <returns>ServerMessageFormat Contains errors or failure answer</returns>
        // ! TODO: PROFILE AND OPTIMIZE
        private ServerMessageFormat ApiProcessLastErrors(string data)
        {
            try
            {
                var process = m_Manager.FindProcess(data);
                if (process == null)
                {
                    Logger.LogError($"[ApiProcessLastLogs] Process {data} not found.");
                    return new() { Data = "Process not found", StatusCode = 404 };
                }


                Logger.LogInfo($"Returning status for process {data}.");

                return new() { Data = process.ProcessHandler.GetLastErrors(), StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error while getting process status: {ex.Message}");
                return new() { Data = "Error while getting process status", StatusCode = 500 };
            }
        }
    }
}
