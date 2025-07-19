using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServer_Backend.Communication
{
    public class ServerMessageFormat
    {
        public int StatusCode { get; set; } = 500; // e.g. 200, 404, 500
        public object Data { get; set; } = "Service Unavailable"; // e.g. Json data or other content

        public string SerilizeToJson()
        {
            return $"{{\"StatusCode\": {StatusCode}, \"Data\": \"{Data}\"}}";
        }
    }
}
