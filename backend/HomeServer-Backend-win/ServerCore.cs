using HomeServer_Backend.Communication;
using HomeServer_Backend.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HomeServer_Backend
{
    public class ServerCore
    {
        public const int SERVER_PORT = 3123;
        public const string SERVER_HOST = "127.0.0.1";

        private Task? server_task;
        private ProcessesManager m_Manager;
        private SimpleTcpServer m_TcpServer;
        
        ServerCore()
        {
            Logger.LogInfo("Core Server Starting...");
            m_TcpServer = new(SERVER_PORT, SERVER_HOST);
            m_Manager = new();
            m_TcpServer.ClientMessageResponder = ClientHandler;
        }

        void LoadData()
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

        void Start()
        {
            server_task = m_TcpServer.StartAsync();
            m_Manager.ForceStart();
            // TODO
        }

        void Shutdown()
        {
            m_TcpServer.Stop();
            m_Manager.Shutdown();
            // TODO
        }

        ServerMessageFormat ClientHandler(ClientMessageFormat message)
        {
            return new() { Data = $"Message received: {message.ToString()}", StatusCode=101 }; // echo
            
            // TODO:
            switch (message.Type)
            {
                case ClientMessagesType.GET:
                    // Handle GET request
                    break;

                case ClientMessagesType.POST:
                    // Handle POST request
                    break;

                case ClientMessagesType.UPDATE:
                    // Handle UPDATE request
                    break;

                case ClientMessagesType.DELETE:
                    // Handle DELETE request
                    break;
            }
           // TODO
           // Handle all client requests
        }
    }
}
