using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.Communication
{

    public struct ClientMessagesType
    {
        // I KNOW THERE IS BETTER WAY BUT IT WAS FUNNY
        public const string GET = "GET";
        public const string POST = "POST";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
    }

    public class ClientMessageFormat
    {
        public ClientMessageFormat() { }
        public ClientMessageFormat(string path, string type, string data)
        {
            Path = path;
            Type = type;
            Data = data;
        }

        public string Path { get; set; } = ""; // e.g. /api/v1/devices/1234
        public string Type { get; set; } = "GET"; // GET, UPDATE, POST, DELETE
        public string Data { get; set; } = ""; // JSON data or other content
        public int ClientID { get; set; } = 0; // client identifier

        public override string ToString()
        {
            // Serilizing it to json
            return $"{{\"ClientID\":{ClientID},\n\t\"Path\":\"{Path}\",\n\t\"Type\":\"{Type}\",\n\t\"Data\":\"{Data}\"\n}}";
        }

        public string SerilizeToJson()
        {
            return $"{{\"ClientID\":{ClientID},\"Path\":\"{Path}\",\"Type\":\"{Type}\",\"Data\":\"{Data}\"}}";
        }
    }
}
