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
        /// Trying to start process under the given tag
        /// </summary>
        /// <param name="data">process tag</param>
        /// <returns>200 if successed to stop or error code if failed</returns>
        private ServerMessageFormat ApiProcessesStart(string data)
        {

            ProcessesManager.ProcessSlave? slave = this.m_Manager.FindProcess(data);
            if (slave == null)
            {
                Logger.LogError($"[ApiProcessesStart] Process {data} not found.");
                return new ServerMessageFormat { Data = "Process not found", StatusCode = 404 };
            }

            try
            {
                slave.ProcessHandler.StartProcess();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ApiProcessesStart] Error while starting process {data}: {ex.Message}");
                return new ServerMessageFormat { Data = $"Error while starting process: {ex.Message}", StatusCode = 500 };
            }

            Logger.LogInfo($"[ApiProcessesStart] Process {data} started successfully.");
            return new ServerMessageFormat
            {
                Data = $"Process {data} started successfully.",
                StatusCode = 200
            };
        }

        /// <summary>
        /// Trying to stop process under the given tag
        /// </summary>
        /// <param name="data">Processes tag</param>
        /// <returns>200 if sucessed or error message</returns>
        private ServerMessageFormat ApiProcessesStop(string data)
        {
            ProcessesManager.ProcessSlave? slave = this.m_Manager.FindProcess(data);
            if (slave == null)
            {
                Logger.LogError($"[ApiProcessesStart] Process {data} not found.");
                return new ServerMessageFormat { Data = "Process not found", StatusCode = 404 };
            }

            try
            {
                slave.ProcessHandler.StopProcess();
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ApiProcessesStart] Error while stopping process {data}: {ex.Message}");
                return new ServerMessageFormat { Data = $"Error while stopping process: {ex.Message}", StatusCode = 500 };
            }

            Logger.LogInfo($"[ApiProcessesStart] Process {data} stopped successfully.");
            return new ServerMessageFormat
            {
                Data = $"Process {data} stopped successfully.",
                StatusCode = 200
            };
        }

    }
}
