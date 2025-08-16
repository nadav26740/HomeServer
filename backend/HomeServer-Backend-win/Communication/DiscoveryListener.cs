using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.Communication
{
    /// <summary>
    ///  for listening to discovery messages from clients
    ///  and by that allowing users to discover the server in local network.
    /// </summary>
    internal class DiscoveryListener : IDisposable
    {
        /// <summary>
        /// discovery server request handler delegate.
        /// </summary>
        /// <param name="buffer">message buffer</param>
        /// <param name="serverDiscoveryNet">Server socket network</param>
        /// <param name="remoteEP">Client information</param>
        public delegate void DiscoveryRequestHandler(string buffer, UdpClient serverDiscoveryNet, IPEndPoint remoteEP);

        /// <summary>
        /// Discovery request handler event.
        /// </summary>
        public DiscoveryRequestHandler? RequestHandler;
        
        /// <summary>
        /// Discovery listener stopped event.
        /// </summary>
        public EventHandler? OnListenerCrashed;

        private readonly UdpClient udpListener;
        
        public bool IsListening { get; private set; } = false;
        CancellationTokenSource StopListenerToken;


        private int port;

        
        /// <param name="port">Listing port</param>
        public DiscoveryListener(int port)
        {
            // Initialize the listener
            // This could be a UDP listener or any other type of discovery mechanism
            this.port = port;
            udpListener = new(port);
            StopListenerToken = new();
        }

        /// <summary>
        /// Async method to start listening for discovery messages.
        /// </summary>
        public async Task StartAsyncListening()
        {
            // Start listening for discovery messages
            // This method should handle incoming discovery requests
            Logger.LogInfo($"Discovery Listener started on port {port}");
            if (IsListening)
            {
                Logger.LogError("Discovery Listener failed to start");
                Logger.LogWarn("Discovery Listener is already running.");
                return;
            }

            if (udpListener == null || udpListener.Client == null || !udpListener.Client.IsBound)
            {
                Logger.LogError("Discovery Listener failed to start udp listener not available");
                return;
            }

            IsListening = true;

            try
            {
                while (IsListening)
                {
                    // Receive data from any client
                    UdpReceiveResult result = await udpListener.ReceiveAsync(StopListenerToken.Token);
                
                    byte[] data = result.Buffer;
                    string message = Encoding.UTF8.GetString(data);
                    Logger.LogInfo($"[Discovery Listener] Received from {result.RemoteEndPoint}: \"{message}\"");

                    // If it's a discovery buffer, send back response
                    _ = Task.Run(() => RequestHandler?.Invoke(message, udpListener, result.RemoteEndPoint));
                }
            }
            catch (ObjectDisposedException ex)
            {
                // Listener was stopped, exit gracefully
                Logger.LogWarn("Discovery listener Disposed: " + ex.Message);
            }
            catch (OperationCanceledException ex)
            {
                // Listener was stopped, exit gracefully
                Logger.LogWarn("Discovery listener stopped: " + ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Discovery Listener Stopped: " + ex.Message);
                OnListenerCrashed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stopping the async listening for discovery messages.
        /// </summary>
        public void StopAsyncListening()
        {
            // Stop listening for discovery messages
            // Clean up resources if necessary
            IsListening = false;
            StopListenerToken.Cancel();
            udpListener?.Close();
        }

        /// <summary>
        /// Disposing all resources
        /// </summary>
        public void Dispose()
        {
            StopAsyncListening();
            udpListener?.Dispose();
        }
        
        // Additional methods for handling specific discovery protocols can be added here
    }
}
