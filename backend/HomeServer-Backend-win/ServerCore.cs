using HomeServer_Backend.Communication;
using HomeServer_Backend.Loader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HomeServer_Backend
{
    public class ServerCore
    {
        public const string SERVER_HOST = "127.0.0.1";

        private Task? server_task;
        private ProcessesManager m_Manager;
        private SimpleTcpServer m_TcpServer;
        
        public ServerCore(string ConfigPath = "")
        {
            try
            {
                if (ConfigPath != "")
                {
                    Config.LoadConfig(ConfigPath);
                }
                else
                {
                    Config.LoadConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                Console.WriteLine("Creating default Config");
                Config.WriteConfigFile();
            }
            

            Logger.LogInfo("Core Server Starting...");
            m_TcpServer = new(Config.data.ServerPort, SERVER_HOST);
            m_Manager = new();
            m_TcpServer.ClientMessageResponder = ClientHandler;
        }

        ~ServerCore()
        {
            Shutdown();
        }

        public void LoadData()
        {
            // TODO:
            // Load Config..

            ProcessSlaveArgs[]? procSlaves = ProcessConfigSave.ReadSlaveProcessData("processes.json");
            if (procSlaves == null || procSlaves.Length == 0)
            {
                Logger.LogWarn("No process slaves found in the config file. Please check your configuration.");
            }
            else
            {
                foreach (var slave in procSlaves)
                {
                    m_Manager.AddProcess(slave.CreateProcessSlave());
                }

                Logger.LogInfo($"Loaded {m_Manager.GetProcesses().Length} process slaves from config file.");
            }

            // Load all data from the config file
        }

        public void Start()
        {
            Logger.LogInfo("Server Core Start has been called");
            server_task = m_TcpServer.StartAsync();
            m_Manager.ForceStart();
            
        }

        public void Shutdown()
        {
            Logger.LogWarn("Server Core shutdown has been called");
            m_TcpServer.Stop();
            m_Manager.Shutdown();
        }

        ServerMessageFormat ClientHandler(ClientMessageFormat message)
        {            
            switch (message.Type)
            {
                case ClientMessagesType.GET:
                    // Handle GET request
                    return HandleGETRequests(message);

                case ClientMessagesType.POST:
                    // Handle POST request
                    return HandlePOSTRequests(message);

                case ClientMessagesType.UPDATE:
                    // Handle UPDATE request
                    return HandleUPDATERequests(message);

                case ClientMessagesType.DELETE:
                    // Handle DELETE request
                    return HandleDELETERequests(message);

                default:
                    Logger.LogError($"Unknown message type: {message.Type} for path: {message.Path} with data: {message.Data}");
                    return new() { Data = "Unknown message type", StatusCode = 400 };
            }
        }

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
            
                Logger.LogInfo($"Returning {processes.Length} processes.");
                return new() { Data = processes, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error while getting processes: {ex.Message}");
                return new() { Data = "Error while getting processes", StatusCode = 500 };
            }
        }

        /// <summary>
        /// Return all the server processes status
        /// </summary>
        /// <param name="data">No use for now</param>
        /// <returns>server processes status in message format</returns>
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
                Logger.LogInfo($"Returning {status.Length} processes status.");

                return new() { Data = status, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error while getting processes status: {ex.Message}");
                return new() { Data = "Error while getting processes status", StatusCode = 500 };
            }
        }
    }
}
