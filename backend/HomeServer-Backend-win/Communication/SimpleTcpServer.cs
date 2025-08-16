using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;


namespace HomeServer_Backend.Communication
{
    public class SimpleTcpServer
    {
        public delegate ServerMessageFormat ClientMessageHandler(ClientMessageFormat message);

        /// TODO: add time checker and logger
        public ClientMessageHandler? ClientMessageResponder;

        private readonly TcpListener _listener;
        private readonly int _port;
        private readonly string _ipAddress;
        private bool Running = false;

        /// <summary>
        /// allowd IPs to connect to the server.
        /// Leave empty for all ips to connect.
        /// </summary>
        public string[] AllowedIPS = { };

        public SimpleTcpServer(int port, string ipAddress = "127.0.0.1")
        {
            _ipAddress = ipAddress;
            _port = port;
            _listener = new TcpListener(IPAddress.Any, _port);
        }

        /// <summary>
        /// Asyncronic Listener
        /// </summary>
        /// <returns>Nothing</returns>
        public async Task StartAsync()
        {
            Running = true;
            _listener.Start();
            Logger.LogInfo($"Server started. Listening on {_listener.Server.LocalEndPoint}");

            try
            {
                while (Running)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    
                    // Todo add whitelist
                    // if (client.Client.Connected)

                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Listener error occured: {ex.Message}");
            }
            finally
            {
                Logger.LogWarn("Server Listener Stopped");
                _listener.Stop();
            }
        }

        public void Start()
        {
            _listener.Start();
            Logger.LogInfo($"Server started. Listening on {_ipAddress}:{_port}");

            try
            {
                while (Running)
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Listener error occurred: {ex.Message}");
            }
            finally
            {
                _listener.Stop();
            }
        }

        public void Stop() { Running = false; _listener.Stop(); }

        private async Task HandleClientAsync(TcpClient client)
        {
            string ClientRemoteEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
            Logger.LogInfo("New client connected: " + ClientRemoteEndPoint);
            

            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        ClientMessageFormat messageFormated = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientMessageFormat>(message) ?? new ClientMessageFormat { Data = message, Type = "unknown" };

                        Logger.LogInfo($"{{ \"IP\": \"{ClientRemoteEndPoint}\" ,\"message\":{messageFormated} }}");

                        // Serilizing the message into json
                        message = JsonConvert.SerializeObject( ClientMessageResponder?.Invoke(messageFormated), Formatting.None,
                            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.None}) ?? new ServerMessageFormat().SerilizeToJson();

                        byte[] response = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(response, 0, response.Length);
                        Logger.LogInfo($"(Client {ClientRemoteEndPoint}) Response sent: {message}");
                        
                        // TODO: ADD ON PRODUCTION
                        // client.Close(); // COMMENTED FOR DEBUG ONLY
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"(Client {ClientRemoteEndPoint}) handling error: {ex.Message}");
            }
            finally
            {
                client.Close();
                Logger.LogInfo($"Client {ClientRemoteEndPoint} disconnected.");
            }
        }

    }
}
