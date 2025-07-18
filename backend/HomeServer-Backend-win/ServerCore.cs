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

        private ProcessesManager m_Manager;
        private Communication.SimpleTcpServer m_TcpServer;
        
        ServerCore()
        {
            Logger.LogInfo("Core Server Starting...");
            m_TcpServer = new(SERVER_PORT, SERVER_HOST);
            m_Manager = new();
        }

        void LoadData()
        {
            // TODO:
            // Load all data from the config file
            // Load all data from processes configs
        }

        void Start()
        {
            // TODO
        }

        void Shutdown()
        {
            // TODO
        }

        string ClientHandler(string message)
        {
            return "Message received: " + message; // echo
           // TODO
            // Handle all client requests
        }
    }
}
