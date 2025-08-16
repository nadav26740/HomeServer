using HomeServer_Backend.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.Core
{
    public partial class ServerCore
    {
        private struct ProcessInputMessage
        {
            public string ProcessTag { get; set; }
            public string Input { get; set; }
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
