using HomeServer_Backend.Communication;
using HomeServer_Backend.Loader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace HomeServer_Backend.Core
{
    public partial class ServerCore
    {
        // Metadata
        private string name = "Home Server";
        private string version = "1.0.0";

        private ProcessesManager m_Manager;

        // ======== Networking ==============
        public const string SERVER_HOST = "127.0.0.1";
        public const string DISCOVERY_REQUEST_MESSAGE = "DISCOVER_LOCAL_HOMESERVER";

        private Task? server_task;
        private Task? Discovery_task;

        private SimpleTcpServer m_TcpServer;
        private DiscoveryListener m_DiscoveryListener;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConfigPath"></param>
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
            m_DiscoveryListener = new(Config.data.DiscoveryPort);
            m_DiscoveryListener.OnDiscoveryRequest = HandleDiscoveryRequest; // Register discovery request handler
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
            Discovery_task = m_DiscoveryListener.StartAsyncListening(); 

            m_Manager.ForceStart();

        }

        public void Shutdown()
        {
            Logger.LogWarn("Server Core shutdown has been called");
            m_TcpServer.Stop();
            m_Manager.Shutdown();
            m_DiscoveryListener.StopAsyncListening();
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

        private class DiscoveryResponse
        {
            public string Name { get; set; } = "Home Server";
            public bool Secured { get; set; } = false; // Example secured status
            public string IP { get; set; } = "0.0.0.0";
            public int Port { get; set; } = 0; // Example server port
            public string Version { get; set; } = "1.0.0"; // Example version
            public string Message { get; set; } = "";

        }

        void HandleDiscoveryRequest(string buffer, UdpClient serverDiscoveryNet, IPEndPoint remoteEP)
        {

            if (buffer == DISCOVERY_REQUEST_MESSAGE)
            {
                DiscoveryResponse response = new DiscoveryResponse
                {
                    Message = "Discovery response from Home Server",
                    Secured = false,
                    IP = this.m_TcpServer.Address,
                    Port = this.m_TcpServer.Port,
                    Name = this.name,
                    Version = this.version
                };

                // Serialize the response to JSON
                string responseJson = JsonConvert.SerializeObject(response, Formatting.Indented);
                byte[] responseData = Encoding.UTF8.GetBytes(responseJson);
                serverDiscoveryNet.Send(responseData, responseData.Length, remoteEP);
                Logger.LogInfo($"Sending discovery response: ${responseJson}");

            }
        }
    }
}
