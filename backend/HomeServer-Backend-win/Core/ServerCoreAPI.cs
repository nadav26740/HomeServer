using HomeServer_Backend.Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.Core
{
    public partial class ServerCore
    {
        // ============ Handle Types ===============
        private ServerMessageFormat HandleGETRequests(ClientMessageFormat message)
        {
            
            switch (message.Path.ToLower())
            {
                case "/api/processes":
                    // Return all processes status
                    return ApiProcesses(message.Data);

                case "/api/processes/status":
                    return ApiProcessesStatus(message.Data);

                case "/api/process/lastlogs":
                    return ApiProcessLastLogs(message.Data);

                case "/api/process/lasterrors":
                    return ApiProcessLastErrors(message.Data);

                default:
                    // Handle other GET requests
                    Logger.LogError($"Unknown GET request path: {message.Path} with data: {message.Data}");
                    return new() { Data = "Unknown GET request path", StatusCode = 404 };
            }
        }

        private ServerMessageFormat HandlePOSTRequests(ClientMessageFormat message)
        {
            switch (message.Path.ToLower())
            {
                case "api/process/input":
                    return ApiProcessInput(message.Data);

                default:
                    // Handle other GET requests
                    Logger.LogError($"Unknown POST request path: {message.Path} with data: {message.Data}");
                    return new() { Data = "Unknown POST request path", StatusCode = 404 };
            }
        }

        private ServerMessageFormat HandleDELETERequests(ClientMessageFormat message)
        {
            switch (message.Path.ToLower())
            {
                default:
                    // Handle other GET requests
                    Logger.LogError($"Unknown DELETE request path: {message.Path} with data: {message.Data}");
                    return new() { Data = "Unknown DELETE request path", StatusCode = 404 };
            }
        }

        private ServerMessageFormat HandleUPDATERequests(ClientMessageFormat message)
        {
            switch (message.Path.ToLower())
            {
                case "/api/process/start":
                    return ApiProcessesStart(message.Data);

                case "/api/process/stop":
                    return ApiProcessesStop(message.Data);

                default:
                    // Handle other GET requests
                    Logger.LogError($"Unknown UPDATE request path: {message.Path} with data: {message.Data}");
                    return new() { Data = "Unknown UPDATE request path", StatusCode = 404 };
            }
        }


        // Server API Answers
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


        private struct ProcessInputMessage
        {
            public string ProcessTag { get; set; }
            public string Input { get; set; }
        }

        private ServerMessageFormat ApiProcessInput(string data)
        {
            ProcessInputMessage? processInput = JsonConvert.DeserializeObject<ProcessInputMessage>(data);
            if (processInput == null || string.IsNullOrEmpty(processInput?.ProcessTag) || string.IsNullOrEmpty(processInput?.Input))
            {
                Logger.LogError($"[ApiProcessInput] Invalid input data: {data}");
                return new ServerMessageFormat { Data = "Invalid input data", StatusCode = 400 };
            }

            ProcessesManager.ProcessSlave? slave = this.m_Manager.FindProcess(processInput?.ProcessTag);
            if (slave == null)
            {
                Logger.LogError($"[ApiProcessInput] Process {processInput?.ProcessTag} not found.");
                return new ServerMessageFormat { Data = "Process not found", StatusCode = 404 };
            }

            if (!slave.ProcessHandler.IsRunning)
            {
                Logger.LogError($"[ApiProcessInput] Process {processInput?.ProcessTag} isn't running");
                return new ServerMessageFormat { Data = "Process isn't running", StatusCode = 400 };
            }

            try
            {
                slave.ProcessHandler.WriteToProcess(processInput?.Input ?? " ");
                Logger.LogInfo($"[ApiProcessInput] Input sent to process {processInput?.ProcessTag} successfully.");
                return new ServerMessageFormat { Data = "Input sent successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Logger.LogError($"[ApiProcessInput] Error while sending input to process {processInput?.ProcessTag}: {ex.Message}");
                return new ServerMessageFormat { Data = $"Error while sending input: {ex.Message}", StatusCode = 500 };
            }
        }
    }
}
