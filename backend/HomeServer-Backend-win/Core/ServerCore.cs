using HomeServer_Backend.Communication;
using HomeServer_Backend.Loader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HomeServer_Backend.Core
{
    public partial class ServerCore : IAsyncDisposable
    {
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

            Logger.ForceSetLoggingPath(Config.data.LogPath);

            Logger.LogInfo("Core Server Starting...");
            m_TcpServer = new(Config.data.ServerPort);
            
            m_Manager = new();
            m_TcpServer.ClientMessageResponder += ClientHandler;
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Run( () => Shutdown() );
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

        
    }
}
