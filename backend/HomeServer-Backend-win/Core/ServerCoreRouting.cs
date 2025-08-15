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

        /// <summary>
        /// Routing GET requests to the appropriate handlers.
        /// </summary>
        /// <param name="message">Client message</param>
        /// <returns>Server response</returns>
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

        /// <summary>
        /// Routing POST requests to the appropriate handlers.
        /// </summary>
        /// <param name="message">Client message</param>
        /// <returns>Server response</returns>
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

        /// <summary>
        /// Routing DELETE requests to the appropriate handlers.
        /// </summary>
        /// <param name="message">Client message</param>
        /// <returns>Server response</returns>
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

        /// <summary>
        /// Routing UPDATE requests to the appropriate handlers.
        /// </summary>
        /// <param name="message">Client message</param>
        /// <returns>Server response</returns>
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
    }
}
